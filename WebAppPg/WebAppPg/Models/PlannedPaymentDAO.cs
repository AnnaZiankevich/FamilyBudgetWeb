using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPg.Models
{
    public class PlannedPaymentDAO
    {
        public static List<PlannedPayment> GetPlannedPaymentList(NpgsqlConnection conn)
        {
            conn = DbConn.Instance.GetUsersConnection();
            NpgsqlCommand cmd = new NpgsqlCommand("select -1 as id, '-- no planned payment --' as name union all select id, name from sb.planned_payments", conn);
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            List<PlannedPayment> plannedPayment = new List<PlannedPayment>();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    plannedPayment.Add(new PlannedPayment
                    {
                        id = rdr.GetInt32(0),
                        name = rdr.GetString(1)
                    });
                }
            }
            DbConn.Instance.FreeConnection(conn);
            return plannedPayment;
        }
    }
}
