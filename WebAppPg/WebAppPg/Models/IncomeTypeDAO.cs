using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPg.Models
{
    public class IncomeTypeDAO
    {
        public static List<IncomeType> GetIncomeTypeList(NpgsqlConnection conn)
        {
            conn = DbConn.Instance.GetUsersConnection();
            NpgsqlCommand cmd = new NpgsqlCommand("select id, name from sb.income_types", conn);
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            List<IncomeType> incomeType = new List<IncomeType>();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    incomeType.Add(new IncomeType
                    {
                        id = rdr.GetInt32(0),
                        name = rdr.GetString(1)
                    });
                }
            }
            DbConn.Instance.FreeConnection(conn);
            return incomeType;
        }
    }
}
