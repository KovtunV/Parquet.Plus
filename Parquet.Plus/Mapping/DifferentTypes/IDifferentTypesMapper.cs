namespace Parquet.Plus.Mapping.DifferentTypes
{
    /// <summary>
    /// Interface for type mappers
    /// </summary>
    /// <typeparam name="TProperty">Type of model's proeprty</typeparam>
    /// <typeparam name="TColumn">Type of column</typeparam>
    public interface IDifferentTypesMapper<TProperty, TColumn>
    {
        /// <summary>
        /// Converts a propertyItem to a columnItem
        /// </summary>
        /// <param name="item">item to convert</param>
        /// <returns>TColumn item</returns>
        TColumn ToColumn(TProperty item);

        /// <summary>
        /// Converts a columnItem to a propertyItem
        /// </summary>
        /// <param name="item">item to convert</param>
        /// <returns>TProperty item</returns>
        TProperty ToProperty(TColumn item);
    }
}
