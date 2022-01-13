using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPg.Models
{
    public class PaymentDAO
    {
        public static Payment FindById(int id)
        {
            int paymentId;
            string paymentName;
            int plannedPaymentId;
            int accountId;
            int paymentReceiverId;
            int paymentTypeId;
            decimal amount;
            string currencyCode;
            DateTime paymentDate;
            int rowVersion;
            Payment payment = null;
            NpgsqlConnection conn = DbConn.Instance.GetUsersConnection();
            NpgsqlCommand cmd = new NpgsqlCommand("select id, name, planned_payment_id, account_id, payment_receiver_id, payment_type_id, amount, currency_code, payment_date, row_version from sb.payments where id = @id", conn);
            cmd.Parameters.AddWithValue("id", NpgsqlDbType.Integer, id);
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                paymentId = rdr.GetInt32(0);
                paymentName = rdr.GetString(1);
                if (!rdr.IsDBNull(2))
                    plannedPaymentId = rdr.GetInt32(2);
                else
                    plannedPaymentId = -1;
                accountId = rdr.GetInt32(3);             
                paymentReceiverId = rdr.GetInt32(4);
                paymentTypeId = rdr.GetInt32(5);
                amount = rdr.GetDecimal(6);
                currencyCode = rdr.GetString(7);
                paymentDate = rdr.GetDateTime(8);
                rowVersion = rdr.GetInt32(9);

                payment = new Payment { id = paymentId, name = paymentName, planned_payment_id = plannedPaymentId, account_id = accountId, payment_receiver_id = paymentReceiverId, payment_type_id = paymentTypeId, amount = amount, currency_code = currencyCode, payment_date = paymentDate, row_version = rowVersion };
            }
            DbConn.Instance.FreeConnection(conn);
            return payment;
        }
    }
}

