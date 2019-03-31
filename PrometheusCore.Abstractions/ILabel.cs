using System.Collections.Generic;

namespace PrometheusCore
{
    public interface ILabel : IEqualityComparer<ILabel>
    {
        string Value { get; }
        string Name { get; }
    }
}