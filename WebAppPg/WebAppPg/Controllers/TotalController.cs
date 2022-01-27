using Microsoft.AspNetCore.Mvc;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAppPg.Models;

namespace WebAppPg.Controllers
{
    public class TotalController : Controller
    {
        static TotalModelList totalList = new TotalModelList { totalIncomeModelList = new List<TotalIncomeModelListItem>(), totalPaymentsModelList = new List<TotalPaymentsModelListItem>() };

        [HttpGet]
        public IActionResult List()
        {
            int period_id = 202112;
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

            totalList.periodList = PeriodDAO.GetPeriodList(conn);
            totalList.period_id = period_id;
            totalList.accountList = AccountDAO.GetAccountListTotal(conn);
            totalList.account_id = account_id;
            totalList.currencyCodelist = CurrencyCodeDAO.GetCurrCodesList(conn);
            totalList.currency_code = currency_code;

            NpgsqlCommand incomeSelCmd = new NpgsqlCommand("select id, income_date, name, source_name, " +
                                                   " amount, currency_code, is_planned " +
                                                   " from sb.get_income_plus_within_period (pni_period_id => @period_id, " +
                                                   "  pvi_currency_code => @selCurr, " +
                                                   "  pii_account_id => @selAcc) order by 2, 3", conn);

            incomeSelCmd.Parameters.AddWithValue("period_id", NpgsqlDbType.Numeric, period_id);
            incomeSelCmd.Parameters.AddWithValue("selCurr", NpgsqlDbType.Varchar, currency_code);
            if (account_id == -1)
                incomeSelCmd.Parameters.AddWithValue("selAcc", NpgsqlDbType.Integer, DBNull.Value);
            else
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
            rdrInc.Close();

            NpgsqlCommand paymentsSelCmd = new NpgsqlCommand("select id, payment_date, name, receiver_name, " +
                                                 "amount, currency_code, is_planned " +
                                                 "from sb.get_payment_plus_whithin_period (pni_period_id => @period_id, " +
                                                 "  pvi_currency_code => @selCurr, " +
                                                 "  pii_account_id => @selAcc) order by 2, 3", conn);
            paymentsSelCmd.Parameters.AddWithValue("period_id", NpgsqlDbType.Numeric, period_id);
            paymentsSelCmd.Parameters.AddWithValue("selCurr", NpgsqlDbType.Varchar, currency_code);
            if (account_id == -1) 
                paymentsSelCmd.Parameters.AddWithValue("selAcc", NpgsqlDbType.Integer, DBNull.Value);
            else
                paymentsSelCmd.Parameters.AddWithValue("selAcc", NpgsqlDbType.Integer, account_id);
            totalList.totalPaymentsModelList.Clear();
            NpgsqlDataReader rdrPaym = paymentsSelCmd.ExecuteReader();
            while (rdrPaym.Read())
            {
                var totalPaym = new TotalPaymentsModelListItem
                {
                    payment_id = rdrPaym.GetInt32("id"),
                    payment_date = rdrPaym.GetDateTime("payment_date"),
                    payment_name = rdrPaym.GetString("name"),
                    payment_receiver_name = rdrPaym.GetString("receiver_name"),
                    payment_amount = rdrPaym.GetDecimal("amount"),
                    payment_currency_code = rdrPaym.GetString("currency_code"),
                    is_payment_planned = rdrPaym.GetBoolean("is_planned")

                };
                totalList.totalPaymentsModelList.Add(totalPaym);
            }
            rdrPaym.Close();

            DbConn.Instance.FreeConnection(conn);
            return View(totalList);

        }

