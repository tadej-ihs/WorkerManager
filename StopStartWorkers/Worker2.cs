using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StopStartWorkers
{
    public class Worker2 : BackgroundService
    {
        private readonly IHostApplicationLifetime applicationLifetime;

        private Timer timer { get; set; }
        private int DisposeCounter { get; set; }
        private int LoopCounter { get; set; }

        public Worker2(IHostApplicationLifetime applicationLifetime)
        {
            timer = new Timer(new TimerCallback(DoOnTimer));
            this.applicationLifetime = applicationLifetime;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Trace.TraceWarning($"{ this.GetType().Name } started!");

            applicationLifetime.ApplicationStopping.Register(this.Dispose);

            while (!stoppingToken.IsCancellationRequested)
            {
                LoopCounter++;
                Trace.TraceInformation($"Loop counter: {LoopCounter}");
                await Task.Delay(100, stoppingToken);
            }

            // never happens
            Trace.TraceWarning($"{ this.GetType().Name } closed!");
        }

        public override void Dispose()
        {
            CleanResources();
            base.Dispose();

        }

        private void CleanResources()
        {
            DisposeCounter++;
            Console.WriteLine($"{this.GetType().Name} CleanResources #{DisposeCounter}");
            timer.Dispose();
        }
        private void DoOnTimer(object state)
        {
            throw new NotImplementedException();
        }



    }
}
