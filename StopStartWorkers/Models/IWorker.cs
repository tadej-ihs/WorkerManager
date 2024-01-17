using System.Threading;
using System.Threading.Tasks;

namespace StopStartWorkers.Models
{
    public interface IWorker
    {
        public int LoopCounter { get; set; }
        public string WorkerName { get; set; }

        //public CancellationToken CancellationToken { get; set; }

        Task StartWorker();
        Task StopWorker();
    }
}
