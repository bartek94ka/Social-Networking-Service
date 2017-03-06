using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using LocalSocial;
using LocalSocial.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Geolocation;

namespace LocalSocial.Controllers
{
    [Route("api/[controller]")]
    public class PostsController : Controller
    {
        private readonly LocalSocialContext _context;
        private readonly UserManager<User> _userManager;

        public PostsController(LocalSocialContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Route("inrange")]
        [HttpPost]
        [Authorize]
        public IEnumerable<Post> GetPostsInRange([FromBody] Location model)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.User.GetUserId();
                var user = _context.User.FirstOrDefault(x => x.Id == userId);
                //range - dystans w kilometrach
                //GeoCalculation.GetDistance(...) - zwraca dystans w milach => 1 mila = 1.6 km
                var posts = (from p in _context.Post.Include(p => p.PostTags)
                             let range = user.SearchRange / 1000
                             where
                             range >=
                             GeoCalculator.GetDistance(model.Latitude, model.Longitude, p.Latitude, p.Longitude, 5) / 1.6
                             orderby p.AddDate descending
                             select p
                                 ).ToList();
                for (int i = 0; i < posts.Count; i++)
                {
                    posts[i].user = _context.User.FirstOrDefault(x => x.Id == posts[i]._UserId);
                }
                return posts;
            }
            return null;
        }
        [Route("add")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPost([FromBody] PostBindingModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.User.GetUserId();
                var user = _context.User.FirstOrDefault(x => x.Id == userId);
                var newPost = new Post()
                {
                    Title = model.Title,
                    Description = model.Description,
                    AddDate = DateTime.Now,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude,
                    _UserId = userId,
                    user = user
                };
                _context.Post.Add(newPost);
                _context.SaveChanges();
                var addedPost = _context.Post.Last();
                foreach (var item in model.Tags)
                {
                    var tag = _context.Tag.FirstOrDefault(x => x.Id == item);
                    if (tag == null)
                    {
                        tag = new Tag
                        {
                            Id = item
                        };
                        _context.Tag.Add(tag);
                    }
                    addedPost.PostTags.Add(new PostTags
                    {
                        PostId = addedPost.Id,
                        TagId = tag.Id
                    });
                }
                await _context.SaveChangesAsync();
                return Ok();
            }
            return HttpBadRequest();
        }
        // GET: api/Posts
        [Route("tag")]
        [HttpPost]
        [Authorize]
        public async Task<IEnumerable<Post>> GetPostsByTag([FromBody] TagBindingModel model)
        {
            if (ModelState.IsValid)
            {
                var posts = (from p in _context.Post.Include(p => p.PostTags)
                             join pt in _context.PostTags on p.Id equals pt.PostId
                             where pt.TagId == model.TagId
                             orderby p.AddDate descending
                             select p).ToList();
                for (int i = 0; i < posts.Count; i++)
                {
                    var user = (from us in _context.User
                                where us.Id == posts[i]._UserId
                                select new User { Name = us.Name, Surname = us.Surname, Email = us.Email, Avatar = us.Avatar });
                    posts[i].user = user.FirstOrDefault();
                }
                
                return posts;
            }
            return null;
        }

        [Route("post/{Id:int}")]
        [HttpGet]
        [Authorize]
        public IActionResult GetPost(int Id)
        {
            Post post = _context.Post.Include(x => x.Comments).ThenInclude(p => p.User).Include(x => x.PostTags).First(x => x.Id == Id);
            var user = (from us in _context.User
                        where us.Id == post._UserId
                        select new User { Name = us.Name, Surname = us.Surname, Email = us.Email, Avatar = us.Avatar });
            if (user != null && post != null)
            {
                post.user = user.FirstOrDefault();
                return Ok(post);
            }
            return HttpBadRequest();
        }

        [Route("edit/{Id:int}")]
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> EditPost(int Id, [FromBody] PostBindingModel model)
        {
            if (ModelState.IsValid)
            {
                Post post = await _context.Post.Include(p=>p.PostTags).FirstAsync(x => x.Id == Id);
                if (post != null)
                {
                    post.Title = model.Title;
                    post.Description = model.Description;
                    foreach (var item in post.PostTags)
                    {
                        _context.Remove(item);
                    }
                    try
                    {
                        _context.SaveChanges();
                        var editedPost = _context.Post.Where(x => x.Id == Id).First();
                        foreach (var item in model.Tags)
                        {
                            var tag = _context.Tag.FirstOrDefault(x => x.Id == item);
                            if (tag == null)
                            {
                                tag = new Tag
                                {
                                    Id = item
                                };
                                _context.Tag.Add(tag);
                            }
                            editedPost.PostTags.Add(new PostTags
                            {
                                PostId = editedPost.Id,
                                TagId = tag.Id
                            });
                        }
                        return Ok();
                    }
                    catch
                    {
                        return HttpBadRequest();
                    }
                }
            }
            return HttpBadRequest();
        }

        [Route("delete/{Id:int}")]
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeletePost(int Id)
        {
            Post post = await _context.Post.Include(x=>x.Comments).Include(x=>x.PostTags).FirstAsync(x => x.Id == Id);
            if (post != null)
            {
                try
                {
                    foreach (var item in post.Comments)
                    {
                        _context.Remove(item);
                    }
                    foreach (var item in post.PostTags)
                    {
                        _context.Remove(item);
                    }
                    _context.Remove(post);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                catch
                {
                    return HttpBadRequest();
                }
            }
            return HttpBadRequest();
        }
        // GET: api/Posts
        [Route("all")]
        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<Post>> GetPosts()
        {
            var userId = HttpContext.User.GetUserId();
            //var posts = _context.Post.AllAsync(x => x.UserId == userId);
            var posts = _context.Post.AsEnumerable();
            return posts;
        }

        [Route("myposts")]
        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<Post>> GetMyPosts()
        {
            var userId = HttpContext.User.GetUserId();
            //var posts = _context.Post.Include(x=>x.user).AsQueryable().Where(x => x._UserId == userId).OrderByDescending(p => p.AddDate);
            var posts = _context.Post.Include(x => x.PostTags).Where(x => x._UserId == userId).OrderByDescending(p => p.AddDate).ToList();
            for (int i = 0; i < posts.Count; i++)
            {
                posts[i].user = _context.User.FirstOrDefault(x => x.Id == posts[i]._UserId);
            }
            return posts;
        }
    }
}