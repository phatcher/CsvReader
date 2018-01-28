using System;

namespace LumenWorks.Framework.IO.Csv
{
    public partial class CsvReader
    {
        /// <summary>
        /// Defines the data reader validations.
        /// </summary>
        [Flags]
        private enum DataReaderValidations
        {
            /// <summary>
            /// No validation.
            /// </summary>
            None = 0,

            /// <summary>
            /// Validate that the data reader is initialized.
            /// </summary>
            IsInitialized = 1,

            /// <summary>
            /// Validate that the data reader is not closed.
            /// </summary>
            IsNotClosed = 2
        }
    }
}