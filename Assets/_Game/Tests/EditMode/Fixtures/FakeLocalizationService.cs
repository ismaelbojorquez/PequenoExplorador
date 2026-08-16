using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Localization;

namespace PequenoExplorador.Tests.EditMode.Fixtures
{
    internal sealed class FakeLocalizationService : ILocalizationService
    {
        private readonly IReadOnlyDictionary<string, string> _values;

        public FakeLocalizationService(IReadOnlyDictionary<string, string> values)
        {
            _values = values ?? throw new ArgumentNullException(nameof(values));
        }

        public event Action<string> LocaleChanged;

        public string ServiceId => "FakeLocalization";
        public string CurrentLocaleCode { get; private set; } = LocaleCode.Spanish;
        public IReadOnlyList<string> AvailableLocaleCodes { get; } =
            new[] { LocaleCode.Spanish, LocaleCode.English, LocaleCode.Pseudo };
        public bool IsPseudoLocale => CurrentLocaleCode == LocaleCode.Pseudo;

        public Task InitializeAsync(CancellationToken cancellationToken) => Task.CompletedTask;
        public void Shutdown() => LocaleChanged = null;

        public string Resolve(LocalizedKey key, params object[] arguments)
        {
            return _values.TryGetValue(key.ToString(), out string value)
                ? string.Format(value, arguments ?? Array.Empty<object>())
                : "[missing " + key + "]";
        }

        public Task SetLocaleAsync(string localeCode, bool persist, CancellationToken cancellationToken)
        {
            CurrentLocaleCode = localeCode;
            LocaleChanged?.Invoke(localeCode);
            return Task.CompletedTask;
        }
    }
}
