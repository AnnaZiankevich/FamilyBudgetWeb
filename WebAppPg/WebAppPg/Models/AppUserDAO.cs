using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPg.Models
{
    public static class AppUserDAO
    {
        public static UserModel FindByName(string login)
        {
            int userId;
            string userLogin;
            string pwdHash;
            UserModel appUser = null;
            NpgsqlConnection conn = MyConn.Instance.GetUsersConnection();
            NpgsqlCommand cmd = new NpgsqlCommand("select id, user_login, user_password from sbudget.app_users where user_login = @login", conn);
            cmd.Parameters.AddWithValue("login", NpgsqlDbType.Varchar, login);
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                userId = rdr.GetInt32(0);
                userLogin = rdr.GetString(1);
                pwdHash = rdr.GetString(2);
                appUser = new UserModel { Id = userId, Login = userLogin, Password = pwdHash };
            }
            MyConn.Instance.FreeConnection(conn);
            return appUser;
        }
    }
}
