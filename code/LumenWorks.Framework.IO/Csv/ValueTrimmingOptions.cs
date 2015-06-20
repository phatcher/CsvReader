using System;

namespace LumenWorks.Framework.IO.Csv
{
    /// <summary>
    /// Determines which values should be trimmed.
    /// </summary>
	[Flags]
	public enum ValueTrimmingOptions
	{
        /// <summary>
        /// Don't do any trimming
        /// </summary>
		None = 0,
        
        /// <summary>
        /// Only trim unquoted values
        /// </summary>
		UnquotedOnly = 1,

        /// <summary>
        /// Only trim quoted values
        /// </summary>
		QuotedOnly = 2,

        /// <summary>
        /// trim all values
        /// </summary>
		All = UnquotedOnly | QuotedOnly
	}
}