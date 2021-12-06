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
    public class AccountOwnerController : Controller
    {
        static List<AccountOwner> accOwnerList = new List<AccountOwner>();
        public IActionResult Index()
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            using (conn)
            {
                NpgsqlCommand cmd = CreateCommand("select a.id, a.name, a.is_active, a.app_user_id, b.user_name from sb.account_owners a " +
                                                               "left join sb.app_users b on b.id = a.app_user_id", conn);
                accOwnerList.Clear();
                NpgsqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var accOwner = new AccountOwner();
                    accOwnerList.Add(accOwner);
                    Read(accOwner, rdr);
                }
                DbConn.Instance.FreeConnection(conn);
            }
            return View(accOwnerList.OrderBy(s => s.id).ToList());
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            AccountOwner accOwners = AccOwnerDAO.FindById(id);
            accOwners.app_user_list = AppUserDAO.GetAppUserList(conn);
            DbConn.Instance.FreeConnection(conn);
            return View(accOwners);
        }

        [HttpPost]
        public IActionResult Edit(int id, [Bind("name", "is_active", "app_user_id", "row_version")] AccountOwner accOwner)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            using (conn)
            {
                string request = "select sb.modify_account_owner (pii_id => :id, pvi_name => :name, " +
                                                "pbi_is_active => :is_active, pii_row_version => :row_version, " +
                                                "pii_app_user_id => :app_user_id) ";
                NpgsqlCommand cmd = CreateCommand(request, conn);
                SetCommandType(cmd, CommandType.Text);
                AddParameter(cmd, "id", NpgsqlDbType.Integer, id);
                AddParameter(cmd, "name", NpgsqlDbType.Varchar, accOwner.name);
                AddParameter(cmd, "is_active", NpgsqlDbType.Boolean, accOwner.is_active);
                AddParameter(cmd, "row_version", NpgsqlDbType.Integer, accOwner.row_version);
                AddParameter(cmd, "app_user_id", NpgsqlDbType.Integer, accOwner.app_user_id);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                DbConn.Instance.FreeConnection(conn);
            }
            return Redirect("/AccountOwner/Index");
        }

        public IActionResult Delete(int id)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            using (conn)
            {
                string request = "select sb.account_owners_delete(pii_id => @id)";
                NpgsqlCommand cmd = CreateCommand(request, conn);
                AddParameter(cmd, "id", NpgsqlDbType.Integer, id);
                SetCommandType(cmd, CommandType.Text);
                cmd.ExecuteNonQuery();
                DbConn.Instance.FreeConnection(conn);
            }
            return Redirect("/AccountOwner/Index");
        }

        [HttpGet]
        public IActionResult Add()
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            AccountOwner accOwners = new AccountOwner();
            accOwners.app_user_list = AppUserDAO.GetAppUserList(conn);
            accOwners.app_user_id = -1;
            DbConn.Instance.FreeConnection(conn);
            return View(accOwners);
        }

        [HttpPost]
        public IActionResult Add([Bind("name", "is_active", "app_user_id")] AccountOwner accOwner)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));

            using (conn)
            {
                string request = "select sb.modify_account_owner (pii_id => @id, pvi_name => @name, " +
                                                    "pbi_is_active => @is_active, pii_row_version => @row_version," +
                                                    "pii_app_user_id => @app_user_id)";
                NpgsqlCommand cmd = CreateCommand(request, conn);
                SetCommandType(cmd, CommandType.Text);
                AddParameter(cmd, "id", NpgsqlDbType.Integer, DBNull.Value);
                AddParameter(cmd, "name", NpgsqlDbType.Varchar, accOwner.name);
                AddParameter(cmd, "is_active", NpgsqlDbType.Boolean, accOwner.is_active);
                AddParameter(cmd, "row_version", NpgsqlDbType.Integer, accOwner.row_version);
                if (accOwner.app_user_id == -1)
                    AddParameter(cmd, "app_user_id", NpgsqlDbType.Integer, DBNull.Value);
                else
                    AddParameter(cmd, "app_user_id", NpgsqlDbType.Integer, accOwner.app_user_id);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                DbConn.Instance.FreeConnection(conn);
            }

            return Redirect("/AccountOwner/Index");
        }

        public NpgsqlCommand CreateCommand(string request, NpgsqlConnection connection)
        {
            return new NpgsqlCommand(request, connection);
        }

        public void Read(AccountOwner model, NpgsqlDataReader reader)
        {
            model.id = Convert.ToInt32(reader["id"]);
            model.name = reader["name"].ToString();
            model.is_active = Convert.ToBoolean(reader["is_active"]);
            if (reader["app_user_id"] == DBNull.Value)
                model.app_user_id = -1;
            else
                model.app_user_id = Convert.ToInt32(reader["app_user_id"]); 
        }

        public void AddParameter(NpgsqlCommand command, string parameterName, NpgsqlDbType parameterType, object value)
        {
            command.Parameters.AddWithValue(parameterName, parameterType, value);
        }

        public void SetCommandType(NpgsqlCommand command, CommandType type)
        {
            command.CommandType = type;
        }

    }
}
