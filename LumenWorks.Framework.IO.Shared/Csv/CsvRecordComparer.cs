//	LumenWorks.Framework.IO.CSV.CachedCsvReader.CsvRecordComparer
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
using System.Collections.Generic;
using System.ComponentModel;
using Debug = System.Diagnostics.Debug;
using System.Globalization;

namespace LumenWorks.Framework.IO.Csv
{
    /// <summary>
    /// Represents a CSV record comparer.
    /// </summary>
    public class CsvRecordComparer : IComparer<string[]>
    {
        /// <summary>
        /// Initializes a new instance of the CsvRecordComparer class.
        /// </summary>
        /// <param name="field">The field index of the values to compare.</param>
        /// <param name="direction">The sort direction.</param>
        public CsvRecordComparer(int field, ListSortDirection direction)
        {
            if (field < 0)
            {
                throw new ArgumentOutOfRangeException("field", field, string.Format(CultureInfo.InvariantCulture, Resources.ExceptionMessage.FieldIndexOutOfRange, field));
            }

            Field = field;
            Direction = direction;
        }

        /// <summary>
        /// Contains the field index of the values to compare.
        /// </summary>
        public int Field { get; }

        /// <summary>
        /// Contains the sort direction.
        /// </summary>
        public ListSortDirection Direction { get; }

        public int Compare(string[] x, string[] y)
        {
            Debug.Assert(x != null && y != null && x.Length == y.Length && Field < x.Length);

            var result = string.Compare(x[Field], y[Field], StringComparison.CurrentCulture);

            return (Direction == ListSortDirection.Ascending ? result : -result);
        }
    }
}