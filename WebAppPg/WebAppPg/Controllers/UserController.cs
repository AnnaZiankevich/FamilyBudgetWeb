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
        public IActionResult LogIn()
        {
            return View();
        }

        [System.Web.Http.HttpPost]
        public async Task<IActionResult> PerformLogin([Bind] UserModel userdetails)
        {
            if ((!string.IsNullOrEmpty(userdetails.Id)) && (!string.IsNullOrEmpty(userdetails.Password)) && (!string.IsNullOrEmpty(userdetails.Login)))
            {
                if ((userdetails.Id.Equals("admin") && userdetails.Password.Equals("admin") && userdetails.Login.Equals("admin")))
                {
                    var claims = new List<Claim>
                    {
                    new Claim(ClaimTypes.Name, userdetails.Id),
                    new Claim(ClaimTypes.Role, "User"),
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

                    return RedirectToAction("LogIn", "Home");
                }
            }
            return View("LogIn");
        }
    }
}
