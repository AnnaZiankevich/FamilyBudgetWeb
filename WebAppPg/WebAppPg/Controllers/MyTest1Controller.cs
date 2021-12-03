using WebAppPg.Models;
using Microsoft.AspNetCore.Mvc;
using System;
//using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace WebAppPg.Controllers
{
    //[Authorize]
    public class MyTest1Controller : Controller
    {
        //private string m_connectionString = "server=localhost;port=5432;user id=postgres;password=123;database=postgres";
        static List<MyTest1> myTets1List = new List<MyTest1>();
        public IActionResult Index()
        {
            NpgsqlConnection connection = MyConn.Instance.GetUsersConnection();
            using (connection)
            {
                NpgsqlCommand cmd = CreateCommand("select a.id, a.name, a.is_active, a.app_user_id, b.user_name from sb.account_owners a " +
                                                               "join sb.app_users b on b.id = a.app_user_id", connection);

                NpgsqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var myTest1 = new MyTest1();
                    myTets1List.Add(myTest1);
                    Read(myTest1, rdr);
                }
                //connection.Close();
                MyConn.Instance.FreeConnection(connection);
            }
            return View(myTets1List.OrderBy(s => s.id).ToList());
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var myTest1 = new MyTest1();
            NpgsqlConnection connection = MyConn.Instance.GetUsersConnection();
            using (connection)
            {
                string request = "select id, name, is_active, app_user_id from sb.account_owners";
                NpgsqlCommand cmd = CreateCommand(request, connection);
                NpgsqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read()) Read(myTest1, rdr);
                MyConn.Instance.FreeConnection(connection);
            }
            //var accOwners = myTest1.Where(s => s.id == id).FirstOrDefault();
            return View(myTest1);
        }

        [HttpPost]
        public IActionResult Edit(int id, [Bind("name")] MyTest1 myTest1)
        {
            NpgsqlConnection connection = MyConn.Instance.GetUsersConnection();
            using (connection)
            {
                string request = "update sbudget.account_owners set name = @name where id = " + id.ToString();
                NpgsqlCommand cmd = CreateCommand(request, connection);
                SetCommandType(cmd, CommandType.Text);
                AddParameter(cmd, "name", NpgsqlDbType.Varchar, myTest1.name);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                MyConn.Instance.FreeConnection(connection);
            }
            return Redirect("/MyTest1/Index");
        }

        public IActionResult Delete(int id)
        {
            NpgsqlConnection connection = MyConn.Instance.GetUsersConnection();
            using (connection)
            {
                string request = "select sb.account_owners_delete(pii_id => @id)";
                NpgsqlCommand cmd = CreateCommand(request, connection);
                AddParameter(cmd, "id", NpgsqlDbType.Integer, id);
                SetCommandType(cmd, CommandType.Text);
                cmd.ExecuteNonQuery();
                MyConn.Instance.FreeConnection(connection);
            }
            return Redirect("/MyTest1/Index");
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add([Bind("name", "is_active", "app_user_id")] MyTest1 myTest1)
        {
            NpgsqlConnection connection = MyConn.Instance.GetUsersConnection();

            using (connection)
            {
                string request = "select sb.modify_account_owner (pii_id => @id, pvi_name => @name, pbi_is_active => @is_active, pii_app_user_id => @app_user_id)";
                NpgsqlCommand cmd = CreateCommand(request, connection);
                SetCommandType(cmd, CommandType.Text);
                AddParameter(cmd, "id", NpgsqlDbType.Integer, DBNull.Value);
                AddParameter(cmd, "name", NpgsqlDbType.Varchar, myTest1.name);
                AddParameter(cmd, "is_active", NpgsqlDbType.Boolean, myTest1.is_active);
                AddParameter(cmd, "app_user_id", NpgsqlDbType.Integer, myTest1.is_active);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                MyConn.Instance.FreeConnection(connection);
            }

            return Redirect("/MyTest1/Index");
        }

        //public NpgsqlConnection CreateConnection()
        //{
            //return new NpgsqlConnection(m_connectionString);
        //}

        public NpgsqlCommand CreateCommand(string request, NpgsqlConnection connection)
        {
            return new NpgsqlCommand(request, connection);
        }

        public void Read(MyTest1 model, NpgsqlDataReader reader)
        {
            model.id = Convert.ToInt32(reader["id"]);
            model.name = reader["name"].ToString();
            model.is_active = Convert.ToBoolean(reader["is_active"]);
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
