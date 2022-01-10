using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPg.Models
{
    public class PlannedIncomeDAO
    {
        public static List<PlannedIncome> GetPlannedIncomeList(NpgsqlConnection conn)
        {
            conn = DbConn.Instance.GetUsersConnection();
            NpgsqlCommand cmd = new NpgsqlCommand("select id, name from sb.planned_income", conn);
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            List<PlannedIncome> plannedIncome = new List<PlannedIncome>();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    plannedIncome.Add(new PlannedIncome
                    {
                        id = rdr.GetInt32(0),
                        name = rdr.GetString(1)
                    });
                }
            }
            DbConn.Instance.FreeConnection(conn);
            return plannedIncome;
        }
    }
}

