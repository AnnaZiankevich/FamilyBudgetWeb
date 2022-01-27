using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Npgsql;
using NpgsqlTypes;
using Quartz;
using Quartz.Impl;
using System.Diagnostics;

namespace WebAppPg.Models
{
    public class ExchangeRates : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Debug.WriteLine("================== Exchange Rate Uploading Started =========================");
            DataTable datesDataTable = new DataTable();
            datesDataTable.Clear();

            NpgsqlConnection conn = DbConn.Instance.GetUsersConnection();
            datesDataTable.Load(new NpgsqlCommand("select dd::date from sb.app_settings s, " +
                  "lateral generate_series(s.app_life_start_date, now()::date, interval '1 day') as dd " +
                  "where not exists(select 1 from sbudget.exchange_rates where rate_date = dd::date and currency_to = 'BYN')",
                  conn).ExecuteReader());
            Debug.WriteLine("****************** DataTable Loaded ********************");
            foreach (DataRow r in datesDataTable.Rows)
            {
                string URL = "https://www.nbrb.by/api/exrates/rates/USD?parammode=2&ondate=" + ((DateTime)r["dd"]).ToString("yyyy-MM-dd");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                Debug.WriteLine("<<<<<<<<<<<<<<<<<<< URL created >>>>>>>>>>>>>>>>>>>>>");
                request.Method = "GET";
                request.Accept = "application/json";
                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    string content = new StreamReader(response.GetResponseStream()).ReadToEnd();

                    NpgsqlCommand callCmd = new NpgsqlCommand();
                    callCmd.CommandText = "call sb.exchange_rate_create_or_update(_currency_to => :curr, _rate_info => :info)";

                    callCmd.Parameters.AddWithValue("curr", NpgsqlDbType.Varchar, "BYN");
                    callCmd.Parameters.AddWithValue("info", NpgsqlDbType.Json, content);
                    callCmd.Connection = conn;
                    callCmd.ExecuteNonQuery();
                    Debug.WriteLine("%%%%%%%%%%%%%%% Procedure executed %%%%%%%%%%%%%%%%%%%%");
                }
                catch (WebException ex)
                {
                    throw new WebException("Unexpected error occured: ", ex);
                }
            }

            return Task.CompletedTask;
        }
    }
}
