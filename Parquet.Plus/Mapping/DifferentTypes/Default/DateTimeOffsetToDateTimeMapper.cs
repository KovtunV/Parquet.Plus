using System;

namespace Parquet.Plus.Mapping.DifferentTypes.Default
{
    internal class DateTimeOffsetToDateTimeMapper : IDifferentTypesMapper<DateTime, DateTimeOffset>
    {
        public DateTimeOffset ToColumn(DateTime item)
        {
            return new DateTimeOffset(item.ToUniversalTime());
        }

        public DateTime ToProperty(DateTimeOffset item)
        {
            return item.LocalDateTime;
        }
    }
}
