using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPg.Models
{
    public class AccTypeDAO
    {
        public static List<AccountType> GetAccTypesList(NpgsqlConnection conn)
        {
            conn = DbConn.Instance.GetUsersConnection();
            NpgsqlCommand cmd = new NpgsqlCommand("select code, name from sb.account_types", conn);
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            List<AccountType> appUsers = new List<AccountType>();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    appUsers.Add(new AccountType
                    {
                        code = rdr.GetString(0),
                        name = rdr.GetString(1)
                    });
                }
            }
            DbConn.Instance.FreeConnection(conn);
            return appUsers;
        }
    }
}
