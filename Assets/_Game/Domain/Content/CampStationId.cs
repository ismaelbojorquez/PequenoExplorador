using System;

namespace PequenoExplorador.Domain.Content
{
    public readonly struct CampStationId : IEquatable<CampStationId>
    {
        private readonly ContentIdValue _value;
        private CampStationId(ContentIdValue value) => _value = value;
        public string Value => _value.ToString();
        public bool IsValid => _value.IsValid;
        public static bool TryParse(string raw, out CampStationId id)
        { bool ok = ContentIdValue.TryCreate(raw, "camp-station", out ContentIdValue value); id = ok ? new CampStationId(value) : default; return ok; }
        public static CampStationId Parse(string raw) => TryParse(raw, out CampStationId id)
            ? id
            : throw new FormatException("Camp station ID must be namespaced as camp-station.<scope>.");
        public bool Equals(CampStationId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is CampStationId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => Value;
    }
}
