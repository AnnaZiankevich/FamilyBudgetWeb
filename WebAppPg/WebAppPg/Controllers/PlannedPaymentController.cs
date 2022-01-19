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
    public class PlannedPaymentController : Controller
    {
        static PlannedPaymentList plnPaymList = new PlannedPaymentList { plannedPaymentList = new List<PlannedPaymentListItem>() };

        [HttpGet]
        public IActionResult Index()
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            using (conn)
            {
                NpgsqlCommand cmd = new NpgsqlCommand("select a.id, a.name, a.payment_receiver_id, " +
                                                        "b.name as payment_receiver_name, a.payment_type_id, " +
                                                        "c.name as payment_type_name, a.amount, a.currency_code, " +
                                                        "a.planned_date, a.start_date, a.end_date, a.account_id, " +
                                                        "d.name as account_name, a.row_version from sb.planned_payments a " +
                                                        "join sb.payment_receivers b on b.id = a.payment_receiver_id " +
                                                        "join sb.payment_types c on c.id = a.payment_type_id " +
                                                        "left join sb.accounts d on d.id = a.account_id " +
                                                        "order by a.name", conn);
                plnPaymList.plannedPaymentList.Clear();
                NpgsqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var plnPayment = new PlannedPaymentListItem
                    {
                        id = rdr.GetInt32("id"),
                        name = rdr.GetString("name"),
                        payment_receiver_id = rdr.GetInt32("payment_receiver_id"),
                        payment_receiver_name = rdr.GetString("payment_receiver_name"),
                        payment_type_id = rdr.GetInt32("payment_type_id"),
                        payment_type_name = rdr.GetString("payment_type_name"),
                        amount = rdr.GetDecimal("amount"),
                        currency_code = rdr.GetString("currency_code"),
                        planned_date = rdr.IsDBNull("planned_date") ? null : rdr.GetDateTime("planned_date"),
                        start_date = rdr.IsDBNull("start_date") ? null : rdr.GetDateTime("start_date"),
                        end_date = rdr.IsDBNull("end_date") ? null : rdr.GetDateTime("end_date"),
                        account_id = rdr.GetInt32("account_id"),
                        account_name = rdr.GetString("account_name"),
                        row_version = rdr.GetInt32("row_version")
                    };

                    plnPaymList.plannedPaymentList.Add(plnPayment);
                }
                DbConn.Instance.FreeConnection(conn);
            }
            return View(plnPaymList);
        }

        [HttpGet]
        public IActionResult Add()
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            PlannedPayment plnPaym = new PlannedPayment();
            plnPaym.paymentReceiversList = PaymentReceiverDAO.GetPaymentReceiverList(conn);
            plnPaym.paymentTypesList = PaymentTypeDAO.GetPaymentTypeList(conn);
            plnPaym.accountList = AccountDAO.GetAccountList(conn);
            plnPaym.currencyCodesList = CurrencyCodeDAO.GetCurrCodesList(conn);
            DbConn.Instance.FreeConnection(conn);
            return View(plnPaym);
        }

        [HttpPost]
        public IActionResult Add([Bind("name", "payment_receiver_id", "payment_type_id", "amount", "currency_code", "planned_date", "start_date", "end_date", "row_version", "account_id")] PlannedPayment plnPayment)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));

            using (conn)
            {
                string request = "select sb.modify_planned_payments (pii_id => @id, pvi_name => @name, " +
                                                    "pii_payment_receiver_id => @payment_receiver_id, " +
                                                    "pii_payment_type_id => @payment_type_id, " +
                                                    "pni_amount => @amount, " +
                                                    "pvi_currency_code => @currency_code, " +
                                                    "pii_row_version => @row_version, " +
                                                    "pii_account_id => @account_id, " +
                                                    "pdi_planned_date => @planned_date, " +
                                                    "pdi_start_date => @start_date, " +
                                                    "pdi_end_date => @end_date)";
                NpgsqlCommand cmd = new NpgsqlCommand(request, conn);
                //SetCommandType(cmd, CommandType.Text);
                cmd.Parameters.AddWithValue("id", NpgsqlDbType.Integer, DBNull.Value);
                cmd.Parameters.AddWithValue("name", NpgsqlDbType.Varchar, plnPayment.name);
                cmd.Parameters.AddWithValue("payment_receiver_id", NpgsqlDbType.Integer, plnPayment.payment_receiver_id);
                cmd.Parameters.AddWithValue("payment_type_id", NpgsqlDbType.Integer, plnPayment.payment_type_id);
                cmd.Parameters.AddWithValue("amount", NpgsqlDbType.Numeric, Convert.ToDecimal(plnPayment.amount));
                cmd.Parameters.AddWithValue("currency_code", NpgsqlDbType.Varchar, plnPayment.currency_code);
                cmd.Parameters.AddWithValue("row_version", NpgsqlDbType.Integer, plnPayment.row_version);
                cmd.Parameters.AddWithValue("account_id", NpgsqlDbType.Integer, plnPayment.account_id);
                if (plnPayment.planned_date == null)
                    cmd.Parameters.AddWithValue("planned_date", NpgsqlDbType.Date, DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("planned_date", NpgsqlDbType.Date, plnPayment.planned_date);

                if (plnPayment.start_date == null)
                    cmd.Parameters.AddWithValue("start_date", NpgsqlDbType.Date, DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("start_date", NpgsqlDbType.Date, plnPayment.start_date);

                if (plnPayment.end_date == null)
                    cmd.Parameters.AddWithValue("end_date", NpgsqlDbType.Date, DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("end_date", NpgsqlDbType.Date, plnPayment.end_date);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                DbConn.Instance.FreeConnection(conn);
            }

            return Redirect("/PlannedPayment/Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            PlannedPayment plnPaym = PlannedPaymentDAO.FindById(id);
            plnPaym.paymentReceiversList = PaymentReceiverDAO.GetPaymentReceiverList(conn);
            plnPaym.paymentTypesList = PaymentTypeDAO.GetPaymentTypeList(conn);
            plnPaym.accountList = AccountDAO.GetAccountList(conn);
            plnPaym.currencyCodesList = CurrencyCodeDAO.GetCurrCodesList(conn);
            DbConn.Instance.FreeConnection(conn);
            return View(plnPaym);
        }

        [HttpPost]
        public IActionResult Edit(int id, [Bind("name", "payment_receiver_id", "payment_type_id", "amount", "currency_code", "planned_date", "start_date", "end_date", "row_version", "account_id")] PlannedPayment plnPayment)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));

            using (conn)
            {
                string request = "select sb.modify_planned_payments (pii_id => @id, pvi_name => @name, " +
                                                    "pii_payment_receiver_id => @payment_receiver_id, " +
                                                    "pii_payment_type_id => @payment_type_id, " +
                                                    "pni_amount => @amount, " +
                                                    "pvi_currency_code => @currency_code, " +
                                                    "pii_row_version => @row_version, " +
                                                    "pii_account_id => @account_id, " +
                                                    "pdi_planned_date => @planned_date, " +
                                                    "pdi_start_date => @start_date, " +
                                                    "pdi_end_date => @end_date)";
                NpgsqlCommand cmd = new NpgsqlCommand(request, conn);
                //SetCommandType(cmd, CommandType.Text);
                cmd.Parameters.AddWithValue("id", NpgsqlDbType.Integer, id);
                cmd.Parameters.AddWithValue("name", NpgsqlDbType.Varchar, plnPayment.name);
                cmd.Parameters.AddWithValue("payment_receiver_id", NpgsqlDbType.Integer, plnPayment.payment_receiver_id);
                cmd.Parameters.AddWithValue("payment_type_id", NpgsqlDbType.Integer, plnPayment.payment_type_id);
                cmd.Parameters.AddWithValue("amount", NpgsqlDbType.Numeric, Convert.ToDecimal(plnPayment.amount));
                cmd.Parameters.AddWithValue("currency_code", NpgsqlDbType.Varchar, plnPayment.currency_code);
                cmd.Parameters.AddWithValue("row_version", NpgsqlDbType.Integer, plnPayment.row_version);
                cmd.Parameters.AddWithValue("account_id", NpgsqlDbType.Integer, plnPayment.account_id);
                if (plnPayment.planned_date == null)
                    cmd.Parameters.AddWithValue("planned_date", NpgsqlDbType.Date, DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("planned_date", NpgsqlDbType.Date, plnPayment.planned_date);

                if (plnPayment.start_date == null)
                    cmd.Parameters.AddWithValue("start_date", NpgsqlDbType.Date, DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("start_date", NpgsqlDbType.Date, plnPayment.start_date);

                if (plnPayment.end_date == null)
                    cmd.Parameters.AddWithValue("end_date", NpgsqlDbType.Date, DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("end_date", NpgsqlDbType.Date, plnPayment.end_date);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                DbConn.Instance.FreeConnection(conn);
            }

            return Redirect("/PlannedPayment/Index");
        }

        public IActionResult Delete(int id, [Bind("id", "row_version")] PlannedPayment plnPaym)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            using (conn)
            {
                string request = "call sb.delete_planned_payments(pii_planned_payments_id => @id, pii_row_version => @row_version)";
                NpgsqlCommand cmd = new NpgsqlCommand(request, conn);
                cmd.Parameters.AddWithValue("id", NpgsqlDbType.Integer, id);
                cmd.Parameters.AddWithValue("row_version", NpgsqlDbType.Integer, plnPaym.row_version);
                cmd.ExecuteNonQuery();
                DbConn.Instance.FreeConnection(conn);
            }
            return Redirect("/PlannedPayment/Index");
        }
    }
}
