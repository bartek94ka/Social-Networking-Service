using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using LocalSocial.Models;
using LocalSocial.Models.Bindings;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using LocalSocial;
using System.Linq;

[Route("api/[controller]")]
public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger _logger;
    private readonly LocalSocialContext _context;

    public AccountController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ILoggerFactory loggerFactory)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = loggerFactory.CreateLogger<AccountController>();
        _context = new LocalSocialContext();
    }
    //
    // POST: /Account/Login
    [Route("login")]
    [HttpPost]
    [AllowAnonymous]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> Login([FromBody] Login model, string returnUrl = null)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented
        };
        Dictionary<string, string> messages = new Dictionary<string, string>();

        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return Ok();
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if( user != null)
            {
                messages.Add("Password", "Nieprawidłowe hasło");
            }
            else
            {
                messages.Add("email", "Nieprawidłowy adres e-mail");
            }
            string json = JsonConvert.SerializeObject(messages, settings);
            // If we got this far, something failed, redisplay form
            return HttpBadRequest(json);//View(model);
        }
        return HttpBadRequest();
    }

    [Route("remove")]
    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> RemoveAccount()
    {
        var userId = HttpContext.User.GetUserId();
        var user = _context.User.FirstOrDefault(x => x.Id == userId);
        var userComments = _context.Comment.Where(x => x.UserId == userId);
        _context.RemoveRange(userComments);
        var userFriends = _context.UserFriends.Where(x => x.FriendId == userId && x.UserId == userId);
        _context.RemoveRange(userFriends);
        var userPosts = _context.Post.Where(x => x._UserId == userId);
        var userPostsId = userPosts.Select(y => y.Id);
        var userPostsComments = from comment in _context.Comment
                                join postId in userPostsId on comment.PostId equals postId
                                select comment;
        _context.Comment.RemoveRange(userPostsComments);
        var postsTags = from postTag in _context.PostTags
                        join postId in userPostsId on postTag.PostId equals postId
                        select postTag;
        _context.PostTags.RemoveRange(postsTags);
        _context.Post.RemoveRange(userPosts);
        await _context.SaveChangesAsync();
        await _userManager.DeleteAsync(user);
        return Ok();
    }

    [Route("remove/{email}")]
    [HttpDelete]
    public async Task<IActionResult> RemoveAccountByEmail(string email)
    {
        var user = _context.User.FirstOrDefault(x => x.Email == email);
        if (user == null)
        {
            return HttpBadRequest();
        }

        var userComments = _context.Comment.Where(x => x.UserId == user.Id);
        _context.RemoveRange(userComments);
        var userFriends = _context.UserFriends.Where(x => x.FriendId == user.Id && x.UserId == user.Id);
        _context.RemoveRange(userFriends);
        var userPosts = _context.Post.Where(x => x._UserId == user.Id);
        var userPostsId = userPosts.Select(y => y.Id);
        var userPostsComments = from comment in _context.Comment
                                join postId in userPostsId on comment.PostId equals postId
                                select comment;
        _context.Comment.RemoveRange(userPostsComments);
        var postsTags = from postTag in _context.PostTags
                        join postId in userPostsId on postTag.PostId equals postId
                        select postTag;
        _context.PostTags.RemoveRange(postsTags);
        _context.Post.RemoveRange(userPosts);
        await _context.SaveChangesAsync();
        await _userManager.DeleteAsync(user);
        return Ok();
    }

    //
    // POST: /Account/Register
    [Route("register")]
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] Register model)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented
        };
        Dictionary<string, string> messages = new Dictionary<string, string>();

        if (ModelState.IsValid)
        {
            var user = new User { UserName = model.Email, Email = model.Email, SearchRange = 1000, Avatar = "default"};
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation(3, "User created a new account with password.");
                return Ok();
            }
            var user2 = await _userManager.FindByEmailAsync(model.Email);
            if (user2 != null)
            {
                messages.Add("email", "Podany e-mail jest już zajęty");
            }
            string json2 = JsonConvert.SerializeObject(messages, settings);
            return HttpBadRequest(json2);
        }
        if(model.Password == model.ConfirmPassword)
        {
            if (model.Password.Length < 6)
                messages.Add("Password", "Hasło musi mieć powyżej 6 znaków");
            else
                messages.Add("Password", "Hasło musi zawierać znak specjalny i liczbę");
        }else
        {
            messages.Add("ConfirmPassword", "Hasła muszą być identyczne");
        }
        string json = JsonConvert.SerializeObject(messages, settings);
        return HttpBadRequest(json);
    }

    //
    // POST: /Account/LogOff
    [Route("logoff")]
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> LogOff()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }

    #region Helpers

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    private async Task<User> GetCurrentUserAsync()
    {
        return await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
    }

    private IActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        else
        {
            return RedirectToAction("");
        }
    }

    #endregion
}