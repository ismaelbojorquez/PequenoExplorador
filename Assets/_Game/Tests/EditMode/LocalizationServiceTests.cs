using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PequenoExplorador.Application.Configuration;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Domain.Progress;
using PequenoExplorador.Editor.BuildTools;
using PequenoExplorador.Infrastructure.Localization;
using PequenoExplorador.Infrastructure.Save;
using PequenoExplorador.Tests.EditMode.Fixtures;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class LocalizationServiceTests
    {
        [Test]
        public async Task SpanishDefaultsAndSmartStringsResolveThroughPackage()
        {
            LocalSaveService save = await CreateSaveAsync();
            var service = new UnityLocalizationService(
                save,
                new RecordingLogger(),
                BuildProfile.Development);

            await service.InitializeAsync(CancellationToken.None);

            Assert.That(service.CurrentLocaleCode, Is.EqualTo(LocaleCode.Spanish));
            Assert.That(service.Resolve(LocalizationKeys.ProductName), Does.Contain("Pequeño Explorador"));
            Assert.That(service.Resolve(LocalizationKeys.Version, "9.8.7"), Is.EqualTo("Versión 9.8.7"));
            Assert.That(service.Resolve(LocalizationKeys.StarsCount, 1), Is.EqualTo("Una estrella"));
            Assert.That(service.Resolve(LocalizationKeys.StarsCount, 3), Is.EqualTo("3 estrellas"));
        }

        [Test]
        public async Task EnglishChangePersistsAndRestoresWithoutPlayerPrefs()
        {
            LocalSaveService save = await CreateSaveAsync();
            var first = new UnityLocalizationService(
                save,
                new RecordingLogger(),
                BuildProfile.Development);
            await first.InitializeAsync(CancellationToken.None);

            await first.SetLocaleAsync(LocaleCode.English, persist: true, CancellationToken.None);

            Assert.That(first.Resolve(LocalizationKeys.StatusReady), Is.EqualTo("Ready"));
            Assert.That(save.Current.Preferences.Language, Is.EqualTo(LanguagePreference.English));
            first.Shutdown();

            var restored = new UnityLocalizationService(
                save,
                new RecordingLogger(),
                BuildProfile.Development);
            await restored.InitializeAsync(CancellationToken.None);

            Assert.That(restored.CurrentLocaleCode, Is.EqualTo(LocaleCode.English));
            Assert.That(restored.Resolve(LocalizationKeys.WorldCamp), Is.EqualTo("Camp"));
        }

        [Test]
        public async Task MissingKeyIsDiagnosticInDevelopmentAndSafeInRelease()
        {
            LocalSaveService save = await CreateSaveAsync();
            var development = new UnityLocalizationService(
                save,
                new RecordingLogger(),
                BuildProfile.Development);
            await development.InitializeAsync(CancellationToken.None);
            var missing = new LocalizedKey(LocalizationKeys.UiTable, "ui.fixture.missing");

            Assert.That(development.Resolve(missing), Does.StartWith("[missing UI:ui.fixture.missing]"));
            development.Shutdown();

            var release = new UnityLocalizationService(
                save,
                new RecordingLogger(),
                BuildProfile.Release);
            await release.InitializeAsync(CancellationToken.None);

            Assert.That(release.Resolve(missing), Is.EqualTo("Algo no salió como esperábamos."));
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await release.SetLocaleAsync(LocaleCode.Pseudo, persist: false, CancellationToken.None));
        }

        [Test]
        public async Task PseudoExpandsAndCannotBePersisted()
        {
            LocalSaveService save = await CreateSaveAsync();
            var service = new UnityLocalizationService(
                save,
                new RecordingLogger(),
                BuildProfile.Development);
            await service.InitializeAsync(CancellationToken.None);

            string spanish = service.Resolve(LocalizationKeys.ActionReturnCamp);
            await service.SetLocaleAsync(LocaleCode.Pseudo, persist: false, CancellationToken.None);
            string pseudo = service.Resolve(LocalizationKeys.ActionReturnCamp);

            Assert.That(service.IsPseudoLocale, Is.True);
            Assert.That(pseudo.Length, Is.GreaterThan(spanish.Length));
            Assert.That(pseudo, Is.Not.EqualTo(spanish));
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await service.SetLocaleAsync(LocaleCode.Pseudo, persist: true, CancellationToken.None));
            Assert.That(save.Current.Preferences.Language, Is.EqualTo(LanguagePreference.Spanish));
        }

        [Test]
        public void LocalizationAuthoringHasNoMissingKeysOrGlyphs()
        {
            Assert.That(LocalizationValidationService.Validate(), Is.Empty);
        }

        private static async Task<LocalSaveService> CreateSaveAsync()
        {
            var save = new LocalSaveService(
                new InMemoryFileStore(),
                "0.1.0-test",
                new RecordingLogger(),
                new ISaveMigration[]
                {
                    new LegacyV0ToV1Migration(),
                    new V1ToV2LocalizationMigration(),
                    new V2ToV3AudioMigration()
                });
            await save.InitializeAsync(CancellationToken.None);
            return save;
        }
    }
}
