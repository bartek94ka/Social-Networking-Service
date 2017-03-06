using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LocalSocial.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;

namespace LocalSocial.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly LocalSocialContext _context;
        private readonly UserManager<User> _userManager;

        public UsersController(LocalSocialContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Route("get")]
        [HttpGet]
        //[Authorize]
        //public async Task<IActionResult> User()
        public IActionResult Users()
        {
            var userId = HttpContext.User.GetUserId();
            var user = _context.User.FirstOrDefault(x => x.Id == userId);
            if (user != null)
                return Ok(new UserBindingModel { Name = user.Name, Surname = user.Surname, SearchRange = user.SearchRange, Avatar = user.Avatar });
            else
                return Ok();
        }
        [Route("edit")]
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Users([FromBody] UserBindingModel model)
        //public async Task<IActionResult> Users()
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.User.GetUserId();
                var user = _context.User.FirstOrDefault(x => x.Id == userId);
                if (user != null)
                {
                    user.Name = model.Name;
                    user.Surname = model.Surname;
                    user.SearchRange = model.SearchRange;
                    user.Avatar = model.Avatar;
                    //await _userManager.ChangePasswordAsync(User, model.OldPassword, model.NewPassword);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
            }
            return HttpBadRequest();
        }
        [Route("getUser")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> GetUser([FromBody] UserBindingModel model)
        //public async Task<IActionResult> Users()
        {
            if (ModelState.IsValid)
            {
                var user = _context.User.FirstOrDefault(x => x.Id == model.Id);
                if (user != null)
                {
                    return Ok(user);
                }
            }
            return HttpBadRequest();
        }
    }
}
