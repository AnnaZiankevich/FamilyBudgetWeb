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
    public class IncomeController : Controller
    {
        static IncomeList incList = new IncomeList { incomeList = new List<IncomeListItem>() };

        [HttpGet]
        public IActionResult Index()
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            using (conn)
            {
                NpgsqlCommand cmd = new NpgsqlCommand("select a.id, a.name, a.planned_income_id, b.name as planned_income_name, " +
                                                        "a.income_source_id, c.name as income_source_name, a.income_type_id, " +
                                                        "d.name as income_type_name, a.amount, a.currency_code, a.income_date, " +
                                                        "a.account_id, e.name as account_name, a.row_version from sb.income a " +
                                                        "left join sb.planned_income b on b.id = a.planned_income_id " +
                                                        "join sb.income_sources c on c.id = a.income_source_id " +
                                                        "join sb.income_types d on d.id = a.income_type_id " +
                                                        "left join sb.accounts e on e.id = a.account_id " +
                                                        "order by a.name", conn);
                incList.incomeList.Clear();
                NpgsqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var income = new IncomeListItem
                    {
                        id = rdr.GetInt32("id"),
                        name = rdr.GetString("name"),
                        planned_income_id = (rdr.IsDBNull("planned_income_id") ? -1 : rdr.GetInt32("planned_income_id")),
                        planned_income_name = (rdr.IsDBNull("planned_income_id") ? "" : rdr.GetString("planned_income_name")),
                        income_source_id = rdr.GetInt32("income_source_id"),
                        income_source_name = rdr.GetString("income_source_name"),
                        income_type_id = rdr.GetInt32("income_type_id"),
                        income_type_name = rdr.GetString("income_type_name"),
                        amount = rdr.GetDecimal("amount"),
                        currency_code = rdr.GetString("currency_code"),
                        income_date = rdr.GetDateTime("income_date"),
                        account_id = rdr.GetInt32("account_id"),
                        account_name = rdr.GetString("account_name"),
                        row_version = rdr.GetInt32("row_version")
                    };

                    incList.incomeList.Add(income);
                }
                DbConn.Instance.FreeConnection(conn);
            }
            return View(incList);
        }

        [HttpGet]
        public IActionResult Add()
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            Income income = new Income();
            income.plannedIncomesList = PlannedIncomeDAO.GetPlannedIncomeList(conn);
            income.planned_income_id = -1;
            income.incomeSourcesList = IncomeSourceDAO.GetIncomeSourceList(conn);
            income.incomeTypesList = IncomeTypeDAO.GetIncomeTypeList(conn);
            income.accountList = AccountDAO.GetAccountList(conn);
            income.currencyCodesList = CurrencyCodeDAO.GetCurrCodesList(conn);
            DbConn.Instance.FreeConnection(conn);
            return View(income);
        }

        [HttpPost]
        public IActionResult Add([Bind("name", "income_source_id", "income_type_id", "amount", "currency_code", "income_date", "row_version", "account_id", "planned_income_id")] Income income)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));

            using (conn)
            {
                string request = "select sb.modify_income (pii_id => @id, pvi_name => @name, " +                                       
                                                    "pii_income_source_id => @income_source_id, " +
                                                    "pii_income_type_id => @income_type_id, " +
                                                    "pni_amount => @amount, " +
                                                    "pvi_currency_code => @currency_code, " +
                                                    "pdi_income_date => @income_date, " +
                                                    "pii_row_version => @row_version, " +
                                                    "pii_account_id => @account_id, " +
                                                    "pii_planned_income_id => @planned_income_id)";
                NpgsqlCommand cmd = new NpgsqlCommand(request, conn);
                //SetCommandType(cmd, CommandType.Text);
                cmd.Parameters.AddWithValue("id", NpgsqlDbType.Integer, DBNull.Value);
                cmd.Parameters.AddWithValue("name", NpgsqlDbType.Varchar, income.name);                
                cmd.Parameters.AddWithValue("income_source_id", NpgsqlDbType.Integer, income.income_source_id);
                cmd.Parameters.AddWithValue("income_type_id", NpgsqlDbType.Integer, income.income_type_id);
                cmd.Parameters.AddWithValue("amount", NpgsqlDbType.Numeric, Convert.ToDecimal(income.amount));
                cmd.Parameters.AddWithValue("currency_code", NpgsqlDbType.Varchar, income.currency_code);
                cmd.Parameters.AddWithValue("income_date", NpgsqlDbType.Date, income.income_date);
                cmd.Parameters.AddWithValue("row_version", NpgsqlDbType.Integer, income.row_version);
                cmd.Parameters.AddWithValue("account_id", NpgsqlDbType.Integer, income.account_id);
                if (income.planned_income_id == -1)
                  cmd.Parameters.AddWithValue("planned_income_id", NpgsqlDbType.Integer, DBNull.Value);
                else
                  cmd.Parameters.AddWithValue("planned_income_id", NpgsqlDbType.Integer, income.planned_income_id);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                DbConn.Instance.FreeConnection(conn);
            }

            return Redirect("/Income/Index");
        }

        [HttpGet]
        public IActionResult AddFromPlannedIncomeFirstStage()
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            Income income = new Income();
            income.plannedIncomesList = PlannedIncomeDAO.GetPlannedIncomeListForPeriod(conn);
            DbConn.Instance.FreeConnection(conn);
            return View(income);
        }
        //[Bind("planned_income_id")]
        [HttpPost]
        public IActionResult AddFromPlannedIncomeFirstStage(int planned_income_id)
        {
            return RedirectToAction("AddFromPlannedIncomeSecondStage",
                       new
                       {
                           id = planned_income_id
                       });
        }

        [HttpGet]
        public IActionResult AddFromPlannedIncomeSecondStage(int id) //string name, int income_source_id, int income_type_id, decimal amount, string currency_code, DateTime income_date, int account_id
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            PlannedIncome plannedIncome = PlannedIncomeDAO.FindById(id);
            Income income = new Income();
            income.plannedIncomesList = PlannedIncomeDAO.GetPlannedIncomeListForPeriod(conn);
            income.planned_income_id = plannedIncome.id;
            income.planned_income_name = plannedIncome.name;
            income.name = plannedIncome.name;
            income.incomeSourcesList = IncomeSourceDAO.GetIncomeSourceList(conn);
            income.income_source_id = plannedIncome.income_source_id;
            income.incomeTypesList = IncomeTypeDAO.GetIncomeTypeList(conn);
            income.income_type_id = plannedIncome.income_type_id;
            income.amount = plannedIncome.amount;
            income.currencyCodesList = CurrencyCodeDAO.GetCurrCodesList(conn);
            income.currency_code = plannedIncome.currency_code;
            income.income_date = (DateTime)plannedIncome.planned_date;
            income.accountList = AccountDAO.GetAccountList(conn);
            income.account_id = plannedIncome.account_id;
            DbConn.Instance.FreeConnection(conn);
            return View(income);
        }

        [HttpPost]
        public IActionResult AddFromPlannedIncomeSecondStage([Bind("name", "income_source_id", "income_type_id", "amount", "currency_code", "income_date", "row_version", "account_id", "planned_income_id")] Income income)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));

            using (conn)
            {
                string request = "select sb.modify_income (pii_id => @id, pvi_name => @name, " +
                                                    "pii_income_source_id => @income_source_id, " +
                                                    "pii_income_type_id => @income_type_id, " +
                                                    "pni_amount => @amount, " +
                                                    "pvi_currency_code => @currency_code, " +
                                                    "pdi_income_date => @income_date, " +
                                                    "pii_row_version => @row_version, " +
                                                    "pii_account_id => @account_id, " +
                                                    "pii_planned_income_id => @planned_income_id)";
                NpgsqlCommand cmd = new NpgsqlCommand(request, conn);
                //SetCommandType(cmd, CommandType.Text);
                cmd.Parameters.AddWithValue("id", NpgsqlDbType.Integer, DBNull.Value);
                cmd.Parameters.AddWithValue("name", NpgsqlDbType.Varchar, income.name);
                cmd.Parameters.AddWithValue("income_source_id", NpgsqlDbType.Integer, income.income_source_id);
                cmd.Parameters.AddWithValue("income_type_id", NpgsqlDbType.Integer, income.income_type_id);
                cmd.Parameters.AddWithValue("amount", NpgsqlDbType.Numeric, Convert.ToDecimal(income.amount));
                cmd.Parameters.AddWithValue("currency_code", NpgsqlDbType.Varchar, income.currency_code);
                cmd.Parameters.AddWithValue("income_date", NpgsqlDbType.Date, income.income_date);
                cmd.Parameters.AddWithValue("row_version", NpgsqlDbType.Integer, income.row_version);
                cmd.Parameters.AddWithValue("account_id", NpgsqlDbType.Integer, income.account_id);
                cmd.Parameters.AddWithValue("planned_income_id", NpgsqlDbType.Integer, income.planned_income_id);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                DbConn.Instance.FreeConnection(conn);
            }

            return Redirect("/Income/Index");
        } 

        [HttpGet]
        public IActionResult Edit(int id)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            Income income = IncomeDAO.FindById(id);
            income.plannedIncomesList = PlannedIncomeDAO.GetPlannedIncomeList(conn);
            income.incomeSourcesList = IncomeSourceDAO.GetIncomeSourceList(conn);
            income.incomeTypesList = IncomeTypeDAO.GetIncomeTypeList(conn);
            income.accountList = AccountDAO.GetAccountList(conn);
            income.currencyCodesList = CurrencyCodeDAO.GetCurrCodesList(conn);
            DbConn.Instance.FreeConnection(conn);
            return View(income);
        }

        [HttpPost]
        public IActionResult Edit(int id, [Bind("name", "income_source_id", "income_type_id", "amount", "currency_code", "income_date", "row_version", "account_id", "planned_income_id")] Income income)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));

            using (conn)
            {
                string request = "select sb.modify_income (pii_id => @id, pvi_name => @name, " +
                                                    "pii_income_source_id => @income_source_id, " +
                                                    "pii_income_type_id => @income_type_id, " +
                                                    "pni_amount => @amount, " +
                                                    "pvi_currency_code => @currency_code, " +
                                                    "pdi_income_date => @income_date, " +
                                                    "pii_row_version => @row_version, " +
                                                    "pii_account_id => @account_id, " +
                                                    "pii_planned_income_id => @planned_income_id)";
                NpgsqlCommand cmd = new NpgsqlCommand(request, conn);
                //SetCommandType(cmd, CommandType.Text);
                cmd.Parameters.AddWithValue("id", NpgsqlDbType.Integer, id);
                cmd.Parameters.AddWithValue("name", NpgsqlDbType.Varchar, income.name);
                cmd.Parameters.AddWithValue("income_source_id", NpgsqlDbType.Integer, income.income_source_id);
                cmd.Parameters.AddWithValue("income_type_id", NpgsqlDbType.Integer, income.income_type_id);
                cmd.Parameters.AddWithValue("amount", NpgsqlDbType.Numeric, Convert.ToDecimal(income.amount));
                cmd.Parameters.AddWithValue("currency_code", NpgsqlDbType.Varchar, income.currency_code);
                cmd.Parameters.AddWithValue("income_date", NpgsqlDbType.Date, income.income_date);
                cmd.Parameters.AddWithValue("row_version", NpgsqlDbType.Integer, income.row_version);
                if (income.account_id == -1)
                    cmd.Parameters.AddWithValue("account_id", NpgsqlDbType.Integer, DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("account_id", NpgsqlDbType.Integer, income.account_id);
                if (income.planned_income_id == -1)
                    cmd.Parameters.AddWithValue("planned_income_id", NpgsqlDbType.Integer, DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("planned_income_id", NpgsqlDbType.Integer, income.planned_income_id);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                DbConn.Instance.FreeConnection(conn);
            }

            return Redirect("/Income/Index");
        }

        public IActionResult Delete(int id, [Bind("id", "row_version")] Income income)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            using (conn)
            {
                string request = "call sb.income_delete(pii_income_id => @id, pii_row_version => @row_version)";
                NpgsqlCommand cmd = new NpgsqlCommand(request, conn);
                cmd.Parameters.AddWithValue("id", NpgsqlDbType.Integer, id);
                cmd.Parameters.AddWithValue("row_version", NpgsqlDbType.Integer, income.row_version);
                cmd.ExecuteNonQuery();
                DbConn.Instance.FreeConnection(conn);
            }
            return Redirect("/Income/Index");
        }
    }
}
