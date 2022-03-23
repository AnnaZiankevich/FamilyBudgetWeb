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
    public class PlannedIncomeController : Controller
    {
        static PlannedIncomeList plnIncList = new PlannedIncomeList { plannedIncomeList = new List<PlannedIncomeListItem>() };

        [HttpGet]
        public IActionResult Index(int? page)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            using (conn)
            {
                NpgsqlCommand cmd = new NpgsqlCommand("select a.id, a.name, a.income_source_id, " +
                                                        "b.name as income_source_name, a.income_type_id, " +
                                                        "c.name as income_type_name, a.amount, a.currency_code, " +
                                                        "a.planned_date, a.start_date, a.end_date, a.account_id, " +
                                                        "d.name as account_name, a.row_version from sb.planned_income a " +
                                                        "join sb.income_sources b on b.id = a.income_source_id " +
                                                        "join sb.income_types c on c.id = a.income_type_id " +
                                                        "left join sb.accounts d on d.id = a.account_id " +
                                                        "order by a.name", conn);
                plnIncList.plannedIncomeList.Clear();
                NpgsqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var plnIncome = new PlannedIncomeListItem
                    {
                        id = rdr.GetInt32("id"),
                        name = rdr.GetString("name"),
                        income_source_id = rdr.GetInt32("income_source_id"),
                        income_source_name = rdr.GetString("income_source_name"),
                        income_type_id = rdr.GetInt32("income_type_id"),
                        income_type_name = rdr.GetString("income_type_name"),
                        amount = rdr.GetDecimal("amount"),
                        currency_code = rdr.GetString("currency_code"),
                        planned_date = rdr.IsDBNull("planned_date") ? null : rdr.GetDateTime("planned_date"),
                        start_date = rdr.IsDBNull("start_date") ? null : rdr.GetDateTime("start_date"),
                        end_date = rdr.IsDBNull("end_date") ? null : rdr.GetDateTime("end_date"),
                        account_id = rdr.GetInt32("account_id"),
                        account_name = rdr.GetString("account_name"),
                        row_version = rdr.GetInt32("row_version")
                    };

                    plnIncList.plannedIncomeList.Add(plnIncome);
                }
                DbConn.Instance.FreeConnection(conn);
            }

            return View(plnIncList);
        }

        [HttpGet]
        public IActionResult Add()
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            PlannedIncome plnIncome = new PlannedIncome();
            plnIncome.incomeSoursesList = IncomeSourceDAO.GetIncomeSourceList(conn);
            plnIncome.incomeTypesList = IncomeTypeDAO.GetIncomeTypeList(conn);
            plnIncome.accountList = AccountDAO.GetAccountList(conn);
            plnIncome.currencyCodesList = CurrencyCodeDAO.GetCurrCodesList(conn);
            DbConn.Instance.FreeConnection(conn);
            return View(plnIncome);
        }

        [HttpPost]
        public IActionResult Add([Bind("name", "income_source_id", "income_type_id", "amount", "currency_code", "planned_date", "start_date", "end_date", "row_version", "account_id")] PlannedIncome plnIncome)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));

            using (conn)
            {
                string request = "select sb.modify_planned_income (pii_id => @id, pvi_name => @name, " +
                                                    "pii_income_source_id => @income_source_id, " +
                                                    "pii_income_type_id => @income_type_id, " +
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
                cmd.Parameters.AddWithValue("name", NpgsqlDbType.Varchar, plnIncome.name);
                cmd.Parameters.AddWithValue("income_source_id", NpgsqlDbType.Integer, plnIncome.income_source_id);
                cmd.Parameters.AddWithValue("income_type_id", NpgsqlDbType.Integer, plnIncome.income_type_id);
                cmd.Parameters.AddWithValue("amount", NpgsqlDbType.Numeric, Convert.ToDecimal(plnIncome.amount));
                cmd.Parameters.AddWithValue("currency_code", NpgsqlDbType.Varchar, plnIncome.currency_code);
                cmd.Parameters.AddWithValue("row_version", NpgsqlDbType.Integer, plnIncome.row_version);
                cmd.Parameters.AddWithValue("account_id", NpgsqlDbType.Integer, plnIncome.account_id);
                if (plnIncome.planned_date == null)
                    cmd.Parameters.AddWithValue("planned_date", NpgsqlDbType.Date, DBNull.Value);
                else
                  cmd.Parameters.AddWithValue("planned_date", NpgsqlDbType.Date, plnIncome.planned_date);

                if (plnIncome.start_date == null)
                    cmd.Parameters.AddWithValue("start_date", NpgsqlDbType.Date, DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("start_date", NpgsqlDbType.Date, plnIncome.start_date);

                if (plnIncome.end_date == null)
                    cmd.Parameters.AddWithValue("end_date", NpgsqlDbType.Date, DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("end_date", NpgsqlDbType.Date, plnIncome.end_date);               
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                DbConn.Instance.FreeConnection(conn);
            }

            return Redirect("/PlannedIncome/Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            PlannedIncome plnIncome = PlannedIncomeDAO.FindById(id);
            plnIncome.incomeSoursesList = IncomeSourceDAO.GetIncomeSourceList(conn);
            plnIncome.incomeTypesList = IncomeTypeDAO.GetIncomeTypeList(conn);
            plnIncome.accountList = AccountDAO.GetAccountList(conn);
            plnIncome.currencyCodesList = CurrencyCodeDAO.GetCurrCodesList(conn);
            DbConn.Instance.FreeConnection(conn);
            return View(plnIncome);
        }

        [HttpPost]
        public IActionResult Edit(int id, [Bind("name", "income_source_id", "income_type_id", "amount", "currency_code", "planned_date", "start_date", "end_date", "row_version", "account_id")] PlannedIncome plnIncome)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));

            using (conn)
            {
                string request = "select sb.modify_planned_income (pii_id => @id, pvi_name => @name, " +
                                                    "pii_income_source_id => @income_source_id, " +
                                                    "pii_income_type_id => @income_type_id, " +
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
                cmd.Parameters.AddWithValue("name", NpgsqlDbType.Varchar, plnIncome.name);
                cmd.Parameters.AddWithValue("income_source_id", NpgsqlDbType.Integer, plnIncome.income_source_id);
                cmd.Parameters.AddWithValue("income_type_id", NpgsqlDbType.Integer, plnIncome.income_type_id);
                cmd.Parameters.AddWithValue("amount", NpgsqlDbType.Numeric, Convert.ToDecimal(plnIncome.amount));
                cmd.Parameters.AddWithValue("currency_code", NpgsqlDbType.Varchar, plnIncome.currency_code);
                cmd.Parameters.AddWithValue("row_version", NpgsqlDbType.Integer, plnIncome.row_version);
                cmd.Parameters.AddWithValue("account_id", NpgsqlDbType.Integer, plnIncome.account_id);
                if (plnIncome.planned_date == null)
                    cmd.Parameters.AddWithValue("planned_date", NpgsqlDbType.Date, DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("planned_date", NpgsqlDbType.Date, plnIncome.planned_date);

                if (plnIncome.start_date == null)
                    cmd.Parameters.AddWithValue("start_date", NpgsqlDbType.Date, DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("start_date", NpgsqlDbType.Date, plnIncome.start_date);

                if (plnIncome.end_date == null)
                    cmd.Parameters.AddWithValue("end_date", NpgsqlDbType.Date, DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("end_date", NpgsqlDbType.Date, plnIncome.end_date);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                DbConn.Instance.FreeConnection(conn);
            }

            return Redirect("/PlannedIncome/Index");
        }

        public IActionResult Delete(int id, [Bind("id", "row_version")] PlannedIncome plnInc)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            using (conn)
            {
                string request = "call sb.delete_planned_income(pii_planned_income_id => @id, pii_row_version => @row_version)";
                NpgsqlCommand cmd = new NpgsqlCommand(request, conn);
                cmd.Parameters.AddWithValue("id", NpgsqlDbType.Integer, id);
                cmd.Parameters.AddWithValue("row_version", NpgsqlDbType.Integer, plnInc.row_version);
                cmd.ExecuteNonQuery();
                DbConn.Instance.FreeConnection(conn);
            }
            return Redirect("/PlannedIncome/Index");
        }
    }
}
