using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Writers;
using StopStartWorkers.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StopStartWorkers.Workers
{
    public class WorkerManager
    {

        public int LoopCounter { get; set; }
        public string WorkerName { get; set; }


        private List<object> workers { get; set; }
        private readonly IServiceProvider serviceProvider;

        public WorkerManager(IServiceProvider _serviceProvider)
        {
            WorkerName = this.GetType().Name;
            serviceProvider = _serviceProvider;
            workers = new List<object>();

            GetWorkersReferences();
        }

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

        public async Task<string> StartWorker(string workerName)
        {


            if (string.IsNullOrEmpty(workerName))
            {
                return $"Worker name '{workerName}' not found!";
            }

            var worker = workers.Where(x => ((IWorker)x).WorkerName == workerName).FirstOrDefault();
            if (worker != null)
            {
                return "Worker already running!";
            }

            var appNamespace = this.GetType().Namespace;
            var workerType = Type.GetType(appNamespace + "." + workerName);
            if (workerType == null)
            {
                return $"Worker of type '{workerName}' doesn't exist!";
            }

            var w = serviceProvider.GetRequiredService(workerType);
            workers.Add(w);
            await ((IHostedService)w).StartAsync(new CancellationToken());



            return $"Worker name '{workerName}' started!";
        }

        public async Task<string> StopWorker(string workerName)
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
            var cancellToken = ((IWorker)worker).CancellationToken;
            await ((IHostedService)worker).StopAsync(cancellToken);
            workers.Remove(worker);
            return $"Worker name '{workerName}' stopped!";
        }


        // ############################################################################################ 
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

        //public async Task StartWorker(IServiceProvider serviceProvider)
        //{
        //    // Stop the current service if it exists
        //    await StopCurrentServiceAsync();

        //    // Create a new cancellation token source
        //    cancellationTokenSource = new CancellationTokenSource();

        //    // Resolve and start the new hosted service
        //    currentService = serviceProvider.GetRequiredService<IHostedService>();
        //    await currentService.StartAsync(new CancellationToken());
        //}

        //public async Task StopCurrentServiceAsync()
        //{
        //    if (_currentService != null)
        //    {
        //        // Stop the current hosted service
        //        await _currentService.StopAsync(new CancellationToken());

        //        // Dispose the current hosted service if necessary
        //        if (_currentService is IDisposable disposableService)
        //        {
        //            disposableService.Dispose();
        //        }

        //        // Cancel the token source
        //        cancellationTokenSource?.Cancel();
        //        cancellationTokenSource?.Dispose();
        //    }
        //}

        //public static void AddWorkerCancelledToken(string workerName, CancellationToken cancellationToken)
        //{
        //    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        //    cancellationTokensList.Add(workerName, cancellationToken);
        //}

        //public static void GetWorkersCancelledTokens()
        //{
        //    var result = new List<object>();
        //    foreach (var worker in cancellationTokensList)
        //    {
        //        result.Add(new { workerName = worker.Key, cancellationToken = worker.Value });
        //    }
        //}

        //public static void StopWorker(string workerName)
        //{
        //    CancellationToken token;
        //    if (cancellationTokensList.TryGetValue(workerName, out token))
        //    {
        //        EdiWorker.
        //    }
        //}

    }
}
