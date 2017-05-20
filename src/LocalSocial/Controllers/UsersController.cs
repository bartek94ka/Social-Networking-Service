using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LocalSocial.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using LocalSocial.Services.Interfaces;
//musimy wybrać sciezke do jednego serwisu
//drugi using musi byc zakomentowany
//using LocalSocial.Services.EntityFrameworkServices;
using LocalSocial.Services.DapperServices;

namespace LocalSocial.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly LocalSocialContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IUsersService _usersService;

        public UsersController(LocalSocialContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
            _usersService = new UsersService();
        }

        [Route("get")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMyUserData()
        {
            var userId = HttpContext.User.GetUserId();
            var user = _usersService.GetMyUserData(userId);
            return Ok(user);
        }

        [Route("edit")]
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Users([FromBody] UserBindingModel model)
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
        {
            if (ModelState.IsValid)
            {
                var userData = _usersService.GetUserData(model);
                if(userData != null)
                {
                    return Ok(userData);
                }
            }
            return HttpBadRequest();
        }
    }
}
