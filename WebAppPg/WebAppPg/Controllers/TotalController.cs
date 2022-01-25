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
    public class TotalController : Controller
    {
        //static TotalModelList totalIncModList = new TotalModelList { totalIncomeModelList = new List<TotalIncomeModelListItem>() };
        //static TotalModelList totalPaymModList = new TotalModelList { totalPaymentsModelList = new List<TotalPaymentsModelListItem>() };
        static TotalModelList totalList = new TotalModelList { totalIncomeModelList = new List<TotalIncomeModelListItem>(), totalPaymentsModelList = new List<TotalPaymentsModelListItem>() };

        [HttpGet]
        public IActionResult List([Bind("account_id")] int account_id, [Bind("period_id")] int period_id, [Bind("currency_code")] string currency_code)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection connInc = DbConn.Instance.GetMainConnection(int.Parse(userId));
            NpgsqlConnection connPaym = DbConn.Instance.GetMainConnection(int.Parse(userId));

            using (connInc)
            {
                totalList.periodList = PeriodDAO.GetPeriodList(connInc);
                totalList.accountList = AccountDAO.GetAccountList(connInc);
                totalList.currencyCodelist = CurrencyCodeDAO.GetCurrCodesList(connInc);
                NpgsqlCommand incomeSelCmd = new NpgsqlCommand("select id, income_date, name, source_name, " +
                                                   " amount, currency_code, is_planned " +
                                                   " from sb.get_income_plus_within_period (pni_period_id => @period_id, " +
                                                   "    pvi_currency_code => @selCurr, " +
                                                   "    pii_account_id => @selAcc) order by 2, 3", connInc);
                incomeSelCmd.Parameters.AddWithValue("period_id", NpgsqlDbType.Numeric, period_id);
                incomeSelCmd.Parameters.AddWithValue("selCurr", NpgsqlDbType.Varchar, currency_code);
                incomeSelCmd.Parameters.AddWithValue("selAcc", NpgsqlDbType.Integer, account_id);
                totalList.totalIncomeModelList.Clear();
                NpgsqlDataReader rdrInc = incomeSelCmd.ExecuteReader();
                while (rdrInc.Read())
                {
                    var totalInc = new TotalIncomeModelListItem
                    {
                        income_id = rdrInc.GetInt32("id"),
                        income_date = rdrInc.GetDateTime("income_date"),
                        income_name = rdrInc.GetString("name"),
                        income_source_name = rdrInc.GetString("source_name"),
                        income_amount = rdrInc.GetDecimal("amount"),
                        income_currency_code = rdrInc.GetString("currency_code"),
                        is_income_planned = rdrInc.GetBoolean("is_planned")

                    };
                    totalList.totalIncomeModelList.Add(totalInc);
                }
                DbConn.Instance.FreeConnection(connInc);
            }

            using (connPaym)
            {
                NpgsqlCommand paymentsSelCmd = new NpgsqlCommand("select id, payment_date, name, receiver_name, " +
                                                 "amount, currency_code, is_planned " +
                                                 "from sb.get_payment_plus_whithin_period (pni_period_id => @period_id, " +
                                                 "    pvi_currency_code => @selCurr, " +
                                                 "    pii_account_id => @selAcc) order by 2, 3", connPaym);
                paymentsSelCmd.Parameters.AddWithValue("period_id", NpgsqlDbType.Numeric, period_id);
                paymentsSelCmd.Parameters.AddWithValue("selCurr", NpgsqlDbType.Varchar, currency_code);
                paymentsSelCmd.Parameters.AddWithValue("selAcc", NpgsqlDbType.Integer, account_id);
                totalList.totalPaymentsModelList.Clear();
                NpgsqlDataReader rdrPaym = paymentsSelCmd.ExecuteReader();
                while (rdrPaym.Read())
                {
                    var totalPaym = new TotalPaymentsModelListItem
                    {
                        payment_id = rdrPaym.GetInt32("id"),
                        payment_date = rdrPaym.GetDateTime("income_date"),
                        payment_name = rdrPaym.GetString("name"),
                        payment_receiver_name = rdrPaym.GetString("source_name"),
                        payment_amount = rdrPaym.GetDecimal("amount"),
                        payment_currency_code = rdrPaym.GetString("currency_code"),
                        is_payment_planned = rdrPaym.GetBoolean("is_planned")

                    };
                    totalList.totalPaymentsModelList.Add(totalPaym);
                }
              DbConn.Instance.FreeConnection(connPaym);
            }
                
            return View(totalList);
        }
    }
}
