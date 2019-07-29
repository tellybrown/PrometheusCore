using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace PrometheusCore
{
    public class Gauge : Collector, IGauge
    {
        private double _value;

        public Gauge(string name, ILabels labels) : base(name, labels)
        {
            _value = 0;
        }

        public override async Task SerializeAsync(Stream stream)
        {
            await SerializePairAsync(stream, _identifierBytes, Value.ToString(CultureInfo.InvariantCulture));
        }
        public double Value
        {
            get
            {
                try
                {
                    EnterReadLock();
                    return _value;
                }
                finally
                {
                    ExitReadLock();
                }
            }
        }
        public void Inc(double increment = 1.0)
        {
            try
            {
                EnterWriteLock();
                _value += increment;
            }
            finally
            {
                ExitWriteLock();
            }
        }
        public void Dec(double decrement = 1.0)
        {
            Inc(-decrement);
        }
        public void Set(double value)
        {
            try
            {
                EnterWriteLock();
                _value = value;
            }
            finally
            {
                ExitWriteLock();
            }
        }
    }
}