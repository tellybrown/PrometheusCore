using System.Text;

namespace PrometheusCore
{
    public static class Constants
    {
        public const char NewLine = '\n';
        public const byte NewLineByte = (byte)NewLine;
        public const string ExporterContentType = "text/plain; version=0.0.4";
        public static readonly Encoding ExportEncoding = new UTF8Encoding(false);
    }
}
