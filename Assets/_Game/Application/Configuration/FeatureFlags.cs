using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PequenoExplorador.Application.Configuration
{
    public sealed class FeatureFlags : IFeatureFlags
    {
        private readonly HashSet<FeatureFlag> _enabled;

        public FeatureFlags(IEnumerable<FeatureFlag> enabled)
        {
            FeatureFlag[] values = (enabled ?? Array.Empty<FeatureFlag>()).ToArray();
            if (values.Any(value => value == FeatureFlag.Unknown || !Enum.IsDefined(typeof(FeatureFlag), value)))
            {
                throw new ArgumentException("Feature flags must use a known non-zero stable ID.", nameof(enabled));
            }

            _enabled = new HashSet<FeatureFlag>(values);
            if (_enabled.Count != values.Length)
            {
                throw new ArgumentException("Feature flags must not contain duplicate IDs.", nameof(enabled));
            }

            Enabled = new ReadOnlyCollection<FeatureFlag>(
                _enabled.OrderBy(value => (int)value).ToArray());
        }

        public static FeatureFlags None { get; } = new FeatureFlags(Array.Empty<FeatureFlag>());

        public IReadOnlyList<FeatureFlag> Enabled { get; }

        public bool IsEnabled(FeatureFlag flag)
        {
            return _enabled.Contains(flag);
        }
    }
}
