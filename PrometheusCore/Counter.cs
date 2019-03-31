using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace PrometheusCore
{
    public class Counter : Collector, ICounter
    {
        private double _value;

        public Counter(string name, ILabels labels) : base(name, labels)
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
            if (increment < 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(increment), "Counter value cannot decrease.");
            }

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
    }
}