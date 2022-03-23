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
    public class InteraccountTransactionController : Controller
    {
        static InteraccountTransactionListModel interAccList = new InteraccountTransactionListModel { interaccountTransactionList = new List<InteraccountTransactionItemList>() };
        public IActionResult Index()
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            using (conn)
            {
                NpgsqlCommand cmd = new NpgsqlCommand("select a.id, a.transaction_date, a.source_account_id," +
                                                        "b.name as source_account_name, a.target_account_id," +
                                                        "c.name as target_account_name, a.source_amount, " +
                                                        "a.source_currency_code, a.target_amount, a.target_currency_code, " +
                                                        "a.row_version from sb.interaccount_transactions a " +
                                                        "join sb.accounts b on b.id = a.source_account_id " +
                                                        "join sb.accounts c on c.id = a.target_account_id " +
                                                        "order by a.id", conn);
                interAccList.interaccountTransactionList.Clear();
                NpgsqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var interAccount = new InteraccountTransactionItemList
                    {
                        id = rdr.GetInt32("id"),
                        transaction_date = rdr.GetDateTime("transaction_date"),
                        source_account_id = rdr.GetInt32("source_account_id"),
                        source_account_name = rdr.GetString("source_account_name"),
                        target_account_id = rdr.GetInt32("target_account_id"),
                        target_account_name = rdr.GetString("target_account_name"),
                        source_amount = rdr.GetDecimal("source_amount"),
                        source_currency_code = rdr.GetString("source_currency_code"),
                        target_amount = rdr.GetDecimal("target_amount"),
                        target_currency_code = rdr.GetString("target_currency_code"),
                        row_version = rdr.GetInt32("row_version")
                    };

                    interAccList.interaccountTransactionList.Add(interAccount);
                }
                DbConn.Instance.FreeConnection(conn);
            }
            return View(interAccList);
        }

        [HttpGet]
        public IActionResult AddFirstStage()
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            InteraccountTransaction interAccTransaction = new InteraccountTransaction();
            interAccTransaction.sourceAccountList = AccountDAO.GetAccountList(conn);
            interAccTransaction.targetAccountList = AccountDAO.GetAccountList(conn);
            interAccTransaction.sourceCurrCodesList = CurrencyCodeDAO.GetCurrCodesList(conn);
            DbConn.Instance.FreeConnection(conn);
            return View(interAccTransaction);
        }

        [HttpPost]
        public IActionResult AddFirstStage([Bind("transaction_date", "source_account_id", "target_account_id", "source_amount", "source_currency_code")] InteraccountTransaction interAccTransaction)
        {
            return RedirectToAction("AddSecondStage",
                       new
                       {
                           transaction_date = interAccTransaction.transaction_date,
                           source_account_id = interAccTransaction.source_account_id,
                           target_account_id = interAccTransaction.target_account_id,
                           source_amount = interAccTransaction.source_amount,
                           source_currency_code = interAccTransaction.source_currency_code
                       });
        }

        [HttpGet]
        public IActionResult AddSecondStage(DateTime transaction_date, int source_account_id, int target_account_id, decimal source_amount, string source_currency_code)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            InteraccountTransaction interAccTransaction = new InteraccountTransaction();
            interAccTransaction.sourceAccountList = AccountDAO.GetAccountList(conn);
            interAccTransaction.source_account_id = source_account_id;
            interAccTransaction.targetAccountList = AccountDAO.GetAccountList(conn);
            interAccTransaction.source_currency_code = source_currency_code;
            interAccTransaction.target_account_id = target_account_id;
            interAccTransaction.source_amount = source_amount;
            interAccTransaction.targetCurrCodesList = CurrencyCodeDAO.GetCurrCodesList(conn);
            interAccTransaction.target_currency_code = GetCurrencyCodeForAccount(target_account_id);
            interAccTransaction.transaction_date = transaction_date;
            interAccTransaction.target_amount = ConvertAmount(source_amount, source_currency_code, interAccTransaction.target_currency_code, transaction_date);
            DbConn.Instance.FreeConnection(conn);
            return View(interAccTransaction);
        }

        [HttpPost]
        public IActionResult AddSecondStage([Bind("transaction_date", "source_account_id", "target_account_id", "source_amount", "target_amount", "source_currency_code", "target_currency_code", "row_version")] InteraccountTransaction interAccTransaction)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));

            using (conn)
            {
                string request = "select sb.modify_interaccount_transaction (pii_transaction_id => @id, " +
                                                    "pdi_transaction_date => @transaction_date, " +
                                                    "pii_source_account_id => @source_account_id, " +
                                                    "pii_target_account_id => @target_account_id, " +
                                                    "pni_source_amount => @source_amount, " +
                                                    "pvi_source_cur_code => @source_currency_code, " +
                                                    "pni_target_amount => @target_amount, " +
                                                    "pvi_target_cur_code => @target_currency_code, " +
                                                    "pii_row_version => @row_version)";
                NpgsqlCommand cmd = new NpgsqlCommand(request, conn);
                //SetCommandType(cmd, CommandType.Text);
                cmd.Parameters.AddWithValue("id", NpgsqlDbType.Integer, DBNull.Value);
                cmd.Parameters.AddWithValue("transaction_date", NpgsqlDbType.Date, interAccTransaction.transaction_date);
                cmd.Parameters.AddWithValue("source_account_id", NpgsqlDbType.Integer, interAccTransaction.source_account_id);
                cmd.Parameters.AddWithValue("target_account_id", NpgsqlDbType.Integer, interAccTransaction.target_account_id);
                cmd.Parameters.AddWithValue("source_amount", NpgsqlDbType.Numeric, Convert.ToDecimal(interAccTransaction.source_amount));
                cmd.Parameters.AddWithValue("source_currency_code", NpgsqlDbType.Varchar, interAccTransaction.source_currency_code);
                cmd.Parameters.AddWithValue("target_amount", NpgsqlDbType.Numeric, Convert.ToDecimal(interAccTransaction.target_amount));
                cmd.Parameters.AddWithValue("target_currency_code", NpgsqlDbType.Varchar, interAccTransaction.target_currency_code);
                cmd.Parameters.AddWithValue("row_version", NpgsqlDbType.Integer, interAccTransaction.row_version);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                DbConn.Instance.FreeConnection(conn);
            }

            return Redirect("/InteraccountTransaction/Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            InteraccountTransaction interAccTransaction = InteraccountTransactionDAO.FindById(id);
            interAccTransaction.sourceAccountList = AccountDAO.GetAccountList(conn);
            interAccTransaction.targetAccountList = AccountDAO.GetAccountList(conn);
            interAccTransaction.sourceCurrCodesList = CurrencyCodeDAO.GetCurrCodesList(conn);
            interAccTransaction.targetCurrCodesList = CurrencyCodeDAO.GetCurrCodesList(conn);
            DbConn.Instance.FreeConnection(conn);
            return View(interAccTransaction);
        }

        [HttpPost]
        public IActionResult Edit(int id, [Bind("transaction_date", "source_account_id", "target_account_id", "source_amount", "target_amount", "source_currency_code", "target_currency_code", "row_version")] InteraccountTransaction interAccTransaction)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));

            using (conn)
            {
                string request = "select sb.modify_interaccount_transaction (pii_transaction_id => @id, " +
                                                    "pdi_transaction_date => @transaction_date, " +
                                                    "pii_source_account_id => @source_account_id, " +
                                                    "pii_target_account_id => @target_account_id, " +
                                                    "pni_source_amount => @source_amount, " +
                                                    "pvi_source_cur_code => @source_cur_code, " +
                                                    "pni_target_amount => @target_amount, " +
                                                    "pvi_target_cur_code => @target_cur_code, " +
                                                    "pii_row_version => @row_version)";
                NpgsqlCommand cmd = new NpgsqlCommand(request, conn);
                //SetCommandType(cmd, CommandType.Text);
                cmd.Parameters.AddWithValue("id", NpgsqlDbType.Integer, id);
                cmd.Parameters.AddWithValue("transaction_date", NpgsqlDbType.Date, interAccTransaction.transaction_date);
                cmd.Parameters.AddWithValue("source_account_id", NpgsqlDbType.Integer, interAccTransaction.source_account_id);
                cmd.Parameters.AddWithValue("target_account_id", NpgsqlDbType.Integer, interAccTransaction.target_account_id);
                cmd.Parameters.AddWithValue("source_amount", NpgsqlDbType.Numeric, Convert.ToDecimal(interAccTransaction.source_amount));
                cmd.Parameters.AddWithValue("source_cur_code", NpgsqlDbType.Varchar, interAccTransaction.source_currency_code);
                cmd.Parameters.AddWithValue("target_amount", NpgsqlDbType.Numeric, Convert.ToDecimal(interAccTransaction.target_amount));
                cmd.Parameters.AddWithValue("target_cur_code", NpgsqlDbType.Varchar, interAccTransaction.target_currency_code);
                cmd.Parameters.AddWithValue("row_version", NpgsqlDbType.Integer, interAccTransaction.row_version);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                DbConn.Instance.FreeConnection(conn);
            }

            return Redirect("/InteraccountTransaction/Index");
        }

        public IActionResult Delete(int id, [Bind("id", "row_version")] InteraccountTransaction interAccTransaction)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            using (conn)
            {
                string request = "call sb.delete_interaccount_transactions(pii_transaction_id => @id, " +
                                                                                    "pii_row_version => @row_version)";
                NpgsqlCommand cmd = new NpgsqlCommand(request, conn);
                cmd.Parameters.AddWithValue("id", NpgsqlDbType.Integer, id);
                cmd.Parameters.AddWithValue("row_version", NpgsqlDbType.Integer, interAccTransaction.row_version);
                cmd.ExecuteNonQuery();
                DbConn.Instance.FreeConnection(conn);
            }
            return Redirect("/InteraccountTransaction/Index");
        }

        private string GetCurrencyCodeForAccount(int accountId)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            string currencyCode;
            NpgsqlCommand selCmd = new NpgsqlCommand();
            selCmd.CommandText = "select currency_code from sb.accounts where id = @id";
            selCmd.Parameters.AddWithValue("id", NpgsqlDbType.Integer, accountId);
            selCmd.Connection = conn;
            currencyCode = (string)selCmd.ExecuteScalar();
            return currencyCode;
        }

        private decimal ConvertAmount(decimal _amount, string fromCurrency, string toCurrency, DateTime rateDate)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            decimal amount;
            NpgsqlCommand convCmd = new NpgsqlCommand();
            convCmd.CommandText = "select round(sb.convert_amount " +
                "                   (pni_amount => @amount, pvi_from_currency => @fromcurrency, " +
                "                    pvi_to_currency => @tocurrency, pdi_rate_date => @ratedate), 2)";
            convCmd.Parameters.AddWithValue("amount", NpgsqlDbType.Numeric, _amount);
            convCmd.Parameters.AddWithValue("fromcurrency", NpgsqlDbType.Varchar, fromCurrency);
            convCmd.Parameters.AddWithValue("tocurrency", NpgsqlDbType.Varchar, toCurrency);
            convCmd.Parameters.AddWithValue("ratedate", NpgsqlDbType.Date, rateDate);
            convCmd.Connection = conn;
            amount = (decimal)convCmd.ExecuteScalar();
            return amount;
        }
    }
}
