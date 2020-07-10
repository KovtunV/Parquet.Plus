using System;
using Parquet.Plus.Mapping.DifferentTypes.Default;

namespace Parquet.Plus.Mapping.DifferentTypes
{
    /// <summary>
    /// Default mappers
    /// </summary>
    public static class DefaultMappers
    {
        /// <summary>
        /// Map int column to bool property
        /// </summary>
        public static IDifferentTypesMapper<bool, int> IntToBool => new IntToBoolMapper();

        /// <summary>
        /// Map nullable int column to bool property
        /// </summary>
        public static IDifferentTypesMapper<bool, int?> NullableIntToBool => new NullableIntToBoolMapper();

        /// <summary>
        /// Map dateTimeOffset column to DateTime property, takes LocalDateTime value
        /// </summary>
        public static IDifferentTypesMapper<DateTime, DateTimeOffset> DateTimeOffsetToDateTime => new DateTimeOffsetToDateTimeMapper();

        /// <summary>
        /// Map nullable dateTimeOffset column to DateTime property, takes LocalDateTime value
        /// </summary>
        public static IDifferentTypesMapper<DateTime, DateTimeOffset?> NullableDateTimeOffsetToDateTime => new NullableDateTimeOffsetToDateTimeMapper();
    }
}
