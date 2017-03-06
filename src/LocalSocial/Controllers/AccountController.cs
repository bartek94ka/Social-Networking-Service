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

[Route("api/[controller]")]
public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger _logger;

    public AccountController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ILoggerFactory loggerFactory)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = loggerFactory.CreateLogger<AccountController>();
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
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
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
            string json2 = JsonConvert.SerializeObject(messages, settings);
            // If we got this far, something failed, redisplay form
            return HttpBadRequest(json2);//View(model);
        }
        return HttpBadRequest();//View(model);
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