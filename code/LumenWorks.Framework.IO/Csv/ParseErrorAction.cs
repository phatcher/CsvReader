namespace LumenWorks.Framework.IO.Csv
{
    /// <summary>
    /// Specifies the action to take when a parsing error has occured.
    /// </summary>
    public enum ParseErrorAction
    {
        /// <summary>
        /// Raises the <see cref="M:CsvReader.ParseError"/> event.
        /// </summary>
        RaiseEvent = 0,

        /// <summary>
        /// Tries to advance to next line.
        /// </summary>
        AdvanceToNextLine = 1,

        /// <summary>
        /// Throws an exception.
        /// </summary>
        ThrowException = 2,
    }
}
