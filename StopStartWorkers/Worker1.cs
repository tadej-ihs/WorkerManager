using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StopStartWorkers
{
    public class Worker1 : BackgroundService
    {
        private readonly IHostApplicationLifetime applicationLifetime;

        private Timer timer { get; set; }
        private int DisposeCounter { get; set; }
        private int LoopCounter { get; set; }

        public Worker1(IHostApplicationLifetime applicationLifetime)
        {
            timer = new Timer(new TimerCallback(DoOnTimer));
            this.applicationLifetime = applicationLifetime;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Trace.TraceWarning($"{ this.GetType().Name } started!");

            //applicationLifetime.ApplicationStopping.Register(this.Dispose);

            while (!stoppingToken.IsCancellationRequested)
            {

                LoopCounter++;
                Trace.TraceInformation($"Loop counter: {LoopCounter}");

                if (LoopCounter == 30)
                {
                    // not good => twice in the cleaning resources function if " applicationLifetime.ApplicationStopping.Register(this.Dispose); " present
                    // otherwise once and with exception
                    //StopStartWorkers Information: 0 : Loop counter: 29
                    //StopStartWorkers Information: 0 : Loop counter: 30
                    //StopStartWorkers Information: 0 : Loop counter: 30
                    //Exception thrown: 'System.Threading.Tasks.TaskCanceledException' in System.Private.CoreLib.dll
                    //StopStartWorkers Information: 0 : Worker1 CleanResources #1
                    //Microsoft.Hosting.Lifetime: Information: Application is shutting down...
                    //Exception thrown: 'System.Threading.Tasks.TaskCanceledException' in System.Private.CoreLib.dll
                    //StopStartWorkers Information: 0 : Worker1 CleanResources #2
                    //The thread 0x103c has exited with code 0(0x0).
                    //The program '[12692] iisexpress.exe' has exited with code 0(0x0).
                    // 
                    //applicationLifetime.StopApplication();

                    // not good => Exception thrown: 'System.Threading.Tasks.TaskCanceledException' in System.Private.CoreLib.dll
                    // https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca1816
                    //
                    // this.Dispose();


                    // not good => Exception thrown: 'System.Threading.Tasks.TaskCanceledException' in System.Private.CoreLib.dll
                    //
                    //await this.StopAsync(stoppingToken);
                    //this.StopAsync(stoppingToken);

                    // not good => Exception thrown: 'System.Threading.Tasks.TaskCanceledException' in System.Private.CoreLib.dll
                    // with stopasync override
                    //
                    //await this.StopAsync(stoppingToken);
                }
                await Task.Delay(100, stoppingToken);
            }

            // this never gets reached
            Trace.TraceWarning($"{ this.GetType().Name } closed!");
        }

        public override void Dispose()
        {

            //Trace.TraceWarning($"{ this.GetType().Name } disposing!");
            CleanResources();
            //GC.SuppressFinalize(this);
            base.Dispose();

        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            CleanResources();
            return base.StopAsync(cancellationToken);
        }

        private void CleanResources()
        {
            DisposeCounter++;
            Trace.TraceInformation($"{this.GetType().Name} CleanResources #{DisposeCounter}");
            timer.Dispose();
        }
        private void DoOnTimer(object state)
        {
            throw new NotImplementedException();
        }



    }
}
