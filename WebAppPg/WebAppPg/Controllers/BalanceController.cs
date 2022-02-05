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
    public class BalanceController : Controller
    {
        static BalancesListModel balList = new BalancesListModel { balancesList = new List<BalancesListItem>() };

        [HttpGet]
        public IActionResult List()
        {
            int period_id = 202111;
            int account_id = -1;
            string currency_code = "BYN";
            return processRequest(period_id, account_id, currency_code);
        }

        [HttpPost]
        public IActionResult List([Bind("period_id")] int period_id, [Bind("account_id")] int account_id, [Bind("currency_code")] string currency_code)
        {
            return processRequest(period_id, account_id, currency_code);
        }

        private IActionResult processRequest(int period_id, int account_id, string currency_code)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));

            balList.periodList = PeriodDAO.GetPeriodList(conn);
            balList.period_id = period_id;
            balList.accountList = AccountDAO.GetAccountListTotal(conn);
            balList.account_id = account_id;
            balList.currencyCodelist = CurrencyCodeDAO.GetCurrCodesList(conn);
            balList.currency_code = currency_code;

            NpgsqlCommand incomeSelCmd = new NpgsqlCommand("select id, amount, currency_code " +
                                                   "  from sb.get_account_balance_whithin_period (pni_period_id => @period_id, " +
                                                   "  pvi_currency_code => @selCurr, " +
                                                   "  pii_account_id => @selAcc) order by 2, 3", conn);
            incomeSelCmd.Parameters.AddWithValue("period_id", NpgsqlDbType.Numeric, period_id);
            incomeSelCmd.Parameters.AddWithValue("selCurr", NpgsqlDbType.Varchar, currency_code);
            if (account_id == -1)
                incomeSelCmd.Parameters.AddWithValue("selAcc", NpgsqlDbType.Integer, DBNull.Value);
            else
                incomeSelCmd.Parameters.AddWithValue("selAcc", NpgsqlDbType.Integer, account_id);
            balList.balancesList.Clear();
            NpgsqlDataReader rdrInc = incomeSelCmd.ExecuteReader();
            while (rdrInc.Read())
            {
                var balance = new BalancesListItem
                {
                    id = rdrInc.GetInt32("id"),
                    amount = rdrInc.GetDecimal("amount"),
                    currency_code = rdrInc.GetString("currency_code"),
                };
                balList.balancesList.Add(balance);
            }
            rdrInc.Close();

            DbConn.Instance.FreeConnection(conn);
            return View(balList);

        }
    }
}
