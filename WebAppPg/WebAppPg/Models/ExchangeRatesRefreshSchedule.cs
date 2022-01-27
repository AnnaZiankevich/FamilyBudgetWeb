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
    public class ExchangeRatesRefreshSchedule
    {
        public static async void Start()
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<ExchangeRates>().Build();

            ITrigger trigger = TriggerBuilder.Create()  
                .WithIdentity("trigger1", "group1")
                .StartAt(DateTime.UtcNow.AddSeconds(30))
                .WithSimpleSchedule(x => x
                     .WithIntervalInHours(24)
                     .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);

            Debug.WriteLine(">>>> Exchange Rate Uploading Task was scheduled >>>>>>>");


        }
    }
}
