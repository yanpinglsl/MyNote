using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIDConnectMVC.Models;
using System.Diagnostics;

namespace OpenIDConnectMVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            //string accessToken = HttpContext.GetTokenAsync("access_token").Result;
            //string idToken = HttpContext.GetTokenAsync("id_token").Result;
            var claimsList = from c in User.Claims select new { c.Type, c.Value };
            return View();
        }
        public IActionResult Logout()
        {
            //清楚本地Cookie以及Iddentity Server缓存
            return SignOut("Cookies", "oidc");
        }
    }
}
