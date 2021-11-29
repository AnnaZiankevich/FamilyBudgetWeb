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
using System.Web.Http;

namespace WebAppPg.Controllers
{
    //[Authorize]
    public class MyTest1Controller : Controller
    {
        //private string m_connectionString = "server=localhost;port=5432;user id=postgres;password=123;database=postgres";
        
        public IActionResult Index()
        {
            List<MyTest1> myTets1List = new List<MyTest1>();

            NpgsqlConnection connection = MyConn.Instance.GetUsersConnection();
            using (connection)
            {
                NpgsqlCommand cmd = CreateCommand("select id, name, first_date from sbudget.account_owners", connection);

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
            return View(myTets1List);
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        public IActionResult Edit(int id)
        {
            var myTest1 = new MyTest1();
            NpgsqlConnection connection = MyConn.Instance.GetUsersConnection();
            using (connection)
            {
                string request = "select id, name, first_date from sbudget.account_owners where id = " + id.ToString();
                NpgsqlCommand cmd = CreateCommand(request, connection);
                NpgsqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read()) Read(myTest1, rdr);
                MyConn.Instance.FreeConnection(connection);
            }
            return View(myTest1);
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
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
                string request = "delete from sbudget.account_owners where id = " + id.ToString();
                NpgsqlCommand cmd = CreateCommand(request, connection);
                SetCommandType(cmd, CommandType.Text);
                cmd.ExecuteNonQuery();
                MyConn.Instance.FreeConnection(connection);
            }
            return Redirect("/MyTest1/Index");
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public IActionResult Add([Bind("name")] MyTest1 myTest1)
        {
            NpgsqlConnection connection = MyConn.Instance.GetUsersConnection();

            using (connection)
            {
                string request = "insert into sbudget.account_owners (name) values(@name)";
                NpgsqlCommand cmd = CreateCommand(request, connection);
                SetCommandType(cmd, CommandType.Text);
                AddParameter(cmd, "name", NpgsqlDbType.Varchar, myTest1.name);
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
            if (reader["first_date"] != DBNull.Value)
            {
                model.first_date = Convert.ToDateTime(reader["first_date"]);
                model.first_date_is_null = false;
            }
            else
            {
                model.first_date_is_null = true;
            }
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
