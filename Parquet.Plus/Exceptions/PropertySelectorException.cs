using System;

namespace Parquet.Plus.Exceptions
{
    /// <summary>
    /// Exception when I'm trying make model's property setter/getter
    /// </summary>
    public class PropertySelectorException : Exception
    {
        /// <summary>
        /// Exception when I'm trying make model's property setter/getter
        /// </summary>
        public PropertySelectorException() : base("The property must be selected")
        {
            
        }
    }
}
