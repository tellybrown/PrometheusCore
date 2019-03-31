//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.IO;
//using System.Threading.Tasks;

//namespace PrometheusCore
//{
//    public class QuantileEpsilonPair
//    {
//        public QuantileEpsilonPair(double quantile, double epsilon)
//        {
//            Quantile = quantile;
//            Epsilon = epsilon;
//        }

//        public double Quantile { get; }
//        public double Epsilon { get; }
//    }

//    public interface ISummary
//    {

//    }
//    public class Summary : Collector, ISummary
//    {
//        private IList<Quantile> _objectives;
//        private double[] _sortedObjectives;
//        private double _sum;
//        private uint _count;
//        private SampleBuffer _hotBuf;
//        private SampleBuffer _coldBuf;
//        private QuantileStream[] _streams;
//        private TimeSpan _streamDuration;
//        private QuantileStream _headStream;
//        private int _headStreamIdx;
//        private DateTime _headStreamExpTime;
//        private DateTime _hotBufExpTime;
//        private byte[] _sumIdentifier;
//        private byte[] _countIdentifier;

//        private TimeSpan _maxAge;
//        private int _ageBuckets;
//        private int _bufCap;
//        private static readonly QuantileEpsilonPair[] DefObjectivesArray = new QuantileEpsilonPair[0];

//        public Summary(string name, Labels labels, double initialValue = 0, IList<QuantileEpsilonPair> objectives = null, TimeSpan? maxAge = null, int? ageBuckets = null, int? bufCap = null) : base(name, labels)
//        {
//            var initObjectives = objectives ?? DefObjectivesArray;

//            _objectives = new List<Quantile>();

//            _sumIdentifier = Constants.ExportEncoding.GetBytes(CollectorFactory.CreateIdentifier("sum"));
//            _countIdentifier = Constants.ExportEncoding.GetBytes(CollectorFactory.CreateIdentifier("count"));

//            for (var i = 0; i < _objectives.Count; i++)
//            {
//                var value = double.IsPositiveInfinity(initObjectives[i].Quantile) ? "+Inf" : initObjectives[i].Quantile.ToString(CultureInfo.InvariantCulture);

//                var identifier = CollectorFactory.CreateIdentifier(null, ("quantile", value);

//                _objectives.Add(new Quantile()
//                {
//                    Identifier = identifier,
//                    IdentifierBytes = Constants.ExportEncoding.GetBytes(identifier),
//                    Pair = new QuantileEpsilonPair()
//                    {

//                    }
//                });
//            }
//        }

//        public override async Task SerializeAsync(Stream stream)
//        {
//            // We output sum.
//            // We output count.
//            // We output quantiles.

//            var now = DateTime.UtcNow;

//            var values = new List<(double quantile, double value)>(_objectives.Count);

//            try
//            {
//                _lock.EnterReadLock();

//                // Swap bufs even if hotBuf is empty to set new hotBufExpTime.
//                SwapBufs(now);
//                FlushColdBuf();

//                for (var i = 0; i < _sortedObjectives.Length; i++)
//                {
//                    var quantile = _sortedObjectives[i];
//                    var value = _headStream.Count == 0 ? double.NaN : _headStream.Query(quantile);

//                    values.Add((quantile, value));
//                }

//                await SerializePairAsync(stream, _sumIdentifier, _sum.ToString());
//                await SerializePairAsync(stream, _countIdentifier, _count.ToString());

//                for (var i = 0; i < values.Count; i++)
//                {
//                    serializer.WriteMetric(_quantileIdentifiers[i], values[i].value);
//                }
//            }
//            finally
//            {
//                _lock.ExitReadLock();
//            }
//        }



//        public void Observe(double val)
//        {
//            Observe(val, DateTime.UtcNow);
//        }

//        // MaybeRotateStreams needs mtx AND bufMtx locked.
//        private void MaybeRotateStreams()
//        {
//            while (!_hotBufExpTime.Equals(_headStreamExpTime))
//            {
//                _headStream.Reset();
//                _headStreamIdx++;

//                if (_headStreamIdx >= _streams.Length)
//                {
//                    _headStreamIdx = 0;
//                }

//                _headStream = _streams[_headStreamIdx];
//                _headStreamExpTime = _headStreamExpTime.Add(_streamDuration);
//            }
//        }


//        private class Quantile
//        {
//            public string Identifier { get; set; }
//            public byte[] IdentifierBytes { get; set; }
//            public QuantileEpsilonPair Pair { get; set; }
//        }
//    }
//    public class QuantileCollection
//    {
//        private long _lastRotate;
//        private readonly long _duration;
//        private readonly List<CKMSQuantiles> _buckets;

//        public QuantileCollection(Quantile[] quantiles, long maxAgeSeconds, int ageBuckets)
//        {
//            _duration = maxAgeSeconds / TimeSpan.TicksPerSecond / ageBuckets;
//            _buckets = new List<CKMSQuantiles>(ageBuckets);
//        }
//        public void Insert(double value)
//        {
//            Rotate();
//            //foreach(var quantile in )
//        }
//        public double this[double quantile] => _buckets[_buckets.Count - 1].Value;
//        private void Rotate()
//        {
//            long ticks = DateTime.Now.Ticks;
//            double timeSinceLastRotate = ticks - _lastRotate;
//            if (timeSinceLastRotate > _duration)
//            {
//                _buckets.RemoveAt(0);
//                _buckets.Add(new CKMSQuantiles(quantiles));
//                _lastRotate = ticks;
//            }
//        }
//    }
//    public class CKMSQuantiles
//    {
//        private int _count = 0;
//        private int _compressIdx = 0;
//        protected List<Sample> _samples;
//        private double[] _buffer;
//        private int _bufferCount = 0;
//        private readonly Quantile[] _quantiles;

//        public CKMSQuantiles(Quantile[] quantiles)
//        {
//            _buffer = new double[500];
//            _quantiles = quantiles;
//            _samples = new List<Sample>();
//        }
//        public void Insert(double value)
//        {
//            _buffer[_bufferCount] = value;
//            _bufferCount++;

//            if (_bufferCount == _buffer.Length)
//            {
//                InsertBatch();
//                Compress();
//            }
//        }
//        private bool InsertBatch()
//        {
//            if (_bufferCount == 0)
//            {
//                return false;
//            }

//            //TODO

//            return true;
//        }
//        private void Compress()
//        {
//            if (_samples.Count < 2)
//            {
//                return;
//            }

//            //TODO
//        }
//    }
//    public class Sample
//    {
//        public double Value { get; }
//        public int G { get; set; }
//        public int Delta { get; }

//        public Sample(double value, int lower_delta, int delta)
//        {
//            Value = value;
//            G = lower_delta;
//            Delta = delta;
//        }

//        public override string ToString()
//        {
//            return $"I{{val={Value}, g={G}, del={Delta}}}";
//        }
//    }
//    public class Quantile
//    {
//        public readonly double _quantile;
//        public readonly double _error;
//        public readonly double _u;
//        public readonly double _v;

//        public Quantile(double quantile, double error)
//        {
//            _quantile = quantile;
//            _error = error;
//            _u = 2.0 * error / (1.0 - quantile);
//            _v = 2.0 * error / quantile;
//        }

//        public override string ToString()
//        {
//            return $"Q{{q={_quantile}, eps={_error}}}";
//        }
//    }
//}


