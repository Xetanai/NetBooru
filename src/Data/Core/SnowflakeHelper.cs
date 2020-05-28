using System;
using System.Threading;

namespace NetBooru.Data
{
    /// <summary>
    /// Helper class for generating always-unique IDs across the platform.
    /// </summary>
    public static class SnowflakeHelper
    {
        private static int Generation;

        private static readonly DateTimeOffset Epoch
            = new DateTimeOffset(
                year: 2020, month: 1, day: 1,
                hour: 0, minute: 0, second: 0,
                offset: TimeSpan.Zero);
        private static readonly uint EpochSeconds
            = (uint)Epoch.ToUnixTimeSeconds();

        /// <summary>
        /// Generates a unique ulong ID.
        /// </summary>
        /// <param name="time">DateOffsetTime to use.</param>
        /// <param name="worker">Worder ID to use. Defaults to 0/</param>
        /// <returns></returns>
        public static ulong GenerateSnowflake(DateTimeOffset time,
            byte worker = 0)
        {
            if (time.Year < Epoch.Year)
                throw new ArgumentOutOfRangeException(nameof(time));
            if ((worker & Bits.Worker) != worker)
                throw new ArgumentOutOfRangeException(nameof(worker));

            var generation = unchecked((ushort)Interlocked
                .Increment(ref Generation));

            var timestamp = (ulong)time.ToUnixTimeMilliseconds() - EpochSeconds;

            return ((uint)timestamp << Shifts.Timestamp)
                     | ((ulong)worker << Shifts.Worker)
                     | ((ulong)generation << Shifts.Generation);
        }

        /// <summary>
        /// Gets the creation time of a snowflake.
        /// </summary>
        /// <param name="snowflake">The snowflake ID in question.</param>
        /// <returns></returns>
        public static DateTimeOffset GetCreationTime(ulong snowflake)
        {
            var timestamp = unchecked(
                (uint)((snowflake >> Shifts.Timestamp) & Bits.Timestamp));
            return DateTimeOffset.FromUnixTimeMilliseconds(
                timestamp + EpochSeconds);
        }

        private static class Bits
        {
            public const ulong Timestamp = 0x3FFFFFFFFFF;
            public const ulong Worker = 0x3_FF;
            public const ulong Generation = 0xFFF;
        }

        private static class Shifts
        {
            public const int Timestamp = 0;
            public const int Worker = 42;
            public const int Generation = 52;
        }
    }
}
