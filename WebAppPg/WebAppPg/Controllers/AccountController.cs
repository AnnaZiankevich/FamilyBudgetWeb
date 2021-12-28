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

        //static List<Account> accountsList = new List<Account>();
        static AccountList accountList = new AccountList { accountList = new List<AccountListItem>(), accountOwnerId = -1};

        [HttpGet]
        public IActionResult List([Bind("accOwnerId")] int accOwnerid)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            using (conn)
            {
                NpgsqlCommand cmd = new NpgsqlCommand("select a.id, a.name, a.account_type_code, a.is_active, a.currency_code, " +
                                                            " b.name as account_owner_name, c.name as currency_name, d.name as account_type_name " +
                                                        " from sb.accounts a, sb.account_owners b, sb.currencies c, sb.account_types d " +
                                                        " where a.account_owner_id = @accOwnerId and b.id = a.account_owner_id and c.code = a.currency_code and d.code = a.account_type_code " +
                                                        " order by a.name", conn);
                cmd.Parameters.AddWithValue("accOwnerId", NpgsqlDbType.Integer, accOwnerid);
                accountList.accountList.Clear();
                accountList.accountOwnerId = accOwnerid;
                NpgsqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var account = new AccountListItem
                    {
                        id = rdr.GetInt32("id"),
                        name = rdr.GetString("name"),
                        account_type_code = rdr.GetString("account_type_code"),
                        account_type_name = rdr.GetString("account_type_name"),
                        account_owner_id = accOwnerid,
                        account_owner_name = rdr.GetString("account_owner_name"),
                        is_active = rdr.GetBoolean("is_active"),
                        currency_code = rdr.GetString("currency_code"),
                        currency_name = rdr.GetString("currency_name")
                    };

                    accountList.accountList.Add(account);
                    //Read(account, rdr);
                }
                DbConn.Instance.FreeConnection(conn);
            }
            //return View(accountsList);
            return View(accountList);
        }

        [HttpGet]
        public IActionResult Add([Bind("accOwnerId")] int accOwnerid)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            Account account = new Account();
            account.accountTypesList = AccTypeDAO.GetAccTypesList(conn);
            account.account_owner_id = accOwnerid;
            account.currencyCodesList = CurrencyCodeDAO.GetCurrCodesList(conn);
            DbConn.Instance.FreeConnection(conn);
            return View(account);
        }

        [HttpPost]
        public IActionResult Add([Bind("name", "account_type_code", "is_active", "currency_code", "row_version")] Account account, [Bind("accOwnerId")] int accOwnerId)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));

            //account.account_owner_id = accOwnerid;

            using (conn)
            {
                string request = "select sb.modify_account (pii_id => @id, pvi_name => @name, " +
                                                    "pvi_account_type_code => @account_type_code, " +
                                                    "pii_account_owner_id => @accOwnerId, " +
                                                    "pbi_is_active => @is_active, " +
                                                    "pvi_currency_code => @currency_code, " +
                                                    "pii_row_version => @row_version)";
                NpgsqlCommand cmd = new NpgsqlCommand(request, conn);
                //SetCommandType(cmd, CommandType.Text);
                cmd.Parameters.AddWithValue("id", NpgsqlDbType.Integer, DBNull.Value);
                cmd.Parameters.AddWithValue("name", NpgsqlDbType.Varchar, account.name);
                cmd.Parameters.AddWithValue("account_type_code", NpgsqlDbType.Varchar, account.account_type_code);
                cmd.Parameters.AddWithValue("accOwnerId", NpgsqlDbType.Integer, accOwnerId);
                cmd.Parameters.AddWithValue("is_active", NpgsqlDbType.Boolean, account.is_active);
                cmd.Parameters.AddWithValue("currency_code", NpgsqlDbType.Varchar, account.currency_code);
                cmd.Parameters.AddWithValue("row_version", NpgsqlDbType.Integer, account.row_version);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                DbConn.Instance.FreeConnection(conn);
            }

            return Redirect("/Account/List?accOwnerId="+ accOwnerId.ToString());
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            Account account = AccountDAO.FindById(id);
            account.accountTypesList = AccTypeDAO.GetAccTypesList(conn);
            account.currencyCodesList = CurrencyCodeDAO.GetCurrCodesList(conn);
            DbConn.Instance.FreeConnection(conn);
            return View(account);
        }

        [HttpPost]
        public IActionResult Edit(int id, [Bind("name", "account_type_code", "account_owner_id", "is_active", "currency_code", "row_version")] Account account)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            using (conn)
            {
                string request = "select sb.modify_account (pii_id => @id, pvi_name => @name, " +
                                                    "pvi_account_type_code => @account_type_code, " +
                                                    "pii_account_owner_id => @accOwnerId, " +
                                                    "pbi_is_active => @is_active, " +
                                                    "pvi_currency_code => @currency_code, " +
                                                    "pii_row_version => @row_version)";
                NpgsqlCommand cmd = new NpgsqlCommand(request, conn);
                //SetCommandType(cmd, CommandType.Text);
                cmd.Parameters.AddWithValue("id", NpgsqlDbType.Integer, id);
                cmd.Parameters.AddWithValue("name", NpgsqlDbType.Varchar, account.name);
                cmd.Parameters.AddWithValue("account_type_code", NpgsqlDbType.Varchar, account.account_type_code);
                cmd.Parameters.AddWithValue("accOwnerId", NpgsqlDbType.Integer, account.account_owner_id);
                cmd.Parameters.AddWithValue("is_active", NpgsqlDbType.Boolean, account.is_active);
                cmd.Parameters.AddWithValue("currency_code", NpgsqlDbType.Varchar, account.currency_code);
                cmd.Parameters.AddWithValue("row_version", NpgsqlDbType.Integer, account.row_version);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                DbConn.Instance.FreeConnection(conn);
            }
            return Redirect("/Account/List?accOwnerId=" + account.account_owner_id.ToString());
        }
    }
}
