using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppPg.Models;

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
            NpgsqlConnection conn = DbConn.Instance.GetUsersConnection();
            NpgsqlCommand cmd = new NpgsqlCommand("select id, user_login, user_password from sb.app_users where user_login = @login", conn);
            cmd.Parameters.AddWithValue("login", NpgsqlDbType.Varchar, login);
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                userId = rdr.GetInt32(0);
                userLogin = rdr.GetString(1);
                pwdHash = rdr.GetString(2);
                appUser = new UserModel { Id = userId, Login = userLogin, Password = pwdHash };
            }
            DbConn.Instance.FreeConnection(conn);
            return appUser;
        }

        public static List<AppUserList> GetAppUserList(NpgsqlConnection conn)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("select -1 as id, '-- no app user --' as user_name union all select id, user_name from sb.app_users", conn);
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            List<AppUserList> appUsers = new List<AppUserList>();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    appUsers.Add(new AppUserList
                    {
                        id = rdr.GetInt32(0),
                        user_name = rdr.GetString(1)
                    });
                }
            }
            return appUsers;
        }
    }
}

