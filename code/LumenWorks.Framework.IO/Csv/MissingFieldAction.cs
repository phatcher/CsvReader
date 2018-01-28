namespace LumenWorks.Framework.IO.Csv
{
    /// <summary>
    /// Specifies the action to take when a field is missing.
    /// </summary>
    public enum MissingFieldAction
    {
        /// <summary>
        /// Treat as a parsing error.
        /// </summary>
        ParseError = 0,

        /// <summary>
        /// Replaces by an empty value.
        /// </summary>
        ReplaceByEmpty = 1,

        /// <summary>
        /// Replaces by a null value (<see langword="null"/>).
        /// </summary>
        ReplaceByNull = 2,
    }
}