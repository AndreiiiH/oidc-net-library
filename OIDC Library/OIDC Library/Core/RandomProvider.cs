using System;
using System.Threading;

namespace ChaoticPixel.OIDC.Core
{
    internal static class RandomProvider
    {
        private static int _seed = Environment.TickCount;
        private static ThreadLocal<Random> _randomWrapper = new ThreadLocal<Random>(() =>
        {
            return new Random(Interlocked.Increment(ref _seed));
        });

        public static Random GetThreadRandom()
        {
            return _randomWrapper.Value;
        }
    }
}
