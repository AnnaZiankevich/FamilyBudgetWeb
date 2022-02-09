using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPg.Models
{
    public class AccountDAO
    {
        public static Account FindById(int id)
        {
            int accId;
            string accName;
            string accTypeCode;
            int accOwnerId;
            bool accIsActive;
            string accCurrencyCode;
            int accRowVersion;
            Account account = null;
            NpgsqlConnection conn = DbConn.Instance.GetUsersConnection();
            NpgsqlCommand cmd = new NpgsqlCommand("select id, name, account_type_code, account_owner_id, " +
                                                         "is_active, currency_code, row_version from sb.accounts where id = @id", conn);
            cmd.Parameters.AddWithValue("id", NpgsqlDbType.Integer, id);
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                accId = rdr.GetInt32(0);
                accName = rdr.GetString(1);
                accTypeCode = rdr.GetString(2);
                accOwnerId = rdr.GetInt32(3);
                accIsActive = rdr.GetBoolean(4);
                accCurrencyCode = rdr.GetString(5);
                accRowVersion = rdr.GetInt32(6);

                account = new Account { id = accId, name = accName, account_type_code = accTypeCode, account_owner_id = accOwnerId, is_active = accIsActive, currency_code = accCurrencyCode, row_version = accRowVersion };
            }
            DbConn.Instance.FreeConnection(conn);
            return account;
        }

        public static List<Account> GetAccountList(NpgsqlConnection conn)
        {
            conn = DbConn.Instance.GetUsersConnection();
            NpgsqlCommand cmd = new NpgsqlCommand("select id, name from sb.accounts order by name", conn);
            List<Account> accounts = new List<Account>();
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    accounts.Add(new Account
                    {
                        id = rdr.GetInt32(0),
                        name = rdr.GetString(1)
                    });
                }
            }
            DbConn.Instance.FreeConnection(conn);
            return accounts;
        }

        public static List<Account> GetAccountListTotal(NpgsqlConnection conn)
        {
            conn = DbConn.Instance.GetUsersConnection();
            NpgsqlCommand cmd = new NpgsqlCommand("select -1 as id, '<All accounts>' as name union all select id, name from sb.accounts", conn);
            List<Account> accounts = new List<Account>();
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    accounts.Add(new Account
                    {
                        id = rdr.GetInt32(0),
                        name = rdr.GetString(1)
                    });
                }
            }
            DbConn.Instance.FreeConnection(conn);
            return accounts;
        }
    }
}
