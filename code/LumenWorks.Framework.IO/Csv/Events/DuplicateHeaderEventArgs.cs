using System;

namespace LumenWorks.Framework.IO.Csv
{
    /// <summary>
    /// Provides data for the <see cref="M:CsvReader.OnDuplicateHeader"/> event.
    /// </summary>
    public class DuplicateHeaderEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the DuplicateHeaderEventArgs class.
        /// </summary>
        /// <param name="headerName">The name of the duplicate header.</param>
        /// <param name="index">The index of the duplicate header being added.</param>
        /// <param name="existingDuplicateIndex">The index of the duplicate header that is already in the Column collection.</param>
        public DuplicateHeaderEventArgs(string headerName, int index, int existingDuplicateIndex)
        {
            HeaderName = headerName;
            Index = index;
            ExistingDuplicateIndex = existingDuplicateIndex;
        }

        /// <summary>
        /// Name of the header that is a duplicate.
        /// </summary>
        /// <value>The header name.</value>
        public string HeaderName { get; set; }

        /// <summary>
        /// Index of the duplicate header being added
        /// </summary>
        /// <value>The column index</value>
        public int Index { get; }

        /// <summary>
        /// Index of the duplicate header that has already been added to the Column collection
        /// </summary>
        /// <value>The column index</value>
        public int ExistingDuplicateIndex { get; }
    }
}