using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PequenoExplorador.Application.Audio;
using PequenoExplorador.Application.Configuration;
using PequenoExplorador.Application.Localization;
using PequenoExplorador.Bootstrap;
using PequenoExplorador.Editor.BuildTools;
using PequenoExplorador.Infrastructure.Audio;
using PequenoExplorador.Tests.EditMode.Fixtures;

namespace PequenoExplorador.Tests.EditMode
{
    public sealed class AudioFrameworkTests
    {
        [Test]
        public void AuthoringCatalogMixerClipsAndAddressesAreValid()
        {
            Assert.That(AudioValidationService.Validate(), Is.Empty);
        }

        [TestCase(-0.01f)]
        [TestCase(1.01f)]
        [TestCase(float.NaN)]
        public void SettingsRejectNonNormalizedValues(float invalid)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AudioSettings(invalid, .5f, .5f, .5f, .5f, true));
        }

        [Test]
        public void VoiceQueuePrioritizesInterruptsThenQueuedOrderAndEnforcesCapacity()
        {
            var queue = new VoiceQueueScheduler(2);
            UnityAudioCue normalA = Cue("audio.test.a", AudioPriority.Normal);
            UnityAudioCue low = Cue("audio.test.low", AudioPriority.Low);
            UnityAudioCue normalB = Cue("audio.test.b", AudioPriority.Normal);
            UnityAudioCue critical = Cue("audio.test.critical", AudioPriority.Critical);

            Assert.That(queue.Offer(normalA), Is.EqualTo(VoiceOfferResult.Start));
            Assert.That(queue.Offer(low), Is.EqualTo(VoiceOfferResult.Queue));
            Assert.That(queue.Offer(normalB), Is.EqualTo(VoiceOfferResult.Queue));
            Assert.That(queue.Offer(Cue("audio.test.rejected", AudioPriority.Low)), Is.EqualTo(VoiceOfferResult.Queue));
            Assert.That(queue.PendingCount, Is.EqualTo(2));
            Assert.That(queue.Offer(critical), Is.EqualTo(VoiceOfferResult.Interrupt));
            Assert.That(queue.Current, Is.SameAs(critical));
            Assert.That(queue.CompleteAndTakeNext(), Is.SameAs(normalB));
        }

        [Test]
        public void CooldownUsesInjectedTimeAndReset()
        {
            double now = 10d;
            var tracker = new AudioCooldownTracker(() => now);
            AudioCueId cue = AudioCueIds.ConfirmFeedback;

            Assert.That(tracker.TryConsume(cue, 1f), Is.True);
            Assert.That(tracker.TryConsume(cue, 1f), Is.False);
            now += 1d;
            Assert.That(tracker.TryConsume(cue, 1f), Is.True);
            tracker.Clear();
            Assert.That(tracker.TryConsume(cue, 1f), Is.True);
        }

        [Test]
        public async Task HeadlessSettingsPersistInSaveAndMissingCueNeverBlocks()
        {
            var store = new InMemoryFileStore();
            using (var first = new ServiceRegistry(AppConfigDefaults.Create(BuildProfile.Development), store))
            {
                await first.Host.InitializeAsync(CancellationToken.None);
                var expected = new AudioSettings(.7f, .2f, .3f, .4f, .5f, false);
                await first.Context.Audio.UpdateSettingsAsync(expected, CancellationToken.None);
                Assert.That(first.Context.Audio.Play(new AudioCueId("audio.missing.test")).Status, Is.EqualTo(AudioPlayStatus.Missing));
            }

            using var second = new ServiceRegistry(AppConfigDefaults.Create(BuildProfile.Development), store);
            await second.Host.InitializeAsync(CancellationToken.None);
            Assert.That(second.Context.Audio.Settings, Is.EqualTo(new AudioSettings(.7f, .2f, .3f, .4f, .5f, false)));
        }

        private static UnityAudioCue Cue(string id, AudioPriority priority)
        {
            return new UnityAudioCue(
                new AudioCueId(id), AudioCueCategory.VoiceInstruction, AudioBus.Voice, priority, 0f, .25f, false,
                new LocalizedKey(LocalizationKeys.ContentTable, "content.audio.instruction.explore"), true, null, null, true);
        }
    }
}
