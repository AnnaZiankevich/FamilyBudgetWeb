using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using WebAppPg.Models;

namespace WebAppPg.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        [System.Web.Http.HttpGet]
        public IActionResult LoginForm()
        {
            return View();
        }

        [System.Web.Http.HttpPost]
        public async Task<IActionResult> PerformLogin([Bind] UserModel userdetails)
        {
            if ((!string.IsNullOrEmpty(userdetails.Password)) && (!string.IsNullOrEmpty(userdetails.Login)))
            {
                UserModel appUser = AppUserDAO.FindByName(userdetails.Login);
                if (appUser != null)
                //userdetails.Password.Equals("admin")))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, appUser.Login),
                        new Claim(ClaimTypes.Role, "User"),
                        new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.Now.AddMinutes(10),
                    };

                    await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                    return RedirectToAction("Index", "Home");
                }
            }
            return View("LoginForm");
        }
    }
}
