using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;


namespace WebAppPg.Models
{
    public class MyConn
    {
        private string _mainConnectionString;
        private string _usersConnectionString;

        private static MyConn _dbConnInstance;

        private static object syncObj = new Object();

        private MyConn() { }

        public static void CreateInstance(IConfiguration configuration)
        {
            if (_dbConnInstance == null)
                lock (syncObj)
                {
                    if (_dbConnInstance == null)
                    {
                        _dbConnInstance = new MyConn();
                        _dbConnInstance._mainConnectionString = configuration.GetConnectionString("MainPgDatabase");
                        _dbConnInstance._usersConnectionString = configuration.GetConnectionString("UsersPgDatabase");
                    }
                }
        }

        public static MyConn Instance
        {
            get
            {
                return _dbConnInstance;
            }
        }

        public NpgsqlConnection GetMainConnection(int appUserId)
        {
            NpgsqlConnection conn = new NpgsqlConnection(_mainConnectionString);
            conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand("call sbudget.session_set_app_user(" + appUserId.ToString() + ")", conn);
            cmd.ExecuteNonQuery();
            return conn;
        }

        public NpgsqlConnection GetUsersConnection()
        {
            NpgsqlConnection conn = new NpgsqlConnection(_usersConnectionString);
            conn.Open();
            return conn;
        }

        public void FreeConnection(NpgsqlConnection connection)
        {
            connection.Close();
        }
    }
}
