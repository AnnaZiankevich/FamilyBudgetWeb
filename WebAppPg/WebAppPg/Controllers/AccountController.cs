using WebAppPg.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Security.Claims;

namespace WebAppPg.Controllers
{
    public class AccountController : Controller
    {
        static List<Account> accountsList = new List<Account>();

        /*[HttpGet]
        public IActionResult List(int id)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            using (conn)
            {
                NpgsqlCommand cmd = new NpgsqlCommand("select id, name, account_type_code, is_active, currency_code" +
                                                        " from sb.accounts where account_owner_id = @accOwnerId order by name", conn);
                cmd.Parameters.AddWithValue("accOwnerId", NpgsqlDbType.Integer, id);
                accountsList.Clear();
                NpgsqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var account = new Account 
                    { 
                        id = rdr.GetInt32("id"),
                        name = rdr.GetString("name"),
                        account_type_code = rdr.GetString("account_type_code"),
                        account_owner_id = id,
                        is_active = rdr.GetBoolean("is_active"),
                        currency_code = rdr.GetString("currency_code")
                    };

                    accountsList.Add(account);
                    //Read(account, rdr);
                }
                DbConn.Instance.FreeConnection(conn);
            }
            return View(accountsList);
        }*/

        [HttpGet]
        public IActionResult List([Bind("accOwnerId")] int accOwnerid)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            using (conn)
            {
                NpgsqlCommand cmd = new NpgsqlCommand("select id, name, account_type_code, is_active, currency_code" +
                                                        " from sb.accounts where account_owner_id = @accOwnerId order by name", conn);
                cmd.Parameters.AddWithValue("accOwnerId", NpgsqlDbType.Integer, accOwnerid);
                accountsList.Clear();
                NpgsqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var account = new Account
                    {
                        id = rdr.GetInt32("id"),
                        name = rdr.GetString("name"),
                        account_type_code = rdr.GetString("account_type_code"),
                        account_owner_id = accOwnerid,
                        is_active = rdr.GetBoolean("is_active"),
                        currency_code = rdr.GetString("currency_code")
                    };

                    accountsList.Add(account);
                    //Read(account, rdr);
                }
                DbConn.Instance.FreeConnection(conn);
            }
            return View(accountsList);
        }
     }
}
