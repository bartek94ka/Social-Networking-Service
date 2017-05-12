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

namespace LocalSocial.Controllers
{
    [Route("api/[controller]")]
    public class CommentsController : Controller
    {
        private readonly LocalSocialContext _context;
        private readonly UserManager<User> _userManager;
        public CommentsController(LocalSocialContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Route("add")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment([FromBody] CommentBindingModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.User.GetUserId();
                var user = _context.User.FirstOrDefault(x => x.Id == userId);
                _context.Comment.Add(
                    new Comment()
                    {
                        Content = model.Content,
                        UserId = userId,
                        User = user,
                        PostId = model.PostId
                    });
                await _context.SaveChangesAsync();
                await _context.SaveChangesAsync();
                return Ok();
            }
            return HttpBadRequest();
        }
    }
}
