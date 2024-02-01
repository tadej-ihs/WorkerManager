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
    public class WorkersController : Controller
    {
        private readonly WorkerManager workerManager;

        public WorkersController(WorkerManager workerManager)
        {
            this.workerManager = workerManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetWorkersInfo(string filterByName)
        {
            if (string.IsNullOrEmpty(filterByName))
            {
                return Ok(workerManager.GetWorkersInfo(""));
            }

            var res = await Task.Run(() => workerManager.GetWorkersInfo(filterByName));
            return Ok(res);
       
        }

        [HttpPost]
        public async Task<IActionResult> StartWorker(string workerName)
        {
            if (string.IsNullOrEmpty(workerName))
            {
                return BadRequest("Worker name not found!");
            }

            string res = await workerManager.StartWorkerAsync(workerName);
            return Ok(res);

        }

        [HttpPost]
        public async Task<IActionResult> StopWorker(string workerName)
        {
            if (string.IsNullOrEmpty(workerName))
            {
                return BadRequest("Worker name not found!");
            }

            string res = await workerManager.StopWorkerAsync(workerName);
            return Ok(res);
      
        }

        [HttpPost]
        public async Task<IActionResult> SetAutomaticWorkersRestart(bool enableAutoRestart)
        {
            string res = await workerManager.EnableAutoRestart(enableAutoRestart);

            return Ok(res);

        }



        // ############################################################################################ 
        // ################################    PRIVATE   ############################################## 
        // ############################################################################################ 

    }
}
