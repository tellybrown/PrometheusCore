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
            if (increment < 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(increment), "Counter value cannot decrease.");
            }

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
    }
}