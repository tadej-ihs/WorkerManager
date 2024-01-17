using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StopStartWorkers.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StopStartWorkers.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WorkersController : Controller
    {
        private List<object> workers { get; set; }
        public WorkersController(IServiceProvider serviceProvider)
        {
            workers = new List<object>();
            using (var services = serviceProvider.CreateScope())
            {
                var hostedServices = services.ServiceProvider.GetServices<IHostedService>();
                var filterMyWorkers = hostedServices.Where(x => x.GetType().Namespace.Contains("StopStartWorkers")).ToList();

                foreach (var w in filterMyWorkers)
                {
                    workers.Add((IWorker)w);
                }
            }
        }

        [HttpGet]
        public IActionResult GetWorkersInfo(string filterByName)
        {
            if (string.IsNullOrEmpty(filterByName))
            {
                return Ok(workers);
            }

            var worker = workers
                .Where(x => ((IWorker)x).WorkerName == filterByName)
                .FirstOrDefault();

            return Ok(worker);
        }

        [HttpPost]
        public IActionResult StartWorker(string workerName)
        {

            var worker = workers
                .Where(x => ((IWorker)x).WorkerName == workerName)
                .FirstOrDefault();
            if (worker != null)
            {
                ((IWorker)worker).StopWorker();
            }

            return Ok(worker);
        }

        [HttpPost]
        public IActionResult StopWorker(string workerName)
        {

            var worker = workers
                .Where(x => ((IWorker)x).WorkerName == workerName)
                .FirstOrDefault();
            if (worker != null)
            {
                ((IWorker)worker).StopWorker();
            }

            return Ok(worker);
        }



    }
}
