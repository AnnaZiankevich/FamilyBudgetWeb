﻿using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;


namespace WebAppPg
{
    public class MyConn
    {
        private static MyConn myConnInstance;
        private static object syncObj = new Object();
        //private static IConfiguration config;
        private string connectionString = "server=localhost;port=5432;user id=postgres;password=123;database=postgres;Pooling=true;Minimum Pool Size=10;Maximum Pool Size=100;Application Name=Smart Budget Web";

        private MyConn()
        {

        }

        public static MyConn getInstance()
        {
            if (myConnInstance == null)
            {
                lock (syncObj)
                {
                    if (myConnInstance == null)
                        myConnInstance = new MyConn();
                }
            }
            return myConnInstance;
        }
        public NpgsqlConnection GetConnection(int appUserId)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand("call sbudget.session_set_app_user(" + appUserId.ToString() + ")", conn);
            cmd.ExecuteNonQuery();
            return conn;
        }
        
        public void FreeConnection(NpgsqlConnection connection)
        {
            connection.Close();
        }
    }
}
