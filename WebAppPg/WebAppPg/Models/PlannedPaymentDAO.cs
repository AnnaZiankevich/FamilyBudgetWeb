using Npgsql;
using NpgsqlTypes;
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

        public static List<PlannedPayment> GetPlannedPaymentListForPeriod(NpgsqlConnection conn)
        {
            conn = DbConn.Instance.GetUsersConnection();
            NpgsqlCommand cmd = new NpgsqlCommand("select id, name from sb.planned_payments where " +
                                                      "planned_date >= current_date " +
                                                      "and planned_date <= current_date + interval '1 month'" /*+
                                                      "or start_date >= current_date " +
                                                      "or start_date <= current_date + interval '1 month' "+
                                                      "or end_date >= current_date " +
                                                      "or end_date <= current_date + interval '1 month'" */,
                                                      conn);
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

        public static PlannedPayment FindById(int id)
        {
            int plnPaymId;
            string plnPaymName;
            int accountId;
            int plnPaymReceiverId;
            int plnPaymTypeId;
            decimal amount;
            string currencyCode;
            DateTime? plnPaymPlannedDate;
            DateTime? plnPaymStartDate;
            DateTime? plnPaymEndDate;
            int rowVersion;
            PlannedPayment plnPaym = null;
            NpgsqlConnection conn = DbConn.Instance.GetUsersConnection();
            NpgsqlCommand cmd = new NpgsqlCommand("select id, name, account_id, payment_receiver_id, payment_type_id, amount, currency_code, planned_date, start_date, end_date, row_version from sb.planned_payments where id = @id", conn);
            cmd.Parameters.AddWithValue("id", NpgsqlDbType.Integer, id);
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                plnPaymId = rdr.GetInt32(0);
                plnPaymName = rdr.GetString(1);
                accountId = rdr.GetInt32(2);
                plnPaymReceiverId = rdr.GetInt32(3);
                plnPaymTypeId = rdr.GetInt32(4);
                amount = rdr.GetDecimal(5);
                currencyCode = rdr.GetString(6);
                if (!rdr.IsDBNull(7))
                    plnPaymPlannedDate = rdr.GetDateTime(7);
                else
                    plnPaymPlannedDate = null;
                if (!rdr.IsDBNull(8))
                    plnPaymStartDate = rdr.GetDateTime(8);
                else
                    plnPaymStartDate = null;
                if (!rdr.IsDBNull(9))
                    plnPaymEndDate = rdr.GetDateTime(9);
                else
                    plnPaymEndDate = null;
                rowVersion = rdr.GetInt32(10);

                plnPaym = new PlannedPayment { id = plnPaymId, name = plnPaymName, account_id = accountId, payment_receiver_id = plnPaymReceiverId, payment_type_id = plnPaymTypeId, amount = amount, currency_code = currencyCode, planned_date = plnPaymPlannedDate, start_date = plnPaymStartDate, end_date = plnPaymEndDate, row_version = rowVersion };
            }
            DbConn.Instance.FreeConnection(conn);
            return plnPaym;
        }
    }
}
