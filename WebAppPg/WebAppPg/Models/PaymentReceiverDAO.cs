using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPg.Models
{
    public class PaymentReceiverDAO
    {
        public static List<PaymentReceiver> GetPaymentReceiverList(NpgsqlConnection conn)
        {
            conn = DbConn.Instance.GetUsersConnection();
            NpgsqlCommand cmd = new NpgsqlCommand("select id, name from sb.payment_receivers", conn);
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            List<PaymentReceiver> paymReceiver = new List<PaymentReceiver>();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    paymReceiver.Add(new PaymentReceiver
                    {
                        id = rdr.GetInt32(0),
                        name = rdr.GetString(1)
                    });
                }
            }
            DbConn.Instance.FreeConnection(conn);
            return paymReceiver;
        }
    }
}
