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
            NpgsqlConnection conn = MyConn.Instance.GetUsersConnection();
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
            MyConn.Instance.FreeConnection(conn);
            return appUser;
        }

        public static MyTest1 FindById(int id)
        {
            int accOwnId;
            string accOwnName;
            bool accOwnIsActive;
            int accOwnAppUserId;
            MyTest1 accOwn = null;
            NpgsqlConnection conn = MyConn.Instance.GetUsersConnection();
            NpgsqlCommand cmd = new NpgsqlCommand("select id, name, is_active, app_user_id from sb.account_owners where id = @id", conn);
            cmd.Parameters.AddWithValue("id", NpgsqlDbType.Integer, id);
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                accOwnId = rdr.GetInt32(0);
                accOwnName = rdr.GetString(1);
                accOwnIsActive = rdr.GetBoolean(2);
                accOwnAppUserId = rdr.GetInt32(3);
                accOwn = new MyTest1 { id = accOwnId, name = accOwnName, is_active = accOwnIsActive, app_user_id = accOwnAppUserId };
            }
            MyConn.Instance.FreeConnection(conn);
            return accOwn;
        }
    }
}
