using System;
using System.Threading;

namespace AndreiiiH.OIDC.Core
{
    internal static class RandomProvider
    {
        private static int _seed = Environment.TickCount;
        private static readonly ThreadLocal<Random> _randomWrapper = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref _seed)));

        public static Random GetThreadRandom()
        {
            return _randomWrapper.Value;
        }
    }
}
