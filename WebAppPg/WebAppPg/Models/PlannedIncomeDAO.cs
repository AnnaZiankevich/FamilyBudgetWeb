using Npgsql;
using NpgsqlTypes;
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
            NpgsqlCommand cmd = new NpgsqlCommand("select -1 as id, '-- no planned income --' as name union all select id, name from sb.planned_income", conn);
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

        public static List<PlannedIncome> GetPlannedIncomeListForPeriod(NpgsqlConnection conn)
        {
            conn = DbConn.Instance.GetUsersConnection();
            NpgsqlCommand cmd = new NpgsqlCommand("select id, name from sb.planned_income where " +
                                                      "planned_date >= current_date " +
                                                      "and planned_date <= current_date + interval '1 month'", 
                                                      conn);
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

        public static PlannedIncome FindById(int id)
        {
            int plnIncId;
            string plnIncName;
            int accountId;
            int plnIncSourceId;
            int plnIncTypeId;
            decimal amount;
            string currencyCode;
            DateTime? plnIncPlannedDate;
            DateTime? plnIncStartDate;
            DateTime? plnIncEndDate;
            int rowVersion;
            PlannedIncome plnInc = null;
            NpgsqlConnection conn = DbConn.Instance.GetUsersConnection();
            NpgsqlCommand cmd = new NpgsqlCommand("select id, name, account_id, income_source_id, income_type_id, amount, currency_code, planned_date, start_date, end_date, row_version from sb.planned_income where id = @id", conn);
            cmd.Parameters.AddWithValue("id", NpgsqlDbType.Integer, id);
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                plnIncId = rdr.GetInt32(0);
                plnIncName = rdr.GetString(1);
                accountId = rdr.GetInt32(2);
                plnIncSourceId = rdr.GetInt32(3);
                plnIncTypeId = rdr.GetInt32(4);
                amount = rdr.GetDecimal(5);
                currencyCode = rdr.GetString(6);
                if (!rdr.IsDBNull(7))
                    plnIncPlannedDate = rdr.GetDateTime(7);
                else
                    plnIncPlannedDate = null;
                if (!rdr.IsDBNull(8))
                    plnIncStartDate = rdr.GetDateTime(8);
                else
                    plnIncStartDate = null;
                if (!rdr.IsDBNull(9))
                    plnIncEndDate = rdr.GetDateTime(9);
                else
                    plnIncEndDate = null;
                rowVersion = rdr.GetInt32(10);

                plnInc = new PlannedIncome { id = plnIncId, name = plnIncName, account_id = accountId, income_source_id = plnIncSourceId, income_type_id = plnIncTypeId, amount = amount, currency_code = currencyCode, planned_date = plnIncPlannedDate, start_date = plnIncStartDate, end_date = plnIncEndDate, row_version = rowVersion };
            }
            DbConn.Instance.FreeConnection(conn);
            return plnInc;
        }
    }
}

