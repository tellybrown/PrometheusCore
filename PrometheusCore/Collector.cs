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
        protected readonly ReaderWriterLockSlim _lock;
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

            _lock = new ReaderWriterLockSlim();
            Labels = labels;
            Name = name;
            Identifier = CollectorFactory.CreateIdentifier(name, labels);

            _identifierBytes = Constants.ExportEncoding.GetBytes(Identifier + " ");
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