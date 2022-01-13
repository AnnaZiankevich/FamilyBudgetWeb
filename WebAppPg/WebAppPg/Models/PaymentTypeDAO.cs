using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPg.Models
{
    public class PaymentTypeDAO
    {
        public static List<PaymentType> GetPaymentTypeList(NpgsqlConnection conn)
        {
            conn = DbConn.Instance.GetUsersConnection();
            NpgsqlCommand cmd = new NpgsqlCommand("select id, name from sb.payment_types", conn);
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            List<PaymentType> paymType = new List<PaymentType>();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    paymType.Add(new PaymentType
                    {
                        id = rdr.GetInt32(0),
                        name = rdr.GetString(1)
                    });
                }
            }
            DbConn.Instance.FreeConnection(conn);
            return paymType;
        }
    }
}
