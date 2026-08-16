using System;
using System.Globalization;
using PequenoExplorador.Domain.Content;

namespace PequenoExplorador.Domain.Progress
{
    public sealed class DiscoveryProgress
    {
        public DiscoveryProgress(DiscoveryId id, int count, string firstObservedLocalDate)
        {
            if (!id.IsValid) throw new ArgumentException("Discovery ID is invalid.", nameof(id));
            if (count < 1) throw new ArgumentOutOfRangeException(nameof(count));
            if (!IsOptionalLocalDate(firstObservedLocalDate))
                throw new ArgumentException("Local date must be empty or yyyy-MM-dd.", nameof(firstObservedLocalDate));

            Id = id;
            Count = count;
            FirstObservedLocalDate = firstObservedLocalDate ?? string.Empty;
        }

        public DiscoveryId Id { get; }
        public int Count { get; }
        public string FirstObservedLocalDate { get; }
        public bool IsNew => Count == 1;

        public DiscoveryProgress Increment() =>
            new DiscoveryProgress(Id, checked(Count + 1), FirstObservedLocalDate);

        private static bool IsOptionalLocalDate(string value)
        {
            if (string.IsNullOrEmpty(value)) return true;
            return DateTime.TryParseExact(
                value,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime parsed) &&
                string.Equals(parsed.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), value, StringComparison.Ordinal);
        }
    }
}
