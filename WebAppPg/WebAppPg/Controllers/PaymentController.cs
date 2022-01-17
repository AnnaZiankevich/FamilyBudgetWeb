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
    public class PaymentController : Controller
    {
        static PaymentList paymList = new PaymentList { paymentList = new List<PaymentListItem>() };
        public IActionResult Index()
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            using (conn)
            {
                NpgsqlCommand cmd = new NpgsqlCommand("select a.id, a.name, a.planned_payment_id, b.name as planned_payment_name, " +
                                                        "a.payment_receiver_id, c.name as payment_receiver_name, a.payment_type_id, " +
                                                        "d.name as payment_type_name, a.amount, a.currency_code, a.payment_date, " +
                                                        "a.account_id, e.name as account_name, a.row_version from sb.payments a " +
                                                        "left join sb.planned_payments b on b.id = a.planned_payment_id " +
                                                        "join sb.payment_receivers c on c.id = a.payment_receiver_id " +
                                                        "join sb.payment_types d on d.id = a.payment_type_id " +
                                                        "left join sb.accounts e on e.id = a.account_id " +
                                                        "order by a.name", conn);
                paymList.paymentList.Clear();
                NpgsqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var payment = new PaymentListItem
                    {
                        id = rdr.GetInt32("id"),
                        name = rdr.GetString("name"),
                        planned_payment_id = (rdr.IsDBNull("planned_payment_id") ? -1 : rdr.GetInt32("planned_payment_id")),
                        planned_payment_name = (rdr.IsDBNull("planned_payment_id") ? "" : rdr.GetString("planned_payment_name")),
                        payment_receiver_id = rdr.GetInt32("payment_receiver_id"),
                        payment_receiver_name = rdr.GetString("payment_receiver_name"),
                        payment_type_id = rdr.GetInt32("payment_type_id"),
                        payment_type_name = rdr.GetString("payment_type_name"),
                        amount = rdr.GetDecimal("amount"),
                        currency_code = rdr.GetString("currency_code"),
                        payment_date = rdr.GetDateTime("payment_date"),
                        account_id = rdr.GetInt32("account_id"),
                        account_name = rdr.GetString("account_name"),
                        row_version = rdr.GetInt32("row_version")
                    };

                    paymList.paymentList.Add(payment);
                }
                DbConn.Instance.FreeConnection(conn);
            }
            return View(paymList);
        }

        [HttpGet]
        public IActionResult Add()
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            Payment payment = new Payment();
            payment.plannedPaymentsList = PlannedPaymentDAO.GetPlannedPaymentList(conn);
            payment.planned_payment_id = -1;
            payment.paymentReceiversList = PaymentReceiverDAO.GetPaymentReceiverList(conn);
            payment.paymentTypesList = PaymentTypeDAO.GetPaymentTypeList(conn);
            payment.accountList = AccountDAO.GetAccountList(conn);
            payment.currencyCodesList = CurrencyCodeDAO.GetCurrCodesList(conn);
            DbConn.Instance.FreeConnection(conn);
            return View(payment);
        }

        [HttpPost]
        public IActionResult Add([Bind("name", "account_id", "payment_receiver_id", "payment_type_id", "amount", "currency_code", "payment_date", "row_version", "planned_payment_id")] Payment payment)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));

            using (conn)
            {
                string request = "select sb.modify_payments (pii_id => @id, pvi_name => @name, " +
                                                    "pii_account_id => @account_id, " +
                                                    "pii_payment_receiver_id => @payment_receiver_id, " +
                                                    "pii_payment_type_id => @payment_type_id, " +
                                                    "pni_amount => @amount, " +
                                                    "pvi_currency_code => @currency_code, " +
                                                    "pdi_payment_date => @payment_date, " +
                                                    "pii_row_version => @row_version, " +
                                                    "pii_planned_payment_id => @planned_payment_id)";
                NpgsqlCommand cmd = new NpgsqlCommand(request, conn);
                //SetCommandType(cmd, CommandType.Text);
                cmd.Parameters.AddWithValue("id", NpgsqlDbType.Integer, DBNull.Value);
                cmd.Parameters.AddWithValue("name", NpgsqlDbType.Varchar, payment.name);
                cmd.Parameters.AddWithValue("payment_receiver_id", NpgsqlDbType.Integer, payment.payment_receiver_id);
                cmd.Parameters.AddWithValue("payment_type_id", NpgsqlDbType.Integer, payment.payment_type_id);
                cmd.Parameters.AddWithValue("amount", NpgsqlDbType.Numeric, Convert.ToDecimal(payment.amount));
                cmd.Parameters.AddWithValue("currency_code", NpgsqlDbType.Varchar, payment.currency_code);
                cmd.Parameters.AddWithValue("payment_date", NpgsqlDbType.Date, payment.payment_date);
                cmd.Parameters.AddWithValue("row_version", NpgsqlDbType.Integer, payment.row_version);
                cmd.Parameters.AddWithValue("account_id", NpgsqlDbType.Integer, payment.account_id);
                if (payment.planned_payment_id == -1)
                    cmd.Parameters.AddWithValue("planned_payment_id", NpgsqlDbType.Integer, DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("planned_payment_id", NpgsqlDbType.Integer, payment.planned_payment_id);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                DbConn.Instance.FreeConnection(conn);
            }

            return Redirect("/Payment/Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            Payment payment = PaymentDAO.FindById(id);
            payment.plannedPaymentsList = PlannedPaymentDAO.GetPlannedPaymentList(conn);
            payment.paymentReceiversList = PaymentReceiverDAO.GetPaymentReceiverList(conn);
            payment.paymentTypesList = PaymentTypeDAO.GetPaymentTypeList(conn);
            payment.accountList = AccountDAO.GetAccountList(conn);
            payment.currencyCodesList = CurrencyCodeDAO.GetCurrCodesList(conn);
            DbConn.Instance.FreeConnection(conn);
            return View(payment);
        }

        [HttpPost]
        public IActionResult Edit(int id, [Bind("name", "account_id", "payment_receiver_id", "payment_type_id", "amount", "currency_code", "payment_date", "row_version", "planned_payment_id")] Payment payment)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));

            using (conn)
            {
                string request = "select sb.modify_payments (pii_id => @id, pvi_name => @name, " +
                                                    "pii_account_id => @account_id, " +
                                                    "pii_payment_receiver_id => @payment_receiver_id, " +
                                                    "pii_payment_type_id => @payment_type_id, " +
                                                    "pni_amount => @amount, " +
                                                    "pvi_currency_code => @currency_code, " +
                                                    "pdi_payment_date => @payment_date, " +
                                                    "pii_row_version => @row_version, " +
                                                    "pii_planned_payment_id => @planned_payment_id)";
                NpgsqlCommand cmd = new NpgsqlCommand(request, conn);
                //SetCommandType(cmd, CommandType.Text);
                cmd.Parameters.AddWithValue("id", NpgsqlDbType.Integer, id);
                cmd.Parameters.AddWithValue("name", NpgsqlDbType.Varchar, payment.name);
                cmd.Parameters.AddWithValue("payment_receiver_id", NpgsqlDbType.Integer, payment.payment_receiver_id);
                cmd.Parameters.AddWithValue("payment_type_id", NpgsqlDbType.Integer, payment.payment_type_id);
                cmd.Parameters.AddWithValue("amount", NpgsqlDbType.Numeric, Convert.ToDecimal(payment.amount));
                cmd.Parameters.AddWithValue("currency_code", NpgsqlDbType.Varchar, payment.currency_code);
                cmd.Parameters.AddWithValue("payment_date", NpgsqlDbType.Date, payment.payment_date);
                cmd.Parameters.AddWithValue("row_version", NpgsqlDbType.Integer, payment.row_version);
                cmd.Parameters.AddWithValue("account_id", NpgsqlDbType.Integer, payment.account_id);
                if (payment.planned_payment_id == -1)
                    cmd.Parameters.AddWithValue("planned_payment_id", NpgsqlDbType.Integer, DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("planned_payment_id", NpgsqlDbType.Integer, payment.planned_payment_id);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                DbConn.Instance.FreeConnection(conn);
            }

            return Redirect("/Payment/Index");
        }

        public IActionResult Delete(int id, [Bind("id", "row_version")] Payment payment)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            using (conn)
            {
                string request = "call sb.delete_payment(pii_payment_id => @id, pii_row_version => @row_version)";
                NpgsqlCommand cmd = new NpgsqlCommand(request, conn);
                cmd.Parameters.AddWithValue("id", NpgsqlDbType.Integer, id);
                cmd.Parameters.AddWithValue("row_version", NpgsqlDbType.Integer, payment.row_version);
                cmd.ExecuteNonQuery();
                DbConn.Instance.FreeConnection(conn);
            }
            return Redirect("/Payment/Index");
        }
    }
}