        /*
        [HttpGet]
        public IActionResult List__([Bind("period_id")] int period_id)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            totalList.periodList = PeriodDAO.GetPeriodList(conn);
            totalList.period_id = 202112;
            totalList.accountList = AccountDAO.GetAccountListTotal(conn);
            totalList.account_id = -1;
            totalList.currencyCodelist = CurrencyCodeDAO.GetCurrCodesList(conn);
            totalList.currency_code = "BYN";
            DbConn.Instance.FreeConnection(conn);

            NpgsqlCommand paymentsSelCmd = new NpgsqlCommand();
            NpgsqlCommand incomeSelCmd = new NpgsqlCommand();

            incomeSelCmd.CommandText = "select a.id, a.income_date, a.name, b.name as source_name," +
                                                   " amount, currency_code, (a.planned_income_id is not null) as is_planned" +
                                                   " from sb.income a " +
                                                   " join sb.income_sources b on b.id = a.income_source_id" +
                                                   " order by name";
                totalList.totalIncomeModelList.Clear();
                incomeSelCmd.Connection = DbConn.Instance.GetMainConnection(int.Parse(userId));
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

                paymentsSelCmd.CommandText = "select a.id, a.payment_date, a.name, b.name as receiver_name," +
                                                   " amount, currency_code, (a.planned_payment_id is not null) as is_planned" +
                                                   " from sb.payments a " +
                                                   " join sb.payment_receivers b on b.id = a.payment_receiver_id" +
                                                   " order by name";
                totalList.totalPaymentsModelList.Clear();
                paymentsSelCmd.Connection = DbConn.Instance.GetMainConnection(int.Parse(userId));
                NpgsqlDataReader rdrPaym = paymentsSelCmd.ExecuteReader();
                while (rdrPaym.Read())
                {
                    var totalPaym = new TotalPaymentsModelListItem
                    {
                        payment_id = rdrPaym.GetInt32("id"),
                        payment_date = rdrPaym.GetDateTime("payment_date"),
                        payment_name = rdrPaym.GetString("name"),
                        payment_receiver_name = rdrPaym.GetString("receiver_name"),
                        payment_amount = rdrPaym.GetDecimal("amount"),
                        payment_currency_code = rdrPaym.GetString("currency_code"),
                        is_payment_planned = rdrPaym.GetBoolean("is_planned")

                    };
                    totalList.totalPaymentsModelList.Add(totalPaym);
                }
               
            return View(totalList);
        }

        [HttpPost]
        public IActionResult List__([Bind("account_id")] int account_id, [Bind("period_id")] int period_id, [Bind("currency_code")] string currency_code)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            totalList.periodList = PeriodDAO.GetPeriodList(conn);
            totalList.period_id = period_id;
            totalList.accountList = AccountDAO.GetAccountListTotal(conn);
            totalList.account_id = account_id;
            totalList.currencyCodelist = CurrencyCodeDAO.GetCurrCodesList(conn);
            totalList.currency_code = currency_code;
            DbConn.Instance.FreeConnection(conn);

            NpgsqlCommand paymentsSelCmd = new NpgsqlCommand();
            NpgsqlCommand incomeSelCmd = new NpgsqlCommand();

            incomeSelCmd.CommandText = "select id, income_date, name, source_name, " +
                                                   " amount, currency_code, is_planned " +
                                                   " from sb.get_income_plus_within_period (pni_period_id => @period_id, " +
                                                   "  pvi_currency_code => @selCurr, " +
                                                   "  pii_account_id => @selAcc) order by 2, 3";
            incomeSelCmd.Parameters.AddWithValue("period_id", NpgsqlDbType.Numeric, period_id);
            incomeSelCmd.Parameters.AddWithValue("selCurr", NpgsqlDbType.Varchar, currency_code);
            incomeSelCmd.Parameters.AddWithValue("selAcc", NpgsqlDbType.Integer, account_id);
            totalList.totalIncomeModelList.Clear();
            incomeSelCmd.Connection = DbConn.Instance.GetMainConnection(int.Parse(userId));
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

            paymentsSelCmd.CommandText = "select id, payment_date, name, receiver_name, " +
                                                 "amount, currency_code, is_planned " +
                                                 "from sb.get_payment_plus_whithin_period (pni_period_id => @period_id, " +
                                                 "  pvi_currency_code => @selCurr, " +
                                                 "  pii_account_id => @selAcc) order by 2, 3";
            paymentsSelCmd.Parameters.AddWithValue("period_id", NpgsqlDbType.Numeric, period_id);
            paymentsSelCmd.Parameters.AddWithValue("selCurr", NpgsqlDbType.Varchar, currency_code);
            paymentsSelCmd.Parameters.AddWithValue("selAcc", NpgsqlDbType.Integer, account_id);
            totalList.totalPaymentsModelList.Clear();
            paymentsSelCmd.Connection = DbConn.Instance.GetMainConnection(int.Parse(userId));
            NpgsqlDataReader rdrPaym = paymentsSelCmd.ExecuteReader();
            while (rdrPaym.Read())
            {
                var totalPaym = new TotalPaymentsModelListItem
                {
                    payment_id = rdrPaym.GetInt32("id"),
                    payment_date = rdrPaym.GetDateTime("payment_date"),
                    payment_name = rdrPaym.GetString("name"),
                    payment_receiver_name = rdrPaym.GetString("receiver_name"),
                    payment_amount = rdrPaym.GetDecimal("amount"),
                    payment_currency_code = rdrPaym.GetString("currency_code"),
                    is_payment_planned = rdrPaym.GetBoolean("is_planned")

                };
                totalList.totalPaymentsModelList.Add(totalPaym);
            }

            return View(totalList);
        }

        */
    }
}
