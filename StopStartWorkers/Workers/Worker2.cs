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
    public class Worker2 : BackgroundService, IWorker
    {

        private int _loopCounter;
        public int LoopCounter
        {
            get { return _loopCounter; }
            set
            { 
                _loopCounter = value;
                if(_loopCounter >= 1000)
                {
                    _loopCounter = 0;
                }
               
            }
        }
        public string WorkerName { get; set; }
        public CancellationToken CancellationToken { get; set; }


        public Worker2()
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
        }

        public async Task CleanResources()
        {
            this.Dispose();
            await Task.CompletedTask;
        }

        public override void Dispose()
        {
            LoopCounter = 0;
            base.Dispose();
        }
    }
}
