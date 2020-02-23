using System;
using System.Diagnostics.CodeAnalysis;

namespace StringCountChar
{
    internal static class LocalHelper
    {
#pragma warning disable 0162
        [SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
        [SuppressMessage("ReSharper", "RedundantIfElseBlock")]
        public static long GetTotalIntegralMicroseconds(this TimeSpan value)
        {
            const long MicrosecondsPerMillisecond = 1000L;

            //// ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (TimeSpan.TicksPerMillisecond >= MicrosecondsPerMillisecond)
            {
                const long TicksPerMicrosecond = TimeSpan.TicksPerMillisecond / MicrosecondsPerMillisecond;
                return value.Ticks / TicksPerMicrosecond;
            }
            else
            {
                return value.Ticks / TimeSpan.TicksPerMillisecond * MicrosecondsPerMillisecond;
            }
#pragma warning restore 0162
        }
    }
}