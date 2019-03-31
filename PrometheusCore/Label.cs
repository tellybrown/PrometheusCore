using System;
using System.Text.RegularExpressions;

namespace PrometheusCore
{
    public class Label : ILabel
    {
        private const string ValidLabelNameExpression = "^[a-zA-Z_:][a-zA-Z0-9_:]*$";
        private const string ReservedLabelNameExpression = "^__.*$";

        private static readonly Regex LabelNameRegex = new Regex(ValidLabelNameExpression, RegexOptions.Compiled);
        private static readonly Regex ReservedLabelRegex = new Regex(ReservedLabelNameExpression, RegexOptions.Compiled);

        public Label(string name, string value)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value ?? throw new ArgumentNullException(nameof(value));

            if (!LabelNameRegex.IsMatch(name))
            {
                throw new ArgumentException($"Label name '{name}' does not match regex '{ValidLabelNameExpression}'.");
            }

            if (ReservedLabelRegex.IsMatch(name))
            {
                throw new ArgumentException($"Label name '{name}' is not valid - labels starting with double underscore are reserved!");
            }
        }
        public string Value { get; }
        public string Name { get; }

        private string Escape(string value) => value.Replace("\\", @"\\").Replace("\n", @"\n").Replace("\"", @"\""");

        public bool Equals(ILabel x, ILabel y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            else if (x == null || y == null)
            {
                return false;
            }
            else if (x.Name == y.Name && x.Value == y.Value)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int GetHashCode(ILabel obj)
        {
            return ToString().GetHashCode();
        }
        public override string ToString()
        {
            return $"{Name}=\"{Escape(Value)}\"";
        }
    }
}