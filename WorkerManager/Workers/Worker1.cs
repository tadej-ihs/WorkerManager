using Microsoft.Extensions.Hosting;
using WorkerManager.Models;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace WorkerManager.Workers
{
    public class Worker1 : BackgroundService, IWorker
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

        public Worker1()
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
