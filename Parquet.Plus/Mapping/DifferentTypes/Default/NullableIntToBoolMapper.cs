namespace Parquet.Plus.Mapping.DifferentTypes.Default
{
    internal class NullableIntToBoolMapper : IDifferentTypesMapper<bool, int?>
    {
        public int? ToColumn(bool item)
        {
            return item ? 1 : 0;
        }

        public bool ToProperty(int? item)
        {
            return item.GetValueOrDefault(0) != 0;
        }
    }
}
