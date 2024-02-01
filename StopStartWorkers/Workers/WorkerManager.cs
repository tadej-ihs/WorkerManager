using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Writers;
using StopStartWorkers.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StopStartWorkers.Workers
{
    public class WorkerManager : IDisposable
    {

        public int LoopCounter { get; set; }
        public string WorkerName { get; set; }



        private readonly IServiceProvider serviceProvider;

        //private List<object> workers { get; set; }
        private ConcurrentBag<object> workers { get; set; }
        private bool enableAutoRestart { get; set; }
        private CancellationTokenSource _loopCancellationTokenSource { get; set; }

        public WorkerManager(IServiceProvider _serviceProvider)
        {
            WorkerName = this.GetType().Name;
            serviceProvider = _serviceProvider;

            //workers = new List<object>();
            workers = new ConcurrentBag<object>();


            GetWorkersReferences();
            enableAutoRestart = true;

            _loopCancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => Loop(_loopCancellationTokenSource.Token));
        }


        protected async Task Loop(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                CheckIfAllWorkersAreAlive();


                await Task.Delay(1000);
            }
        }


        // ############################################################################################ 
        // ################################    PUBLIC   ############################################## 
        // ############################################################################################ 

        public object GetWorkersInfo(string filterByName)
        {
            if (string.IsNullOrEmpty(filterByName))
            {
                return workers;
            }

            var worker = workers
                .Where(x => ((IWorker)x).WorkerName == filterByName)
                .FirstOrDefault();

            return worker;
        }

        public async Task<string> StartWorkerAsync(string workerName)
        {
            if (string.IsNullOrEmpty(workerName))
            {
                return $"Worker name '{workerName}' not found!";
            }

            var appNamespace = this.GetType().Namespace;
            var workerType = Type.GetType(appNamespace + "." + workerName);
            if (workerType == null)
            {
                return $"Worker of type '{workerName}' doesn't exist!";
            }

            var worker = workers.Where(x => ((IWorker)x).WorkerName == workerName).FirstOrDefault();
            var workerCanncelled = ((IWorker)worker).CancellationToken.IsCancellationRequested;
            if (worker != null && workerCanncelled == false)
            {
                return "Worker already running!";
            }

            var w = serviceProvider.GetRequiredService(workerType);
            await ((IHostedService)w).StartAsync(new CancellationToken());



            return $"Worker name '{workerName}' started!";
        }

        public async Task<string> StopWorkerAsync(string workerName)
        {
            if (string.IsNullOrEmpty(workerName))
            {
                return $"Worker name '{workerName}' not found!";
            }

            var worker = workers.Where(x => ((IWorker)x).WorkerName == workerName).FirstOrDefault();
            if (worker == null)
            {
                return $"Worker named '{workerName}' is not even running!";
            }

           
            // todo - you should somehow dispose memory here
            // worker stays in list just cancellation token will be switched to new one at start
            var cancellToken = ((IWorker)worker).CancellationToken;
            await ((IHostedService)worker).StopAsync(cancellToken);
            await ((IWorker)worker).CleanResources();

            
            return $"Worker name '{workerName}' stopped!";
        }



        // ############################################################################################ 
        // ################################    PRIVATE   ############################################## 
        // ############################################################################################ 

        private void GetWorkersReferences()
        {
            using (var services = serviceProvider.CreateScope())
            {
                var hostedServices = services.ServiceProvider.GetServices<IHostedService>();
                var filterMyWorkers = hostedServices.Where(x => x.GetType().Namespace.Contains("StopStartWorkers")).ToList();

                foreach (var w in filterMyWorkers)
                {

                    workers.Add(w);

                }
            }



        }

        private async void CheckIfAllWorkersAreAlive()
        {
            if (enableAutoRestart)
            {
                // some workers are stopped and needs to be restarted
                foreach (var worker in workers)
                {
                    if (((IWorker)worker).CancellationToken.IsCancellationRequested)
                    {
                        var res = await StartWorkerAsync(((IWorker)worker).WorkerName);
                        Console.WriteLine(res);
                    }
                }
            }

            // it's ok
        }

        internal Task<string> EnableAutoRestart(bool enableAutoRestart)
        {
            this.enableAutoRestart = enableAutoRestart;
            return Task.FromResult($"enableAutoRestart: {enableAutoRestart}");
        }

        public void Dispose()
        {

            _loopCancellationTokenSource.Cancel();
            _loopCancellationTokenSource.Dispose();
        }
    }
}
