using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Npgsql;
using NpgsqlTypes;

namespace WebAppPg.Models
{
    public static class AccOwnerDAO
    {
        public static AccountOwner FindById(int id)
        {
            int accOwnId;
            string accOwnName;
            bool accOwnIsActive;
            int accOwnRowVersion;
            int accOwnAppUserId;
            AccountOwner accOwn = null;
            NpgsqlConnection conn = DbConn.Instance.GetUsersConnection();
            NpgsqlCommand cmd = new NpgsqlCommand("select id, name, is_active, row_version, app_user_id from sb.account_owners where id = @id", conn);
            cmd.Parameters.AddWithValue("id", NpgsqlDbType.Integer, id);
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                accOwnId = rdr.GetInt32(0);
                accOwnName = rdr.GetString(1);
                accOwnIsActive = rdr.GetBoolean(2);
                accOwnRowVersion = rdr.GetInt32(3);
                if (!rdr.IsDBNull(4))
                    accOwnAppUserId = rdr.GetInt32(4);
                else
                    accOwnAppUserId = -1;
                accOwn = new AccountOwner { id = accOwnId, name = accOwnName, is_active = accOwnIsActive, row_version = accOwnRowVersion, app_user_id = accOwnAppUserId };
            }
            DbConn.Instance.FreeConnection(conn);
            return accOwn;
        }
    }
}
