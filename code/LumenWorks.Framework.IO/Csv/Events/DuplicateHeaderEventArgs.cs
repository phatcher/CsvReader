//	LumenWorks.Framework.IO.CSV.ParseErrorEventArgs
//	Copyright (c) 2006 Sébastien Lorion
//
//	MIT license (http://en.wikipedia.org/wiki/MIT_License)
//
//	Permission is hereby granted, free of charge, to any person obtaining a copy
//	of this software and associated documentation files (the "Software"), to deal
//	in the Software without restriction, including without limitation the rights 
//	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
//	of the Software, and to permit persons to whom the Software is furnished to do so, 
//	subject to the following conditions:
//
//	The above copyright notice and this permission notice shall be included in all 
//	copies or substantial portions of the Software.
//
//	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//	INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
//	PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
//	FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//	ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;

namespace LumenWorks.Framework.IO.Csv
{
	/// <summary>
	/// Provides data for the <see cref="M:CsvReader.OnDuplicateHeader"/> event.
	/// </summary>
	public class DuplicateHeaderEventArgs
    : EventArgs
	{
    #region Constructors

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

		#endregion

		#region Properties

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

	  #endregion
  }
}