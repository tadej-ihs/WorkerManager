﻿using Microsoft.Extensions.Hosting;
using StopStartWorkers.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StopStartWorkers.Workers
{
    public class Worker1 : BackgroundService, IWorker
    {
        public int LoopCounter { get; set; }
        public string WorkerName { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public Worker1(IHostApplicationLifetime applicationLifetime)
        {
            WorkerName = GetType().Name;
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

        public async Task StartWorker()
        {
            //this.StartAsync(CancellationToken);
            await this.ExecuteAsync(new CancellationToken());
            
        }

        public async Task StopWorker()
        {
            this.StopAsync(CancellationToken);
            await Task.CompletedTask;
        }
    }
}