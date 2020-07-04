using System;
using System.Collections.Generic;
using System.Text;

namespace Parquet.Plus.Events
{
    /// <summary>
    /// Invalid column arguments
    /// </summary>
    public class InvalidColumnEventArgs : EventArgs
    {
        /// <summary>
        /// The message with invalid column
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Invalid column arguments
        /// </summary>
        /// <param name="message">The message with invalid column</param>
        public InvalidColumnEventArgs(string message)
        {
            Message = message;
        }
    }
}
