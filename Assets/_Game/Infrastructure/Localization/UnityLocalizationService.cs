using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Configuration;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Application.Logging;
using PequenoExplorador.Application.Save;
using PequenoExplorador.Domain.Progress;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace PequenoExplorador.Infrastructure.Localization
{
    public sealed class UnityLocalizationService : ILocalizationService
    {
        private static readonly string[] UserLocaleCodes =
        {
            LocaleCode.Spanish,
            LocaleCode.English
        };

        private readonly ISaveService _save;
        private readonly AutosaveCoordinator _checkpoints;
        private readonly IAppLogger _logger;
        private readonly bool _development;
        private bool _initialized;

        public UnityLocalizationService(
            ISaveService save,
            IAppLogger logger,
            BuildProfile profile,
            AutosaveCoordinator checkpoints = null)
        {
            _save = save ?? throw new ArgumentNullException(nameof(save));
            _checkpoints = checkpoints;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _development = profile == BuildProfile.Development;
        }

        public event Action<string> LocaleChanged;

        public string ServiceId => "Localization";
        public string CurrentLocaleCode { get; private set; } = LocaleCode.Spanish;
        public bool IsPseudoLocale => string.Equals(CurrentLocaleCode, LocaleCode.Pseudo, StringComparison.Ordinal);

        public IReadOnlyList<string> AvailableLocaleCodes => _development
            ? new[] { LocaleCode.Spanish, LocaleCode.English, LocaleCode.Pseudo }
            : UserLocaleCodes;

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            if (_initialized)
            {
                return;
            }

            cancellationToken.ThrowIfCancellationRequested();
            await LocalizationSettings.InitializationOperation.Task;
            cancellationToken.ThrowIfCancellationRequested();

            _initialized = true;
            string savedLocale = ToLocaleCode(_save.Current.Preferences.Language);
#if PE_LOCALIZATION_SMOKE_EN && (UNITY_EDITOR || PE_DEVELOPMENT_SERVICES)
            savedLocale = LocaleCode.English;
#endif
            try
            {
                await SetLocaleCoreAsync(savedLocale, persist: false, cancellationToken);
            }
            catch
            {
                _initialized = false;
                throw;
            }
            _logger.Write(new AppLogEntry(
                AppLogLevel.Info,
                "Localization",
                "Initialized",
                savedLocale));
        }

        public void Shutdown()
        {
            LocaleChanged = null;
            _initialized = false;
        }

        public string Resolve(LocalizedKey key, params object[] arguments)
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("Localization service must initialize before resolving text.");
            }

            try
            {
                var result = LocalizationSettings.StringDatabase.GetTableEntry(
                    key.Table,
                    key.Entry,
                    LocalizationSettings.SelectedLocale,
                    FallbackBehavior.DontUseFallback);
                if (result.Entry == null || string.IsNullOrEmpty(result.Entry.LocalizedValue))
                {
                    return Missing(key);
                }

                return result.Entry.GetLocalizedString(arguments ?? Array.Empty<object>());
            }
            catch (Exception exception)
            {
                _logger.Write(new AppLogEntry(
                    AppLogLevel.Error,
                    "Localization",
                    "ResolveFailed",
                    exception.GetType().Name));
                return Missing(key);
            }
        }

        public Task SetLocaleAsync(
            string localeCode,
            bool persist,
            CancellationToken cancellationToken)
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("Localization service must initialize before changing locale.");
            }

            return SetLocaleCoreAsync(localeCode, persist, cancellationToken);
        }

        private async Task SetLocaleCoreAsync(
            string localeCode,
            bool persist,
            CancellationToken cancellationToken)
        {
            if (!LocaleCode.IsSupported(localeCode, includePseudo: _development))
            {
                throw new ArgumentException("Unsupported or unsafe locale code.", nameof(localeCode));
            }

            if (persist && string.Equals(localeCode, LocaleCode.Pseudo, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Pseudo locale is Development-only and cannot be persisted.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            Locale locale = LocalizationSettings.AvailableLocales.Locales.FirstOrDefault(
                candidate => LocaleCodeFor(candidate) == localeCode);
            if (locale == null)
            {
                throw new InvalidOperationException("Configured locale is unavailable: " + localeCode);
            }

            string previousCode = CurrentLocaleCode;
            Locale previousLocale = LocalizationSettings.SelectedLocale;
            LocalizationSettings.SelectedLocale = locale;
            CurrentLocaleCode = localeCode;

            if (persist)
            {
                SaveOperationResult saveResult = _checkpoints == null
                    ? await SaveDirectlyAsync(localeCode, cancellationToken)
                    : await _checkpoints.UpdateAndFlushAsync(
                        progress => progress.WithPreferences(
                            progress.Preferences.WithLanguage(ToLanguagePreference(localeCode))),
                        cancellationToken);
                if (!saveResult.IsSuccess)
                {
                    LocalizationSettings.SelectedLocale = previousLocale;
                    CurrentLocaleCode = previousCode;
                    throw new InvalidOperationException("Locale preference could not be saved: " + saveResult.ErrorCode);
                }
            }

            LocaleChanged?.Invoke(CurrentLocaleCode);
        }

        private Task<SaveOperationResult> SaveDirectlyAsync(
            string localeCode,
            CancellationToken cancellationToken)
        {
            PlayerProgress updated = _save.Current.WithPreferences(
                _save.Current.Preferences.WithLanguage(ToLanguagePreference(localeCode)));
            return _save.SaveAsync(updated, cancellationToken);
        }

        private string Missing(LocalizedKey key)
        {
            _logger.Write(new AppLogEntry(
                AppLogLevel.Warning,
                "Localization",
                "MissingKey",
                key.ToString()));
            if (_development)
            {
                return "[missing " + key.Table + ":" + key.Entry + "]";
            }

            if (!key.Equals(LocalizationKeys.SafeFallback))
            {
                var fallback = LocalizationSettings.StringDatabase.GetTableEntry(
                    LocalizationKeys.SafeFallback.Table,
                    LocalizationKeys.SafeFallback.Entry,
                    LocalizationSettings.SelectedLocale,
                    FallbackBehavior.UseFallback);
                if (fallback.Entry != null && !string.IsNullOrEmpty(fallback.Entry.LocalizedValue))
                {
                    return fallback.Entry.GetLocalizedString();
                }
            }

            return "…";
        }

        private static string LocaleCodeFor(Locale locale)
        {
            return locale is UnityEngine.Localization.Pseudo.PseudoLocale
                ? LocaleCode.Pseudo
                : locale.Identifier.Code;
        }

        private static string ToLocaleCode(LanguagePreference preference)
        {
            return preference == LanguagePreference.English
                ? LocaleCode.English
                : LocaleCode.Spanish;
        }

        private static LanguagePreference ToLanguagePreference(string localeCode)
        {
            return string.Equals(localeCode, LocaleCode.English, StringComparison.Ordinal)
                ? LanguagePreference.English
                : LanguagePreference.Spanish;
        }
    }
}
