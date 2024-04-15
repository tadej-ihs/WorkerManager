using System.Threading.Tasks;

namespace WorkerManager.Workers
{
    public interface IWorkerManager
    {
        Task<object> GetWorkersInfo(string filterByName);
        Task<string> StartWorkerAsync(string workerName);
        Task<string> StopWorkerAsync(string workerName);
        Task<string> EnableAutoRestart(bool enableAutoRestart);
        
    }
}