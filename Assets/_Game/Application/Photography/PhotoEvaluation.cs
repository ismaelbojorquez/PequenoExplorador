namespace PequenoExplorador.Application.Photography
{
    public readonly struct PhotoEvaluation
    {
        public PhotoEvaluation(bool isReady, PhotoGuidance guidance, int scorePermille)
        {
            IsReady = isReady;
            Guidance = guidance;
            ScorePermille = scorePermille;
        }

        public bool IsReady { get; }
        public PhotoGuidance Guidance { get; }
        public int ScorePermille { get; }
    }
}
