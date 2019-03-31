using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PrometheusCore
{
    public class Histogram : Collector, IHistogram
    {
        private readonly byte[] _sumIdentifier;
        private readonly byte[] _countIdentifier;
        private readonly Bucket[] _buckets;
        private double _sum;
        private long _count;

        public Histogram(string name, ILabels labels, double[] buckets = null) : base(name, labels)
        {
            var initBuckets = buckets ?? Buckets.Default;

            if (labels?.Any(l => l.Name == "le") == true)
            {
                throw new ArgumentException("'le' is a reserved label name");
            }
            if (initBuckets.Length == 0)
            {
                throw new ArgumentException("Histogram must have at least one bucket");
            }

            if (!double.IsPositiveInfinity(initBuckets[initBuckets.Length - 1]))
            {
                initBuckets = initBuckets.Concat(new[] { double.PositiveInfinity }).ToArray();
            }
            for (int i = 1; i < initBuckets.Length; i++)
            {
                if (initBuckets[i] <= initBuckets[i - 1])
                {
                    throw new ArgumentException("Bucket values must be increasing");
                }
            }

            _buckets = new Bucket[initBuckets.Length];
            _sum = 0;
            _count = 0;

            _countIdentifier = Constants.ExportEncoding.GetBytes(CollectorFactory.CreateIdentifier(name, labels, "count") + " ");
            _sumIdentifier = Constants.ExportEncoding.GetBytes(CollectorFactory.CreateIdentifier(name, labels, "sum") + " ");


            for (var i = 0; i < initBuckets.Length; i++)
            {
                var value = double.IsPositiveInfinity(initBuckets[i]) ? "+Inf" : initBuckets[i].ToString(CultureInfo.InvariantCulture);

                var identifier = CollectorFactory.CreateIdentifier(name, labels, "bucket", ("le", value));

                _buckets[i] = new Bucket()
                {
                    Identifier = identifier,
                    IdentifierBytes = Constants.ExportEncoding.GetBytes(identifier + " "),
                    Count = 0,
                    LessThan = initBuckets[i]
                };
            }
        }

        public double Sum
        {
            get
            {
                try
                {
                    _lock.EnterReadLock();
                    return _sum;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }
        public long Count
        {
            get
            {
                try
                {
                    _lock.EnterReadLock();
                    return _count;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }
        public long this[int index]
        {
            get
            {
                try
                {
                    _lock.EnterReadLock();
                    return _buckets[index].Count;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }
        public int Length => _buckets.Length;
        public override async Task SerializeAsync(Stream stream)
        {
            try
            {
                _lock.EnterReadLock();

                for (int i = 0; i < _buckets.Length; i++)
                {
                    await SerializePairAsync(stream, _buckets[i].IdentifierBytes, _buckets[i].Count.ToString());
                }

                await SerializePairAsync(stream, _countIdentifier, _count.ToString());
                await SerializePairAsync(stream, _sumIdentifier, _sum.ToString());
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
        public void Observe(double amt)
        {
            if (double.IsNaN(amt))
            {
                return;
            }
            try
            {
                _lock.EnterWriteLock();

                for (int i = 0; i < _buckets.Length; i++)
                {
                    if (amt <= _buckets[i].LessThan)
                    {
                        _buckets[i].Count++;
                    }
                }
                _sum += amt;
                _count++;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
        private class Bucket
        {
            public string Identifier { get; set; }
            public byte[] IdentifierBytes { get; set; }
            public double LessThan { get; set; }
            public long Count { get; set; }
        }
    }
}
