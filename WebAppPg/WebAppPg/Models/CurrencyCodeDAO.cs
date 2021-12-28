using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPg.Models
{
    public class CurrencyCodeDAO
    {
        public static List<CurrencyCode> GetCurrCodesList(NpgsqlConnection conn)
        {
            conn = DbConn.Instance.GetUsersConnection();
            NpgsqlCommand cmd = new NpgsqlCommand("select code, name from sb.currencies", conn);
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            List<CurrencyCode> currCodes = new List<CurrencyCode>();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    currCodes.Add(new CurrencyCode
                    {
                        code = rdr.GetString(0),
                        name = rdr.GetString(1)
                    });
                }
            }
            DbConn.Instance.FreeConnection(conn);
            return currCodes;
        }
    }
}
