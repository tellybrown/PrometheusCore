using System.IO;
using System.Threading.Tasks;

namespace PrometheusCore
{
    public interface ICollector
    {
        string Identifier { get; }
        ILabels Labels { get; }
        Task SerializeAsync(Stream stream);
    }
}