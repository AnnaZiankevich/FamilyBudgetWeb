using Microsoft.AspNetCore.Mvc;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAppPg.Models;

namespace WebAppPg.Controllers
{
    public class AppUserController : Controller
    {
        [HttpGet]
        public IActionResult RegisterNewUser()
        {
            NpgsqlConnection conn = DbConn.Instance.GetUsersConnection();
            AppUser appUser = new AppUser();
            DbConn.Instance.FreeConnection(conn);
            return View(appUser);
        }

        [HttpPost]
        public IActionResult RegisterNewUser([Bind] AppUser appUser) //("Name", "Login", "Password", "Is_active")
        {
            NpgsqlConnection conn = DbConn.Instance.GetUsersConnection();

            using (conn)
            {
                string request = "select sb.create_app_user (pii_app_user_id => @id, pvi_user_name => @Name, " +
                                                    "pvi_user_login => @Login, pvi_user_password => @Password, " +
                                                    "pbi_is_active => @Is_active)";
                NpgsqlCommand cmd = new NpgsqlCommand(request, conn);
                //SetCommandType(cmd, CommandType.Text);
                cmd.Parameters.AddWithValue("id", NpgsqlDbType.Integer, DBNull.Value);
                cmd.Parameters.AddWithValue("Name", NpgsqlDbType.Varchar, appUser.Name);
                cmd.Parameters.AddWithValue("Login", NpgsqlDbType.Varchar, appUser.Login);
                cmd.Parameters.AddWithValue("Password", NpgsqlDbType.Varchar, appUser.Password);
                cmd.Parameters.AddWithValue("Is_active", NpgsqlDbType.Boolean, appUser.Is_active);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                DbConn.Instance.FreeConnection(conn);
            }

            return Redirect("/User/LoginForm");
        }
    }
}
