using System.Threading;
using System.Threading.Tasks;

namespace WorkerManager.Models
{
    public interface IWorker
    {
        public int LoopCounter { get; set; }
        public string WorkerName { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public Task CleanResources();
    }
}
