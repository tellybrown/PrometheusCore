using System;

namespace PrometheusCore
{
    public class Background : IBackground
    {
        public Background(Type type)
        {
            Type = type;
        }
        public Type Type { get; }
    }
}
