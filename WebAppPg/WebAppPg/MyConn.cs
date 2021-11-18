using System;
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
    public static class MyConn
    {
        private static readonly IConfiguration config;
        static NpgsqlConnection connection;
        static Boolean isInitialized = false;
        //IDecorator
        //Decorator

        private static MyConn(IConfiguration configuration)
        {
            config = configuration;
        }

        public static NpgsqlConnection GetConnection() 
        {
            if (!isInitialized) OpenConnection();
            return connection;
            //return new NpgsqlConnection(dbConn);
        }

        public static void OpenConnection()
        {
            string dbConn = config.GetValue<string>("ConnectionStrings:PGDB");
            connection = new NpgsqlConnection(dbConn);
            connection.Open();
            isInitialized = true;
        }
        
        public static void FreeConnection()
        {
            if (isInitialized) connection.Close();
        }
    }
}
