using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPg.Models
{
    public class InteraccountTransactionDAO
    {
        public static InteraccountTransaction FindById(int id)
        {
            int interAccId;
            DateTime transactionDate;
            int sourceAccountId;
            int targetAccountId;
            Decimal sourceAmount;
            Decimal targetAmount;
            string sourceCurrencyCode;
            string targetCurrencyCode;
            int rowVersion;
            InteraccountTransaction interaccountTransaction = null;
            NpgsqlConnection conn = DbConn.Instance.GetUsersConnection();
            NpgsqlCommand cmd = new NpgsqlCommand("select id, transaction_date, source_account_id," +
                                                        " target_account_id, source_amount, source_currency_code," +
                                                        " target_amount, target_currency_code, row_version" +
                                                        " from sb.interaccount_transactions where id = @id", conn);
            cmd.Parameters.AddWithValue("id", NpgsqlDbType.Integer, id);
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                interAccId = rdr.GetInt32(0);
                transactionDate = rdr.GetDateTime(1);
                sourceAccountId = rdr.GetInt32(2);
                targetAccountId = rdr.GetInt32(3);
                sourceAmount = rdr.GetDecimal(4);
                sourceCurrencyCode = rdr.GetString(5);
                targetAmount = rdr.GetDecimal(6);
                targetCurrencyCode = rdr.GetString(7);
                rowVersion = rdr.GetInt32(8);

                interaccountTransaction = new InteraccountTransaction { id = interAccId, 
                                          transaction_date = transactionDate, source_account_id = sourceAccountId,
                                          target_account_id = targetAccountId, source_amount = sourceAmount,
                                          source_currency_code = sourceCurrencyCode, target_amount = targetAmount,
                                          target_currency_code = targetCurrencyCode, row_version = rowVersion };
            }
            DbConn.Instance.FreeConnection(conn);
            return interaccountTransaction;
        }
    }
}
