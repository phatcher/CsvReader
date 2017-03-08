//	LumenWorks.Framework.IO.Csv.MalformedCsvException
//	Copyright (c) 2005 Sébastien Lorion
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
  /// Represents the exception that is thrown when a duplicate column header name encounter.
  /// </summary>
  public class DuplicateHeaderException
    : Exception
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
    public DuplicateHeaderException(string headerName, int columnIndex)
      : base($"Duplicate header {headerName} encountered at column index {columnIndex}")
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