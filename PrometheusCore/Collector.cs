using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace PrometheusCore
{
    public abstract class Collector : ICollector
    {
        private readonly SemaphoreSlim _lock;
        protected readonly byte[] _identifierBytes;

        public string Identifier { get; }
        public ILabels Labels { get; }
        public string Name { get; }

        private const string ValidCollectorNameExpression = "^[a-zA-Z_:][a-zA-Z0-9_:]*$";
        private static readonly Regex CollectorNameRegex = new Regex(ValidCollectorNameExpression, RegexOptions.Compiled);

        public Collector(string name, ILabels labels)
        {
            if (!CollectorNameRegex.IsMatch(name))
            {
                throw new ArgumentException($"Collector name '{name}' does not match regex '{ValidCollectorNameExpression}'.");
            }

            _lock = new SemaphoreSlim(2);
            Labels = labels;
            Name = name;
            Identifier = CollectorFactory.CreateIdentifier(name, labels);

            _identifierBytes = Constants.ExportEncoding.GetBytes(Identifier + " ");
        }
        protected void EnterReadLock()
        {
            _lock.Wait();
        }
        protected void ExitReadLock()
        {
            _lock.Release();
        }
        protected void EnterWriteLock()
        {
            _lock.Wait();
            _lock.Wait();
        }
        protected void ExitWriteLock()
        {
            _lock.Release(2);
        }
        public abstract Task SerializeAsync(Stream stream);
        protected async Task SerializePairAsync(Stream stream, byte[] identifierBytes, string value)
        {
            await stream.WriteAsync(identifierBytes, 0, identifierBytes.Length);

            var bytes = Constants.ExportEncoding.GetBytes(value);
            await stream.WriteAsync(bytes, 0, bytes.Length);

            stream.WriteByte(Constants.NewLineByte);
        }
        public override string ToString()
        {
            return $"{Name} [{Labels.ToString()}]";
        }
    }
}