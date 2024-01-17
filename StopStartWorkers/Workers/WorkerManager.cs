using Microsoft.Extensions.Hosting;
using StopStartWorkers.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StopStartWorkers.Workers
{
    public class WorkerManager : BackgroundService
    {
        public int LoopCounter { get; set; }
        public string WorkerName { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public WorkerManager(IHostApplicationLifetime applicationLifetime)
        {
            WorkerName = this.GetType().Name;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Trace.TraceWarning($"{WorkerName} started!");

            CancellationToken = stoppingToken;

            while (!CancellationToken.IsCancellationRequested)
            {

                LoopCounter++;
                Trace.TraceInformation($"Loop counter of {WorkerName}: {LoopCounter}");

               
                await Task.Delay(100, stoppingToken);
            }

            // this never gets reached
            Trace.TraceWarning($"{WorkerName} closed!");
        }

   

      
    }
}
