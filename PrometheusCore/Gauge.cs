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
                    _lock.EnterReadLock();
                    return _value;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }
        public void Inc(double increment = 1.0)
        {
            try
            {
                _lock.EnterWriteLock();
                _value += increment;
            }
            finally
            {
                _lock.ExitWriteLock();
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
                _lock.EnterWriteLock();
                _value = value;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }
}