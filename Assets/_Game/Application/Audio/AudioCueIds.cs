namespace PequenoExplorador.Application.Audio
{
    public static class AudioCueIds
    {
        public static readonly AudioCueId CampMusic = new AudioCueId("audio.music.camp");
        public static readonly AudioCueId CampAmbience = new AudioCueId("audio.ambience.camp");
        public static readonly AudioCueId ConfirmFeedback = new AudioCueId("audio.feedback.confirm");
        public static readonly AudioCueId RetryFeedback = new AudioCueId("audio.feedback.retry");
        public static readonly AudioCueId ExploreInstruction = new AudioCueId("audio.voice.instruction.explore");
        public static readonly AudioCueId JungleName = new AudioCueId("audio.voice.name.jungle");
        public static readonly AudioCueId WelcomeNarration = new AudioCueId("audio.voice.narration.welcome");
    }
}
