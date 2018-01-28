using System;

namespace LumenWorks.Framework.IO.Csv
{
    /// <summary>
    /// Represents the exception that is thrown when a duplicate column header name encounter.
    /// </summary>
    public class DuplicateHeaderException : Exception
    {
        /// <summary>
        /// Contains the message that describes the error.
        /// </summary>
        private string _headerName;

        /// <summary>
        /// Contains the column index where the duplicate was found.
        /// </summary>
        private int _columnIndex;

        /// <summary>
        /// Initializes a new instance of the DuplicateHeaderException class.
        /// </summary>
        public DuplicateHeaderException(string headerName, int columnIndex) : base($"Duplicate header {headerName} encountered at column index {columnIndex}")
        {
            _headerName = headerName;
            _columnIndex = columnIndex;
        }

        /// <summary>
        /// Gets the HeaderName of the column with the duplicate.
        /// </summary>
        /// <value>The name of the column header.</value>
        public string HeaderName
        {
            get { return _headerName; }
        }

        /// <summary>
        /// Gets the column index where the duplicate was found.
        /// </summary>
        /// <value>The index of the column.</value>
        public int ColumnIndex
        {
            get { return _columnIndex; }
        }
    }
}