using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;

using LumenWorks.Framework.IO.Csv.Resources;

namespace LumenWorks.Framework.IO.Csv
{
#if !NETSTANDARD1_3
    /// <summary>
    /// Represents a reader that provides fast, cached, dynamic access to CSV data.
    /// </summary>
    /// <remarks>The number of records is limited to <see cref="System.Int32.MaxValue"/> - 1.</remarks>
    public class CachedCsvReader : CsvReader, IListSource
    {
        /// <summary>
        /// Indicates if a new record is being read from the CSV stream.
        /// </summary>
        private bool _readingStream;

        /// <summary>
        /// Contains the binding list linked to this reader.
        /// </summary>
        private CsvBindingList _bindingList;

        /// <summary>
        /// Initializes a new instance of the CsvReader class.
        /// </summary>
        /// <param name="reader">A <see cref="T:TextReader"/> pointing to the CSV file.</param>
        /// <param name="hasHeaders"><see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.</param>
        /// <exception cref="T:ArgumentNullException"><paramref name="reader"/> is a <see langword="null"/>.</exception>
        /// <exception cref="T:ArgumentException">Cannot read from <paramref name="reader"/>.</exception>
        public CachedCsvReader(TextReader reader, bool hasHeaders = true) : this(reader, hasHeaders, DefaultBufferSize)
        {
        }

        /// <summary>
        /// Contains the cached records.
        /// </summary>
        public List<string[]> Records { get; private set; }

