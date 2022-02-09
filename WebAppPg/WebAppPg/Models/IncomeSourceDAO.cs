using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPg.Models
{
    public class IncomeSourceDAO
    {
        public static List<IncomeSource> GetIncomeSourceList(NpgsqlConnection conn)
        {
            conn = DbConn.Instance.GetUsersConnection();
            NpgsqlCommand cmd = new NpgsqlCommand("select id, name from sb.income_sources", conn);
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            List<IncomeSource> incomeSourse = new List<IncomeSource>();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    incomeSourse.Add(new IncomeSource
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
