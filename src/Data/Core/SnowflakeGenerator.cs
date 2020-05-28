using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace NetBooru.Data
{
    /// <summary>
    /// Value generator for snowflakes
    /// </summary>
    public class SnowflakeGenerator : ValueGenerator<ulong>
    {
        private static readonly Random Random = new Random();

        private readonly byte _workerId;

        /// <inheritdoc/>
        public SnowflakeGenerator()
            : base()
        {
            _workerId = (byte)Random.Next(byte.MaxValue);
        }

        /// <inheritdoc/>
        public override bool GeneratesTemporaryValues => false;

        /// <inheritdoc/>
        public override ulong Next(EntityEntry entry)
        {
            return SnowflakeHelper.GenerateSnowflake(
                DateTimeOffset.UtcNow, _workerId);
        }
    }
}
