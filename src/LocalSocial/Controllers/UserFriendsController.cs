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
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LocalSocial.Controllers
{
    [Route("api/[controller]")]
    public class UserFriendsController : Controller
    {
        private readonly LocalSocialContext _context;
        private readonly UserManager<User> _userManager;
        public UserFriendsController(LocalSocialContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Route("posts")]
        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<Post>> GetMyFriendsPosts()
        {
            var userId = HttpContext.User.GetUserId();
            var posts = (from p in _context.Post.Include(p => p.PostTags)
                         join uf in _context.UserFriends on p._UserId equals uf.FriendId
                         where uf.UserId == userId
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
        [Route("myfriends")]
        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<User>> GetFriends()
        {
            var userId = HttpContext.User.GetUserId();
            var entity = (from uf in _context.UserFriends
                          join us in _context.User on uf.FriendId equals us.Id
                          where uf.UserId == userId
                          select us).AsEnumerable();
            //var entity = _context.Friend.FirstOrDefault(x=>x.UserId == userId);

            return entity;
        }
        [Route("find")]
        [HttpPost]
        [Authorize]
        public async Task<IEnumerable<User>> FindFriends([FromBody] UserBindingModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.User.GetUserId();
                var user = _context.User.FirstOrDefault(x => x.Id == userId);
                //wszyscy poza biezacym uzytkownikiem
                var userfriends = (from u in _context.UserFriends
                                   where u.UserId == userId
                                   select u.FriendId).ToList();
                userfriends.Add(userId);
                var friends = (from u in _context.User
                               where !userfriends.Contains(u.Id)
                               select u);

                friends =
                    friends.Where(x => x.Name == model.Name || x.Surname == model.Surname || x.Email == model.Email);
                return friends.AsEnumerable();
            }
            return null;
        }
        //[Route("addfriend/{UserId:string}")]
        [Route("add")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddFriend([FromBody] UserBindingModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.User.GetUserId();
                var user = _context.User.FirstOrDefault(x => x.Id == userId);
                var friend = _context.User.FirstOrDefault(x => x.Id == model.Id);
                if (user != null && friend != null)
                {
                    _context.UserFriends.Add(new UserFriends()
                    {
                        FriendId = friend.Id,
                        UserId = user.Id
                    });
                }
                await _context.SaveChangesAsync();
                return Ok();
            }
            return HttpBadRequest();
        }
        [Route("remove")]
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> RemoveFriend([FromBody] UserBindingModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.User.GetUserId();
                var user = _context.User.FirstOrDefault(x => x.Id == userId);
                //var friend = _context.User.FirstOrDefault(x => x.Id == UserId);
                if (user != null)
                {
                    var userfriend =
                        _context.UserFriends.FirstOrDefault(x => x.UserId == userId && x.FriendId == model.Id);
                    _context.UserFriends.Remove(userfriend);
                    //user.Friends.Remove(friend);
                    //user.Friends.Friends.Remove(friend);
                }
                await _context.SaveChangesAsync();
                return Ok();
            }
            return HttpBadRequest();
        }
    }
}