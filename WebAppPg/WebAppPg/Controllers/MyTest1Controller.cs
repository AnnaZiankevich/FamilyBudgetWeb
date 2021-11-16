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

namespace WebAppPg.Controllers
{
    public class MyTest1Controller : Controller
    {
        public IActionResult Index()
        {
            List<MyTest1> myTets1List = new List<MyTest1>();

            string CS = "server=localhost;port=5432;user id=postgres;password=123;database=postgres";
            using (NpgsqlConnection con = new NpgsqlConnection(CS))
            {
                NpgsqlCommand cmd = CreateCommand("select id, name, first_date from sbudget.account_owners", con);
                con.Open();

                NpgsqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var myTest1 = new MyTest1();
                    myTest1.id = Convert.ToInt32(rdr["id"]);
                    myTest1.name = rdr["name"].ToString();
                    if (rdr["first_date"] != DBNull.Value)
                    {
                        myTest1.first_date = Convert.ToDateTime(rdr["first_date"]);
                        myTest1.first_date_is_null = false;
                    }
                    else
                    {
                        myTest1.first_date_is_null = true;
                    }
                    myTets1List.Add(myTest1);
                }
            }
            return View(myTets1List);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            string CS = "server=localhost;port=5432;user id=postgres;password=p123;database=postgres";
            var myTest1 = new MyTest1();
            using (NpgsqlConnection con = new NpgsqlConnection(CS))
            {
                NpgsqlCommand cmd = CreateCommand("select id, name, first_date from sbudget.account_owners where id = " + id.ToString(), con);
                con.Open();
                NpgsqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    myTest1.id = Convert.ToInt32(rdr["id"]);
                    myTest1.name = rdr["name"].ToString();
                    if (rdr["first_date"] != DBNull.Value)
                    {
                        myTest1.first_date = Convert.ToDateTime(rdr["first_date"]);
                        myTest1.first_date_is_null = false;
                    }
                    else
                    {
                        myTest1.first_date_is_null = true;
                    }
                }
                con.Close();
            }
            return View(myTest1);
        }

        [HttpPost]
        public IActionResult Edit(int id, [Bind("name")] MyTest1 myTest1)
        {
            string CS = "server=localhost;port=5432;user id=postgres;password=123;database=postgres";
            using (NpgsqlConnection con = new NpgsqlConnection(CS))
            {
                NpgsqlCommand cmd = CreateCommand("update sbudget.account_owners set name = @name where id = " + id.ToString(), con);
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("name", NpgsqlDbType.Varchar, myTest1.name);
                con.Open();
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            return Redirect("/MyTest1");
        }

        public IActionResult Delete(int id)
        {
            string CS = "server=localhost;port=5432;user id=postgres;password=123;database=postgres";
            using (NpgsqlConnection con = new NpgsqlConnection(CS))
            {
                NpgsqlCommand cmd = CreateCommand("delete from sbudget.account_owners where id = " + id.ToString(), con);
                cmd.CommandType = System.Data.CommandType.Text;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            return Redirect("/MyTest1");
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add([Bind("name")] MyTest1 myTest1)
        {
            string CS = "server=localhost;port=5432;user id=postgres;password=123;database=postgres";
            using (NpgsqlConnection con = new NpgsqlConnection(CS))
            {
                NpgsqlCommand cmd = CreateCommand("insert into sbudget.account_owners (name) values(@name)", con);
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("name", NpgsqlDbType.Varchar, myTest1.name);
                con.Open();
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            return Redirect("/MyTest1");
        }

        public NpgsqlCommand CreateCommand(string request, NpgsqlConnection connection)
        {
            return new NpgsqlCommand(request, connection);
        }

    }
}