        /// <summary>
        /// Initializes a new instance of the CsvReader class.
        /// </summary>
        /// <param name="reader">A <see cref="T:TextReader"/> pointing to the CSV file.</param>
        /// <param name="hasHeaders"><see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.</param>
        /// <param name="bufferSize">The buffer size in bytes.</param>
        /// <exception cref="T:ArgumentNullException">
        ///        <paramref name="reader"/> is a <see langword="null"/>.
        /// </exception>
        /// <exception cref="T:ArgumentException">
        ///        Cannot read from <paramref name="reader"/>.
        /// </exception>
        public CachedCsvReader(TextReader reader, bool hasHeaders, int bufferSize)
            : this(reader, hasHeaders, DefaultDelimiter, DefaultQuote, DefaultEscape, DefaultComment, ValueTrimmingOptions.UnquotedOnly, bufferSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CsvReader class.
        /// </summary>
        /// <param name="reader">A <see cref="T:TextReader"/> pointing to the CSV file.</param>
        /// <param name="hasHeaders"><see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.</param>
        /// <param name="delimiter">The delimiter character separating each field (default is ',').</param>
        /// <exception cref="T:ArgumentNullException">
        ///        <paramref name="reader"/> is a <see langword="null"/>.
        /// </exception>
        /// <exception cref="T:ArgumentException">
        ///        Cannot read from <paramref name="reader"/>.
        /// </exception>
        public CachedCsvReader(TextReader reader, bool hasHeaders, char delimiter)
            : this(reader, hasHeaders, delimiter, DefaultQuote, DefaultEscape, DefaultComment, ValueTrimmingOptions.UnquotedOnly, DefaultBufferSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CsvReader class.
        /// </summary>
        /// <param name="reader">A <see cref="T:TextReader"/> pointing to the CSV file.</param>
        /// <param name="hasHeaders"><see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.</param>
        /// <param name="delimiter">The delimiter character separating each field (default is ',').</param>
        /// <param name="bufferSize">The buffer size in bytes.</param>
        /// <exception cref="T:ArgumentNullException">
        ///        <paramref name="reader"/> is a <see langword="null"/>.
        /// </exception>
        /// <exception cref="T:ArgumentException">
        ///        Cannot read from <paramref name="reader"/>.
        /// </exception>
        public CachedCsvReader(TextReader reader, bool hasHeaders, char delimiter, int bufferSize)
            : this(reader, hasHeaders, delimiter, DefaultQuote, DefaultEscape, DefaultComment, ValueTrimmingOptions.UnquotedOnly, bufferSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CsvReader class.
        /// </summary>
        /// <param name="reader">A <see cref="T:TextReader"/> pointing to the CSV file.</param>
        /// <param name="hasHeaders"><see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.</param>
        /// <param name="delimiter">The delimiter character separating each field (default is ',').</param>
        /// <param name="quote">The quotation character wrapping every field (default is ''').</param>
        /// <param name="escape">
        /// The escape character letting insert quotation characters inside a quoted field (default is '\').
        /// If no escape character, set to '\0' to gain some performance.
        /// </param>
        /// <param name="comment">The comment character indicating that a line is commented out (default is '#').</param>
        /// <param name="trimmingOptions">Determines how values should be trimmed.</param>
        /// <param name="nullValue">The value which denotes a DbNull-value.</param>
        /// <exception cref="T:ArgumentNullException">
        ///        <paramref name="reader"/> is a <see langword="null"/>.
        /// </exception>
        /// <exception cref="T:ArgumentException">
        ///        Cannot read from <paramref name="reader"/>.
        /// </exception>
        public CachedCsvReader(TextReader reader, bool hasHeaders, char delimiter, char quote, char escape, char comment, ValueTrimmingOptions trimmingOptions, string nullValue = null)
            : this(reader, hasHeaders, delimiter, quote, escape, comment, trimmingOptions, DefaultBufferSize, nullValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CsvReader class.
        /// </summary>
        /// <param name="reader">A <see cref="T:TextReader"/> pointing to the CSV file.</param>
        /// <param name="hasHeaders"><see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.</param>
        /// <param name="delimiter">The delimiter character separating each field (default is ',').</param>
        /// <param name="quote">The quotation character wrapping every field (default is ''').</param>
        /// <param name="escape">
        /// The escape character letting insert quotation characters inside a quoted field (default is '\').
        /// If no escape character, set to '\0' to gain some performance.
        /// </param>
        /// <param name="comment">The comment character indicating that a line is commented out (default is '#').</param>
        /// <param name="trimmingOptions">Determines how values should be trimmed.</param>
        /// <param name="bufferSize">The buffer size in bytes.</param>
        /// <param name="nullValue">The value which denotes a DbNull-value.</param>
        /// <exception cref="T:ArgumentNullException">
        ///        <paramref name="reader"/> is a <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///        <paramref name="bufferSize"/> must be 1 or more.
        /// </exception>
        public CachedCsvReader(TextReader reader, bool hasHeaders, char delimiter, char quote, char escape, char comment, ValueTrimmingOptions trimmingOptions, int bufferSize, string nullValue = null)
            : base(reader, hasHeaders, delimiter, quote, escape, comment, trimmingOptions, bufferSize, nullValue)
        {
            Records = new List<string[]>();
            CacheRecordIndex = -1;
        }

        /// <summary>
        /// Gets the current record index in the CSV file.
        /// </summary>
        /// <value>The current record index in the CSV file.</value>
        public override long CurrentRecordIndex => CacheRecordIndex;

        /// <summary>
        /// Contains the current record index (inside the cached records array).
        /// </summary>
        protected long CacheRecordIndex { get; private set; }

        /// <summary>
        /// Gets a value that indicates whether the current stream position is at the end of the stream.
        /// </summary>
        /// <value><see langword="true"/> if the current stream position is at the end of the stream; otherwise <see langword="false"/>.</value>
        public override bool EndOfStream => CacheRecordIndex >= FileRecordIndex && base.EndOfStream;

        /// <summary>
        /// Gets the field at the specified index.
        /// </summary>
        /// <value>The field at the specified index.</value>
        /// <exception cref="T:ArgumentOutOfRangeException"><paramref name="field"/> must be included in [0, <see cref="M:FieldCount"/>[.</exception>
        /// <exception cref="T:InvalidOperationException">No record read yet. Call ReadLine() first.</exception>
        /// <exception cref="MissingFieldCsvException">The CSV data appears to be missing a field.</exception>
        /// <exception cref="T:MalformedCsvException">The CSV appears to be corrupt at the current position.</exception>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">The instance has been disposed of.</exception>
        public override string this[int field]
        {
            get
            {
                if (_readingStream)
                {
                    return base[field];
                }

                if (CacheRecordIndex > -1)
                {
                    if (field > -1 && field < FieldCount)
                    {
                        return Records[(int) CacheRecordIndex][field];
                    }

                    throw new ArgumentOutOfRangeException(nameof(field), field, string.Format(CultureInfo.InvariantCulture, ExceptionMessage.FieldIndexOutOfRange, field));
                }
               
                throw new InvalidOperationException(ExceptionMessage.NoCurrentRecord);
            }
        }

        /// <summary>
        /// Reads the CSV stream from the current position to the end of the stream.
        /// </summary>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        ///    The instance has been disposed of.
        /// </exception>
        public virtual void ReadToEnd()
        {
            CacheRecordIndex = FileRecordIndex;

            while (ReadNextRecord()) ;
        }

        /// <summary>
        /// Reads the next record.
        /// </summary>
        /// <param name="onlyReadHeaders">
        /// Indicates if the reader will proceed to the next record after having read headers.
        /// <see langword="true"/> if it stops after having read headers; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="skipToNextLine">
        /// Indicates if the reader will skip directly to the next line without parsing the current one. 
        /// To be used when an error occurs.
        /// </param>
        /// <returns><see langword="true"/> if a record has been successfully reads; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        ///    The instance has been disposed of.
        /// </exception>
        protected override bool ReadNextRecord(bool onlyReadHeaders, bool skipToNextLine)
        {
            if (CacheRecordIndex < FileRecordIndex)
            {
                CacheRecordIndex++;
                return true;
            }
            else
            {
                _readingStream = true;
                
                try
                {
                    var canRead = base.ReadNextRecord(onlyReadHeaders, skipToNextLine);

                    if (canRead)
                    {
                        var record = new string[FieldCount];

                        if (FileRecordIndex > -1)
                        {
                            CopyCurrentRecordTo(record);
                            Records.Add(record);
                        }
                        else
                        {
                            if (MoveTo(0))
                            {
                                CopyCurrentRecordTo(record);
                            }

                            MoveTo(-1);
                        }

                        if (!onlyReadHeaders)
                        {
                            CacheRecordIndex++;
                        }
                    }
                    else
                    {
                        // No more records to read, so set array size to only what is needed
                        Records.Capacity = Records.Count;
                    }

                    return canRead;
                }
                finally
                {
                    _readingStream = false;
                }
            }
        }

        /// <summary>
        /// Moves before the first record.
        /// </summary>
        public void MoveToStart()
        {
            CacheRecordIndex = -1;
        }

        /// <summary>
        /// Moves to the last record read so far.
        /// </summary>
        public void MoveToLastCachedRecord()
        {
            CacheRecordIndex = FileRecordIndex;
        }

        /// <summary>
        /// Moves to the specified record index.
        /// </summary>
        /// <param name="record">The record index.</param>
        /// <returns><c>true</c> if the operation was successful; otherwise, <c>false</c>.</returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">The instance has been disposed of.</exception>
        public override bool MoveTo(long record)
        {
            if (record < -1)
            {
                record = -1;
            }

            if (record <= FileRecordIndex)
            {
                CacheRecordIndex = record;
                return true;
            }

            CacheRecordIndex = FileRecordIndex;
            return base.MoveTo(record);
        }

        bool IListSource.ContainsListCollection => false;

        System.Collections.IList IListSource.GetList()
        {
            return _bindingList ?? (_bindingList = new CsvBindingList(this));
        }
    }
#endif
}