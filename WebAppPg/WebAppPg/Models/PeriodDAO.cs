using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPg.Models
{
    public class PeriodDAO
    {
        public static List<Period> GetPeriodList(NpgsqlConnection conn)
        {
            conn = DbConn.Instance.GetUsersConnection();
            NpgsqlCommand cmd = new NpgsqlCommand("select id, name from sb.periods", conn);
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            List<Period> period = new List<Period>();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    period.Add(new Period
                    {
                        id = rdr.GetInt32(0),
                        name = rdr.GetString(1)
                    });
                }
            }
            DbConn.Instance.FreeConnection(conn);
            return period;
        }
    }
}
