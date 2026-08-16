using System;

namespace PequenoExplorador.Domain.Content
{
    internal readonly struct ContentIdValue : IEquatable<ContentIdValue>
    {
        private ContentIdValue(string value) => Value = value;

        public string Value { get; }
        public bool IsValid => !string.IsNullOrEmpty(Value);

        public static bool TryCreate(string raw, string root, out ContentIdValue value)
        {
            value = default;
            if (string.IsNullOrWhiteSpace(raw) || string.IsNullOrWhiteSpace(root) ||
                !raw.StartsWith(root + ".", StringComparison.Ordinal) || raw.Length <= root.Length + 1)
            {
                return false;
            }

            bool previousSeparator = true;
            for (int index = 0; index < raw.Length; index++)
            {
                char character = raw[index];
                bool separator = character == '.' || character == '-';
                bool allowed = character >= 'a' && character <= 'z' ||
                               character >= '0' && character <= '9' || separator;
                if (!allowed || separator && previousSeparator) return false;
                previousSeparator = separator;
            }

            if (previousSeparator) return false;
            value = new ContentIdValue(raw);
            return true;
        }

        public bool Equals(ContentIdValue other) => string.Equals(Value, other.Value, StringComparison.Ordinal);
        public override bool Equals(object obj) => obj is ContentIdValue other && Equals(other);
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value ?? string.Empty);
        public override string ToString() => Value ?? string.Empty;
    }
}
