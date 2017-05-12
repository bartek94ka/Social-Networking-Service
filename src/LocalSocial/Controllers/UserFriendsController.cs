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
using LocalSocial.Services.EntityFrameworkServices;
using LocalSocial.Services.Interfaces;

namespace LocalSocial.Controllers
{
    [Route("api/[controller]")]
    public class UserFriendsController : Controller
    {
        private readonly LocalSocialContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IUserFriendsService _userFriendsService;

        public UserFriendsController(LocalSocialContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
            _userFriendsService = new UserFriendsService();
        }

        [Route("posts")]
        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<Post>> GetMyFriendsPosts()
        {
            var userId = HttpContext.User.GetUserId();
            var posts = _userFriendsService.GetMyFriendsPosts(userId);
            return posts;
        }

        [Route("myfriends")]
        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<User>> GetFriends()
        {
            var userId = HttpContext.User.GetUserId();
            var friends = _userFriendsService.GetFriends(userId);
            return friends;
        }

        [Route("find")]
        [HttpPost]
        [Authorize]
        public async Task<IEnumerable<User>> FindFriends([FromBody] UserBindingModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.User.GetUserId();
                var friends = _userFriendsService.FindFriends(userId, model);
                return friends;
            }
            return null;
        }

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
                if (user != null)
                {
                    var userfriend =
                        _context.UserFriends.FirstOrDefault(x => x.UserId == userId && x.FriendId == model.Id);
                    _context.UserFriends.Remove(userfriend);
                }
                await _context.SaveChangesAsync();
                return Ok();
            }
            return HttpBadRequest();
        }
    }
}