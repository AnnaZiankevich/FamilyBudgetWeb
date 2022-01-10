using Microsoft.AspNetCore.Mvc;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAppPg.Models;

namespace WebAppPg.Controllers
{
    public class AccountBalanceController : Controller
    {
        static AccountBalancesList accountBalList = new AccountBalancesList { accountBalancesList = new List<AccountBalanceListItem>(), account_id = -1 };

        [HttpGet]
        public IActionResult List([Bind("account_id")] int account_id)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            using (conn)
            {
                NpgsqlCommand cmd = new NpgsqlCommand("select a.id, a.account_id, a.period_id, a.amount, a.currency_code," +
                                                        " b.name as period_name, c.name as account_name" +
                                                        " from sb.account_balances a, sb.periods b, sb.accounts c" +
                                                        " where a.account_id = @account_id and b.id = a.period_id and c.id = a.account_id", conn);
                cmd.Parameters.AddWithValue("account_id", NpgsqlDbType.Integer, account_id);
                accountBalList.accountBalancesList.Clear();
                accountBalList.account_id = account_id;
                NpgsqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var accountBalance = new AccountBalanceListItem
                    {
                        id = rdr.GetInt32("id"),
                        account_id = account_id,
                        account_name = rdr.GetString("account_name"),
                        period_id = rdr.GetInt32("period_id"),
                        period_name = rdr.GetString("period_name"),
                        amount = rdr.GetDecimal("amount"),
                        currency_code = rdr.GetString("currency_code")                       
                    };

                    accountBalList.accountBalancesList.Add(accountBalance);
                }
                DbConn.Instance.FreeConnection(conn);
            }
            return View(accountBalList);
        }
    }
}
