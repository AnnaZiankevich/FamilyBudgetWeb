using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace WebAppPg
{
    public class DAOFactory
    {
        //NpgsqlConnectionStringBuilder objBuild = new NpgsqlConnectionStringBuilder();
        NpgsqlConnection objConn = new NpgsqlConnection();

        public DAOFactory()
        {
            NpgsqlConnectionStringBuilder objBuild = new NpgsqlConnectionStringBuilder();
            objBuild.Host = "localhost";
            objBuild.Port = 5432;
            objBuild.Database = "postgres";
            objBuild.Username = "postgres";
            objBuild.Password = "123";
            objBuild.MaxPoolSize = 100;
            objBuild.MinPoolSize = 1;
            objBuild.Pooling = true;
            objConn.ConnectionString = objBuild.ConnectionString;
        }

        public void OpenConn (NpgsqlConnection conn)
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
                conn.Open();
        }

        public string ConnStr
        {
            get
            {
                return objConn.ConnectionString;
            }
            set
            {
                ConnStr = value;
            }
        }
    }
}
