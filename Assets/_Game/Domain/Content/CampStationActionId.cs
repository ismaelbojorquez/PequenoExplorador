using System;

namespace PequenoExplorador.Domain.Content
{
    public readonly struct CampStationActionId : IEquatable<CampStationActionId>
    {
        private readonly ContentIdValue _value;
        private CampStationActionId(ContentIdValue value) => _value = value;
        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;
        public static bool TryParse(string raw, out CampStationActionId id)
        { bool ok = ContentIdValue.TryCreate(raw, "camp-action", out ContentIdValue value); id = ok ? new CampStationActionId(value) : default; return ok; }
        public static CampStationActionId Parse(string raw) => TryParse(raw, out CampStationActionId id)
            ? id
            : throw new FormatException("Camp action ID must be namespaced as camp-action.<scope>.");
        public bool Equals(CampStationActionId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is CampStationActionId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
    }
}
