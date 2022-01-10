using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPg.Models
{
    public class IncomeSourceDAO
    {
        public static List<IncomeSourse> GetIncomeSourceList(NpgsqlConnection conn)
        {
            conn = DbConn.Instance.GetUsersConnection();
            NpgsqlCommand cmd = new NpgsqlCommand("select id, name from sb.income_sources", conn);
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            List<IncomeSourse> incomeSourse = new List<IncomeSourse>();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    incomeSourse.Add(new IncomeSourse
                    {
                        id = rdr.GetInt32(0),
                        name = rdr.GetString(1)
                    });
                }
            }
            DbConn.Instance.FreeConnection(conn);
            return incomeSourse;
        }
    }
}
