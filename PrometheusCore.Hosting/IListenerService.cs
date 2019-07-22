using System.Threading;
using System.Threading.Tasks;

namespace PrometheusCore.Hosting
{
    public interface IListenerService
    {
        Task StartAsync(CancellationToken cancellationToken);
    }
}