using System;
using PequenoExplorador.Application.Services;

namespace PequenoExplorador.Infrastructure.Random
{
    public sealed class SeededRandomSource : IRandomSource
    {
        private readonly System.Random _random;

        public SeededRandomSource(int seed)
        {
            Seed = seed;
            _random = new System.Random(seed);
        }

        public int Seed { get; }

        public int Next(int maxExclusive)
        {
            if (maxExclusive <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxExclusive));
            }

            return _random.Next(maxExclusive);
        }
    }
}
