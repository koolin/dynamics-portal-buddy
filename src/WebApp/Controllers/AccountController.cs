using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using WebApp.Extensions;

namespace WebApp.Controllers
{    
public class AccountController : Controller
{
    private readonly AzureAdB2COidcOptions _azureAdB2COptions;

    public AccountController(IOptions<AzureAdB2COidcOptions> authOptions)
    {
        _azureAdB2COptions = authOptions.Value;
    }

    // GET: /Account/SignIn
    [HttpGet]
    public async Task<IActionResult> SignIn(string returnUrl = "/")
    {
        if (HttpContext.User == null || !HttpContext.User.Identity.IsAuthenticated)
        {
            var authenticationProperties = new AuthenticationProperties { RedirectUri = Url.Action("LoginCallback", "Account", new { rurl = returnUrl }) };
            await HttpContext.Authentication.ChallengeAsync(_azureAdB2COptions.DefaultPolicy.ToLower(), authenticationProperties);
        }

        return Redirect(returnUrl);
    }


    [HttpGet]
    public async Task<IActionResult> LoginCallback(string rurl)
    {
        if (HttpContext.User.Identity.IsAuthenticated)
        {
            return Redirect(rurl);
        }

        return RedirectToAction("Index", new { rurl });
    }

    // GET: /Account/LogOff
    [HttpGet]
    public async Task LogOff()
    {
        if (HttpContext.User != null && HttpContext.User.Identity.IsAuthenticated)
        {
            var scheme = (HttpContext.User.FindFirst("tfp"))?.Value;
                
            if (string.IsNullOrEmpty(scheme))
                scheme = (HttpContext.User.FindFirst("http://schemas.microsoft.com/claims/authnclassreference"))?.Value;

            await HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.Authentication.SignOutAsync(scheme.ToLower(), new AuthenticationProperties { RedirectUri = "/" });
        }
    }
}
}