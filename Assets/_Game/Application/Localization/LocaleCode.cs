using System;

namespace PequenoExplorador.Application.Localization
{
    public readonly struct LocaleCode : IEquatable<LocaleCode>
    {
        public const string Spanish = "es";
        public const string English = "en";
        public const string Pseudo = "qps-ploc";

        public LocaleCode(string value)
        {
            if (!IsSupported(value, includePseudo: true))
            {
                throw new ArgumentException("Locale code must be es, en or qps-ploc.", nameof(value));
            }

            Value = value;
        }

        public string Value { get; }
        public bool IsPseudo => string.Equals(Value, Pseudo, StringComparison.Ordinal);

        public static bool IsSupported(string value, bool includePseudo)
        {
            return string.Equals(value, Spanish, StringComparison.Ordinal) ||
                   string.Equals(value, English, StringComparison.Ordinal) ||
                   includePseudo && string.Equals(value, Pseudo, StringComparison.Ordinal);
        }

        public bool Equals(LocaleCode other) => string.Equals(Value, other.Value, StringComparison.Ordinal);
        public override bool Equals(object obj) => obj is LocaleCode other && Equals(other);
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value ?? string.Empty);
        public override string ToString() => Value ?? string.Empty;
        public static bool operator ==(LocaleCode left, LocaleCode right) => left.Equals(right);
        public static bool operator !=(LocaleCode left, LocaleCode right) => !left.Equals(right);
    }
}
