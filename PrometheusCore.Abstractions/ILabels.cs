using System.Collections.Generic;

namespace PrometheusCore
{
    public interface ILabels : IList<ILabel>, IEqualityComparer<ILabels>
    {
        string Serialize();
    }
}