using System.Collections.Generic;
using System.Threading.Tasks;
using WorkerManager.Models;

namespace WorkerManager.Workers
{
    public interface IWorkerManager
    {
        //Task<object> GetWorkersInfo(string filterByName);
        //Task<string> StartWorkerAsync(string workerName);
        //Task<string> StopWorkerAsync(string workerName);
        //Task<string> EnableAutoRestart(bool enableAutoRestart);


        Task<List<IWorker>> GetWorkersInfo(string filterByName);
        Task<string> StartWorkerAsync(string workerName);
        Task<string> StopWorkerAsync(string workerName);
        Task<string> EnableAutoRestart(bool enableAutoRestart);

    }
}