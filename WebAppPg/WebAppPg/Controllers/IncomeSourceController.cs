using Microsoft.AspNetCore.Mvc;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAppPg.Models;

namespace WebAppPg.Controllers
{
    public class IncomeSourceController : Controller
    {
        [HttpGet]
        public IActionResult Add()
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));
            IncomeSource incomeSource = new IncomeSource();
            DbConn.Instance.FreeConnection(conn);
            return View(incomeSource);
        }

        [HttpPost]
        public IActionResult Add([Bind("name", "is_active")] IncomeSource incomeSource)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            NpgsqlConnection conn = DbConn.Instance.GetMainConnection(int.Parse(userId));

            using (conn)
            {
                string request = "select sb.modify_income_sources (pii_income_source_id => @id, pvi_name => @name, " +
                                                    "pbi_is_active => @is_active)";
                NpgsqlCommand cmd = new NpgsqlCommand(request, conn);
                //SetCommandType(cmd, CommandType.Text);
                cmd.Parameters.AddWithValue("id", NpgsqlDbType.Integer, DBNull.Value);
                cmd.Parameters.AddWithValue("name", NpgsqlDbType.Varchar, incomeSource.name);
                cmd.Parameters.AddWithValue("is_active", NpgsqlDbType.Boolean, incomeSource.is_active);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                DbConn.Instance.FreeConnection(conn);
            }

            return Redirect("/Income/Index");
        }
    }
}
