using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StopStartWorkers.Models;
using StopStartWorkers.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StopStartWorkers.Controllers
{
 
    [ApiController]
    [Route("[controller]/[action]")]
    public class WorkersController : Controller, IWorkerManager
    {
        private readonly IWorkerManager workerManager;

        public WorkersController(IWorkerManager workerManager)
        {
            this.workerManager = workerManager;
        }

       
        [HttpGet]
        public async Task<object> GetWorkersInfo(string filterByName)
        {
            return await workerManager.GetWorkersInfo(filterByName);

        }

        [HttpPost]
        public async Task<string> StartWorkerAsync(string workerName)
        {
            return await workerManager.StartWorkerAsync(workerName);
        }

        [HttpPost]
        public async Task<string> StopWorkerAsync(string workerName)
        {
            return await workerManager.StopWorkerAsync(workerName);
        }

        [HttpPost]
        public Task<string> EnableAutoRestart(bool enableAutoRestart)
        {
            return workerManager.EnableAutoRestart(enableAutoRestart);
        }
    }
}
