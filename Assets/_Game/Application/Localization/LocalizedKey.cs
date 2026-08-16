using System;

namespace PequenoExplorador.Application.Localization
{
    public readonly struct LocalizedKey : IEquatable<LocalizedKey>
    {
        public LocalizedKey(string table, string entry)
        {
            if (string.IsNullOrWhiteSpace(table))
            {
                throw new ArgumentException("Localization table is required.", nameof(table));
            }

            if (string.IsNullOrWhiteSpace(entry) || entry.IndexOf('.', StringComparison.Ordinal) <= 0)
            {
                throw new ArgumentException("Localization entry must be a namespaced stable key.", nameof(entry));
            }

            Table = table;
            Entry = entry;
        }

        public string Table { get; }
        public string Entry { get; }

        public bool Equals(LocalizedKey other)
        {
            return string.Equals(Table, other.Table, StringComparison.Ordinal) &&
                   string.Equals(Entry, other.Entry, StringComparison.Ordinal);
        }

        public override bool Equals(object obj) => obj is LocalizedKey other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Table, Entry);
        public override string ToString() => Table + ":" + Entry;
    }
}
