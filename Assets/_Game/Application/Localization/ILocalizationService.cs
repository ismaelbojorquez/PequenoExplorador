using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Lifecycle;

namespace PequenoExplorador.Application.Localization
{
    public interface ILocalizationService : IApplicationService
    {
        event Action<string> LocaleChanged;

        string CurrentLocaleCode { get; }
        IReadOnlyList<string> AvailableLocaleCodes { get; }
        bool IsPseudoLocale { get; }

        string Resolve(LocalizedKey key, params object[] arguments);
        Task SetLocaleAsync(string localeCode, bool persist, CancellationToken cancellationToken);
    }
}
