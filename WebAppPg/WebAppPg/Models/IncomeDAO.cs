using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPg.Models
{
    public class IncomeDAO
    {
        public static Income FindById(int id)
        {
            int incomeId;
            string incomeName;
            int plannedIncomeId;
            int accountId;
            int incomeSourceId;
            int incomeTypeId;
            decimal amount;
            string currencyCode;
            DateTime income_date;
            int rowVersion;
            Income income = null;
            NpgsqlConnection conn = DbConn.Instance.GetUsersConnection();
            NpgsqlCommand cmd = new NpgsqlCommand("select id, name, planned_income_id, account_id, income_source_id, income_type_id, amount, currency_code, income_date, row_version from sb.income where id = @id", conn);
            cmd.Parameters.AddWithValue("id", NpgsqlDbType.Integer, id);
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                incomeId = rdr.GetInt32(0);
                incomeName = rdr.GetString(1);
                if (!rdr.IsDBNull(2))
                    plannedIncomeId = rdr.GetInt32(2);
                else
                    plannedIncomeId = -1;              
                accountId = rdr.GetInt32(3);               
                incomeSourceId = rdr.GetInt32(4);
                incomeTypeId = rdr.GetInt32(5);
                amount = rdr.GetDecimal(6);
                currencyCode = rdr.GetString(7);
                income_date = rdr.GetDateTime(8);
                rowVersion = rdr.GetInt32(9);
                
                income = new Income { id = incomeId, name = incomeName, planned_income_id = plannedIncomeId, account_id = accountId, income_source_id = incomeSourceId, income_type_id = incomeTypeId, amount = amount, currency_code = currencyCode, income_date = income_date, row_version = rowVersion };
            }
            DbConn.Instance.FreeConnection(conn);
            return income;
        }
    }
}
