using System;
using System.Collections;
using System.Collections.Generic;

using LumenWorks.Framework.IO.Csv.Resources;

namespace LumenWorks.Framework.IO.Csv
{
    public partial class CsvReader
    {
        /// <summary>
        /// Supports a simple iteration over the records of a <see cref="T:CsvReader"/>.
        /// </summary>
        public struct RecordEnumerator : IEnumerator<string[]>
        {
            /// <summary>
            /// Contains the enumerated <see cref="T:CsvReader"/>.
            /// </summary>
            private CsvReader _reader;

            /// <summary>
            /// Contains the current record.
            /// </summary>
            private string[] _current;

            /// <summary>
            /// Contains the current record index.
            /// </summary>
            private long _currentRecordIndex;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:RecordEnumerator"/> class.
            /// </summary>
            /// <param name="reader">The <see cref="T:CsvReader"/> to iterate over.</param>
            /// <exception cref="T:ArgumentNullException">
            ///        <paramref name="reader"/> is a <see langword="null"/>.
            /// </exception>
            public RecordEnumerator(CsvReader reader)
            {
                if (reader == null)
                {
                    throw new ArgumentNullException(nameof(reader));
                }

                _reader = reader;
                _current = null;

                _currentRecordIndex = reader.CurrentRecordIndex;
            }

            /// <summary>
            /// Gets the current record.
            /// </summary>
            public string[] Current
            {
                get { return _current; }
            }

            /// <summary>
            /// Advances the enumerator to the next record of the CSV.
            /// </summary>
            /// <returns><see langword="true"/> if the enumerator was successfully advanced to the next record, <see langword="false"/> if the enumerator has passed the end of the CSV.</returns>
            public bool MoveNext()
            {
                if (_reader.CurrentRecordIndex != _currentRecordIndex)
                {
                    throw new InvalidOperationException(ExceptionMessage.EnumerationVersionCheckFailed);
                }

                if (_reader.ReadNextRecord())
                {
                    _current = new string[_reader._fieldCount];

                    _reader.CopyCurrentRecordTo(_current);
                    _currentRecordIndex = _reader.CurrentRecordIndex;

                    return true;
                }
                else
                {
                    _current = null;
                    _currentRecordIndex = _reader.CurrentRecordIndex;

                    return false;
                }
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first record in the CSV.
            /// </summary>
            public void Reset()
            {
                if (_reader.CurrentRecordIndex != _currentRecordIndex)
                {
                    throw new InvalidOperationException(ExceptionMessage.EnumerationVersionCheckFailed);
                }

                _reader.MoveTo(-1);

                _current = null;
                _currentRecordIndex = _reader.CurrentRecordIndex;
            }

            /// <summary>
            /// Gets the current record.
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    if (_reader.CurrentRecordIndex != _currentRecordIndex)
                    {
                        throw new InvalidOperationException(ExceptionMessage.EnumerationVersionCheckFailed);
                    }

                    return this.Current;
                }
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                _reader = null;
                _current = null;
            }
        }
    }
}