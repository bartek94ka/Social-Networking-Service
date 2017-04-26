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
using LocalSocial.Services.Interfaces;
//musimy wybraæ sciezke do jednego serwisu
//drugi using musi byc zakomentowany
//using LocalSocial.Services.EntityFrameworkServices;
using LocalSocial.Services.DapperServices;

namespace LocalSocial.Controllers
{
    [Route("api/[controller]")]
    public class PostsController : Controller
    {
        private readonly LocalSocialContext _context;
        private readonly UserManager<User> _userManager;
        private readonly PostService _postService;

        public PostsController(LocalSocialContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
            _postService = new PostService();
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
                var posts = _postService.GetPostsInRange(user, model);
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
                var posts = _postService.GetPostsByTag(model);
                return posts;
            }
            return null;
        }

        [Route("post/{Id:int}")]
        [HttpGet]
        [Authorize]
        public IActionResult GetPost(int Id)
        {
            var post = _postService.GetPost(Id);
            if (post == null)
            {
                return HttpBadRequest();
            }
            else
            {
                return Ok(post);
            }
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
        public async Task<IEnumerable<Post>> GetAllPosts()
        {
            var posts = _postService.GetAllPosts();
            return posts;
        }

        [Route("myposts")]
        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<Post>> GetMyPosts()
        {
            var userId = HttpContext.User.GetUserId();
            var posts = _postService.GetMyPosts(userId);
            return posts;
        }
    }
}