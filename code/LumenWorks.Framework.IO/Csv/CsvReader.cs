using System;
using System.Collections;
using System.Collections.Generic;
#if !NETSTANDARD1_3
using System.Data;
using System.Data.Common;
#endif
using System.Globalization;
using System.IO;

using LumenWorks.Framework.IO.Csv.Resources;

using Debug = System.Diagnostics.Debug;

namespace LumenWorks.Framework.IO.Csv
{
    using System.Text;

    /// <summary>
    /// Represents a reader that provides fast, non-cached, forward-only access to CSV data.  
    /// </summary>
#if NETSTANDARD1_3
    public partial class CsvReader : IEnumerable<string[]>, IDisposable
#else
    public partial class CsvReader : IDataReader, IEnumerable<string[]>
#endif
    {
        /// <summary>
        /// Defines the default buffer size.
        /// </summary>
        public const int DefaultBufferSize = 0x1000;

        /// <summary>
        /// Defines the default delimiter character separating each field.
        /// </summary>
        public const char DefaultDelimiter = ',';

        /// <summary>
        /// Defines the default quote character wrapping every field.
        /// </summary>
        public const char DefaultQuote = '"';

        /// <summary>
        /// Defines the default escape character letting insert quotation characters inside a quoted field.
        /// </summary>
        public const char DefaultEscape = '"';

        /// <summary>
        /// Defines the default comment character indicating that a line is commented out.
        /// </summary>
        public const char DefaultComment = '#';

        /// <summary>
        /// Defines the default value for AddMark indicating should the CsvReader add null bytes removal mark ([removed x null bytes])
        /// </summary>
        private const bool DefaultAddMark = false;

        /// <summary>
        /// Defines the default value for Threshold indicating when the CsvReader should replace/remove consecutive null bytes
        /// </summary>
        private const int DefaultThreshold = 60;

        /// <summary>
        /// Contains the <see cref="T:TextReader"/> pointing to the CSV file.
        /// </summary>
        private TextReader _reader;

        /// <summary>
        /// Indicates if the class is initialized.
        /// </summary>
        private bool _initialized;

        /// <summary>
        /// Contains the dictionary of field indexes by header. The key is the field name and the value is its index.
        /// </summary>
        private Dictionary<string, int> _fieldHeaderIndexes;

        /// <summary>
        /// Contains the starting position of the next unread field.
        /// </summary>
        private int _nextFieldStart;

        /// <summary>
        /// Contains the index of the next unread field.
        /// </summary>
        private int _nextFieldIndex;

        /// <summary>
        /// Contains the array of the field values for the current record.
        /// A null value indicates that the field have not been parsed.
        /// </summary>
        private string[] _fields;

        /// <summary>
        /// Contains the maximum number of fields to retrieve for each record.
        /// </summary>
        private int _fieldCount;

        /// <summary>
        /// Contains the read buffer.
        /// </summary>
        private char[] _buffer;

        /// <summary>
        /// Contains the current read buffer length.
        /// </summary>
        private int _bufferLength;

        /// <summary>
        /// Indicates if the end of the reader has been reached.
        /// </summary>
        private bool _eof;

        /// <summary>
        /// Indicates if the last read operation reached an EOL character.
        /// </summary>
        private bool _eol;

        /// <summary>
        /// Indicates if the first record is in cache.
        /// This can happen when initializing a reader with no headers
        /// because one record must be read to get the field count automatically
        /// </summary>
        private bool _firstRecordInCache;

        /// <summary>
        /// Like CsvReader(TextReader reader, bool hasHeaders) but removes consecutive null bytes above a threshold from source stream.
        /// </summary>
        /// <param name="stream">A <see cref="T:Stream"/> pointing to the CSV file.</param>
        /// <param name="hasHeaders"><see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.</param>
        /// <param name="encoding"> specifies the encoding of the underlying stream.</param>
        /// <param name="addMark"><see langword="true"/> if want to add a mark ([removed x null bytes]) to indicate removal, remove silently if <see langword="false"/>.</param>
        /// <param name="threshold">only consecutive null bytes above this threshold will be removed or replaced by a mark.</param>
        /// <exception cref="T:ArgumentNullException"><paramref name="stream"/> is a <see langword="null"/>.</exception>
        /// <exception cref="T:ArgumentException">Cannot read from <paramref name="stream"/>.</exception>
        public CsvReader(Stream stream, bool hasHeaders, Encoding encoding, bool addMark = DefaultAddMark, int threshold = DefaultThreshold)
            : this(new NullRemovalStreamReader(stream, addMark, threshold, encoding), hasHeaders)
        {
        }

        /// <summary>
        /// Like CsvReader(TextReader reader, bool hasHeaders, int bufferSize) but removes consecutive null bytes above a threshold from source stream.
        /// </summary>
        /// <param name="stream">A <see cref="T:Stream"/> pointing to the CSV file.</param>
        /// <param name="hasHeaders"><see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.</param>
        /// <param name="encoding"> specifies the encoding of the underlying stream.</param>
        /// <param name="bufferSize">The buffer size in bytes.</param>
        /// <param name="addMark"><see langword="true"/> if want to add a mark ([removed x null bytes]) to indicate removal, remove silently if <see langword="false"/>.</param>
        /// <param name="threshold"> only consecutive null bytes above this threshold will be removed or replaced by a mark.</param>
        /// <exception cref="T:ArgumentNullException"><paramref name="stream"/> is a <see langword="null"/>.</exception>
        /// <exception cref="T:ArgumentException">Cannot read from <paramref name="stream"/>.</exception>
        public CsvReader(Stream stream, bool hasHeaders, Encoding encoding, int bufferSize, bool addMark = DefaultAddMark, int threshold = DefaultThreshold)
            : this(new NullRemovalStreamReader(stream, addMark, threshold, encoding, bufferSize), hasHeaders, bufferSize)
        {
        }

        /// <summary>
        /// Like CsvReader(TextReader reader, bool hasHeaders, char delimiter) but removes consecutive null bytes above a threshold from source stream.
        /// </summary>
        /// <param name="stream">A <see cref="T:Stream"/> pointing to the CSV file.</param>
        /// <param name="hasHeaders"><see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.</param>
        /// <param name="encoding"> specifies the encoding of the underlying stream.</param>
        /// <param name="delimiter">The delimiter character separating each field (default is ',').</param>
        /// <param name="addMark"><see langword="true"/> if want to add a mark ([removed x null bytes]) to indicate removal, remove silently if <see langword="false"/>.</param>
        /// <param name="threshold"> only consecutive null bytes above this threshold will be removed or replaced by a mark.</param>
        /// <exception cref="T:ArgumentNullException"><paramref name="stream"/> is a <see langword="null"/>.</exception>
        /// <exception cref="T:ArgumentException">Cannot read from <paramref name="stream"/>.</exception>
        public CsvReader(Stream stream, bool hasHeaders, Encoding encoding, char delimiter, bool addMark = DefaultAddMark, int threshold = DefaultThreshold)
            : this(new NullRemovalStreamReader(stream, addMark, threshold, encoding), hasHeaders, delimiter)
        {
        }

        /// <summary>
        /// Like CsvReader(TextReader reader, bool hasHeaders, char delimiter, int bufferSize) but removes consecutive null bytes above a threshold from source stream.
        /// </summary>
        /// <param name="stream">A <see cref="T:Stream"/> pointing to the CSV file.</param>
        /// <param name="hasHeaders"><see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.</param>
        /// <param name="encoding"> specifies the encoding of the underlying stream.</param>
        /// <param name="delimiter">The delimiter character separating each field (default is ',').</param>
        /// <param name="bufferSize">The buffer size in bytes.</param>
        /// <param name="addMark"><see langword="true"/> if want to add a mark ([removed x null bytes]) to indicate removal, remove silently if <see langword="false"/>.</param>
        /// <param name="threshold"> only consecutive null bytes above this threshold will be removed or replaced by a mark.</param>
        /// <exception cref="T:ArgumentNullException"><paramref name="stream"/> is a <see langword="null"/>.</exception>
        /// <exception cref="T:ArgumentException">Cannot read from <paramref name="stream"/>.</exception>
        public CsvReader(Stream stream, bool hasHeaders, Encoding encoding, char delimiter, int bufferSize, bool addMark = DefaultAddMark, int threshold = DefaultThreshold)
            : this(new NullRemovalStreamReader(stream, addMark, threshold, encoding, bufferSize), hasHeaders, delimiter, bufferSize)
        {
        }

        /// <summary>
        /// Like CsvReader(TextReader reader, bool hasHeaders, char delimiter, char quote, char escape, char comment, ValueTrimmingOptions trimmingOptions, string nullValue)
        /// but removes consecutive null bytes above a threshold from source stream.
        /// </summary>
        /// <param name="stream">A <see cref="T:Stream"/> pointing to the CSV file.</param>
        /// <param name="hasHeaders"><see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.</param>
        /// <param name="encoding"> specifies the encoding of the underlying stream.</param>
        /// <param name="delimiter">The delimiter character separating each field (default is ',').</param>
        /// <param name="quote">The quotation character wrapping every field (default is ''').</param>
        /// <param name="escape">
        /// The escape character letting insert quotation characters inside a quoted field (default is '\').
        /// If no escape character, set to '\0' to gain some performance.
        /// </param>
        /// <param name="comment">The comment character indicating that a line is commented out (default is '#').</param>
        /// <param name="trimmingOptions">Determines which values should be trimmed.</param>
        /// <param name="nullValue">The value which denotes a DbNull-value.</param>
        /// <param name="addMark"><see langword="true"/> if want to add a mark ([removed x null bytes]) to indicate removal, remove silently if <see langword="false"/>.</param>
        /// <param name="threshold"> only consecutive null bytes above this threshold will be removed or replaced by a mark.</param>
        /// <exception cref="T:ArgumentNullException"><paramref name="stream"/> is a <see langword="null"/>.</exception>
        /// <exception cref="T:ArgumentException">Cannot read from <paramref name="stream"/>.</exception>
        public CsvReader(Stream stream, bool hasHeaders, Encoding encoding, char delimiter, char quote, char escape, char comment, ValueTrimmingOptions trimmingOptions, string nullValue = null, bool addMark = DefaultAddMark, int threshold = DefaultThreshold)
            : this(new NullRemovalStreamReader(stream, addMark, threshold, encoding), hasHeaders, delimiter, quote, escape, comment, trimmingOptions, DefaultBufferSize, nullValue)
        {
        }

        /// <summary>
        ///     Like CsvReader(TextReader reader, bool hasHeaders, char delimiter, char quote, char escape, char comment, ValueTrimmingOptions trimmingOptions, int bufferSize, string nullValue)
        ///     but removes consecutive null bytes above a threshold from source stream.
        /// </summary>
        /// <param name="stream">A <see cref="T:Stream"/> pointing to the CSV file.</param>
        /// <param name="hasHeaders"><see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.</param>
        /// <param name="encoding"> specifies the encoding of the underlying stream.</param>
        /// <param name="delimiter">The delimiter character separating each field (default is ',').</param>
        /// <param name="quote">The quotation character wrapping every field (default is ''').</param>
        /// <param name="escape">
        /// The escape character letting insert quotation characters inside a quoted field (default is '\').
        /// If no escape character, set to '\0' to gain some performance.
        /// </param>
        /// <param name="comment">The comment character indicating that a line is commented out (default is '#').</param>
        /// <param name="trimmingOptions">Determines which values should be trimmed.</param>
        /// <param name="bufferSize">The buffer size in bytes.</param>
        /// <param name="nullValue">The value which denotes a DbNull-value.</param>
        /// <param name="addMark"><see langword="true"/> if want to add a mark ([removed x null bytes]) to indicate removal, remove silently if <see langword="false"/>.</param>
        /// <param name="threshold"> only consecutive null bytes above this threshold will be removed or replace by a mark.</param>
        /// <exception cref="T:ArgumentNullException"><paramref name="stream"/> is a <see langword="null"/>.</exception>
        /// <exception cref="T:ArgumentException">Cannot read from <paramref name="stream"/>.</exception>
        public CsvReader(Stream stream, bool hasHeaders, Encoding encoding, char delimiter, char quote, char escape, char comment, ValueTrimmingOptions trimmingOptions, int bufferSize, string nullValue = null, bool addMark = DefaultAddMark, int threshold = DefaultThreshold)
            : this(new NullRemovalStreamReader(stream, addMark, threshold, encoding, bufferSize), hasHeaders, delimiter, quote, escape, comment, trimmingOptions, bufferSize, nullValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CsvReader class.
        /// </summary>
        /// <param name="reader">A <see cref="T:TextReader"/> pointing to the CSV file.</param>
        /// <param name="hasHeaders"><see langword="true"/>If field names are located on the first non commented line, otherwise, <see langword="false"/>.</param>
        /// <exception cref="T:ArgumentNullException"><paramref name="reader"/> is a <see langword="null"/>.</exception>
        /// <exception cref="T:ArgumentException">Cannot read from <paramref name="reader"/>.</exception>
        public CsvReader(TextReader reader, bool hasHeaders) : this(reader, hasHeaders, DefaultDelimiter, DefaultQuote)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CsvReader class.
        /// </summary>
        /// <param name="reader">A <see cref="T:TextReader"/> pointing to the CSV file.</param>
        /// <param name="hasHeaders"><see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.</param>
        /// <param name="bufferSize">The buffer size in bytes.</param>
        /// <exception cref="T:ArgumentNullException"><paramref name="reader"/> is a <see langword="null"/>.</exception>
        /// <exception cref="T:ArgumentException">Cannot read from <paramref name="reader"/>.</exception>
        public CsvReader(TextReader reader, bool hasHeaders, int bufferSize) : this(reader, hasHeaders, DefaultDelimiter, DefaultQuote, bufferSize: bufferSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CsvReader class.
        /// </summary>
        /// <param name="reader">A <see cref="T:TextReader"/> pointing to the CSV file.</param>
        /// <param name="hasHeaders"><see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.</param>
        /// <param name="delimiter">The delimiter character separating each field (default is ',').</param>
        /// <exception cref="T:ArgumentNullException"><paramref name="reader"/> is a <see langword="null"/>.</exception>
        /// <exception cref="T:ArgumentException">Cannot read from <paramref name="reader"/>.</exception>
        public CsvReader(TextReader reader, bool hasHeaders, char delimiter) : this(reader, hasHeaders, delimiter, DefaultQuote)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CsvReader class.
        /// </summary>
        /// <param name="reader">A <see cref="T:TextReader"/> pointing to the CSV file.</param>
        /// <param name="hasHeaders"><see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.</param>
        /// <param name="delimiter">The delimiter character separating each field (default is ',').</param>
        /// <param name="bufferSize">The buffer size in bytes.</param>
        /// <exception cref="T:ArgumentNullException"><paramref name="reader"/> is a <see langword="null"/>.</exception>
        /// <exception cref="T:ArgumentException">Cannot read from <paramref name="reader"/>.</exception>
        public CsvReader(TextReader reader, bool hasHeaders, char delimiter, int bufferSize) : this(reader, hasHeaders, delimiter, DefaultQuote, bufferSize: bufferSize)
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
        /// <param name="trimmingOptions">Determines which values should be trimmed.</param>
        /// <param name="bufferSize">The buffer size in bytes.</param>
        /// <param name="nullValue">The value which denotes a DbNull-value.</param>
        /// <exception cref="T:ArgumentNullException"><paramref name="reader"/> is a <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="bufferSize"/> must be 1 or more.</exception>
        public CsvReader(TextReader reader, bool hasHeaders = true, char delimiter = DefaultDelimiter, char quote = DefaultQuote, char escape = DefaultEscape, char comment = DefaultComment, ValueTrimmingOptions trimmingOptions = ValueTrimmingOptions.UnquotedOnly, int bufferSize = DefaultBufferSize, string nullValue = null)
        {
#if DEBUG
#if !NETSTANDARD1_3
            _allocStack = new System.Diagnostics.StackTrace();
#endif
#endif

            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize), bufferSize, ExceptionMessage.BufferSizeTooSmall);
            }

            BufferSize = bufferSize;

            var streamReader = reader as StreamReader;
            if (streamReader != null)
            {
                var stream = streamReader.BaseStream;

                if (stream.CanSeek)
                {
                    // Handle bad implementations returning 0 or less
                    if (stream.Length > 0)
                    {
                        BufferSize = (int) Math.Min(bufferSize, stream.Length);
                    }
                }
            }

            _reader = reader;
            Delimiter = delimiter;
            Quote = quote;
            Escape = escape;
            Comment = comment;

            HasHeaders = hasHeaders;
            TrimmingOption = trimmingOptions;
            NullValue = nullValue;
            SupportsMultiline = true;
            SkipEmptyLines = true;

            Columns = new List<Column>();
            DefaultHeaderName = "Column";

            FileRecordIndex = -1;
            DefaultParseErrorAction = ParseErrorAction.RaiseEvent;
        }

        /// <summary>
        /// Occurs when there is an error while parsing the CSV stream.
        /// </summary>
        public event EventHandler<ParseErrorEventArgs> ParseError;

        /// <summary>
        /// Raises the <see cref="M:ParseError"/> event.
        /// </summary>
        /// <param name="e">The <see cref="ParseErrorEventArgs"/> that contains the event data.</param>
        protected virtual void OnParseError(ParseErrorEventArgs e)
        {
            var handler = ParseError;

            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Occurs when HasHeaders is true and a duplicate Column Header Name is encountered.
        /// Setting the HeaderName property on this column will prevent the library from throwing a duplicate key exception
        /// </summary>
        public event EventHandler<DuplicateHeaderEventArgs> DuplicateHeaderEncountered;

        /// <summary>
        /// Gets the comment character indicating that a line is commented out.
        /// </summary>
        /// <value>The comment character indicating that a line is commented out.</value>
        public char Comment { get; }

        /// <summary>
        /// Gets the escape character letting insert quotation characters inside a quoted field.
        /// </summary>
        /// <value>The escape character letting insert quotation characters inside a quoted field.</value>
        public char Escape { get; }

        /// <summary>
        /// Gets the delimiter character separating each field.
        /// </summary>
        /// <value>The delimiter character separating each field.</value>
        public char Delimiter { get; }

        /// <summary>
        /// Gets the quotation character wrapping every field.
        /// </summary>
        /// <value>The quotation character wrapping every field.</value>
        public char Quote { get; }

        /// <summary>
        /// Indicates if field names are located on the first non commented line.
        /// </summary>
        /// <value><see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.</value>
        public bool HasHeaders { get; }

        /// <summary>
        /// Indicates if spaces at the start and end of a field are trimmed.
        /// </summary>
        /// <value><see langword="true"/> if spaces at the start and end of a field are trimmed, otherwise, <see langword="false"/>.</value>
        public ValueTrimmingOptions TrimmingOption { get; }

        /// <summary>
        /// Contains the value which denotes a DbNull-value.
        /// </summary>
        public string NullValue { get; }

        /// <summary>
        /// Gets the buffer size.
        /// </summary>
        public int BufferSize { get; }

        /// <summary>
        /// Gets or sets the default action to take when a parsing error has occured.
        /// </summary>
        /// <value>The default action to take when a parsing error has occured.</value>
        public ParseErrorAction DefaultParseErrorAction { get; set; }

        /// <summary>
        /// Gets or sets the action to take when a field is missing.
        /// </summary>
        /// <value>The action to take when a field is missing.</value>
        public MissingFieldAction MissingFieldAction { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the reader supports multiline fields.
        /// </summary>
        /// <value>A value indicating if the reader supports multiline field.</value>
        public bool SupportsMultiline { get; set; }

        /// <summary>
        /// Gets or sets a value giving a maxmimum length (in bytes) for any quoted field.
        /// </summary>
        /// <value>The maximum length (in bytes) of a CSV field.</value>
        public int? MaxQuotedFieldLength { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the reader will skip empty lines.
        /// </summary>
        /// <value>A value indicating if the reader will skip empty lines.</value>
        public bool SkipEmptyLines { get; set; }

        /// <summary>
        /// Gets or sets the default header name when it is an empty string or only whitespaces.
        /// The header index will be appended to the specified name.
        /// </summary>
        /// <value>The default header name when it is an empty string or only whitespaces.</value>
        public string DefaultHeaderName { get; set; }

        /// <summary>
        /// Gets or sets column information for the CSV.
        /// </summary>
        public IList<Column> Columns { get; set; }

        /// <summary>
        /// Gets or sets whether we should use the column default values if the field is not in the record.
        /// </summary>
        public bool UseColumnDefaults { get; set; }

        /// <summary>
        /// Gets the maximum number of fields to retrieve for each record.
        /// </summary>
        /// <value>The maximum number of fields to retrieve for each record.</value>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">The instance has been disposed of.</exception>
        public int FieldCount
        {
            get
            {
                EnsureInitialize();
                return _fieldCount;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the current stream position is at the end of the stream.
        /// </summary>
        /// <value><see langword="true"/> if the current stream position is at the end of the stream; otherwise <see langword="false"/>.</value>
        public virtual bool EndOfStream { get; private set; }

        /// <summary>
        /// Gets the field headers.
        /// </summary>
        /// <returns>The field headers or an empty array if headers are not supported.</returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">The instance has been disposed of.</exception>
        public string[] GetFieldHeaders()
        {
            EnsureInitialize();
            Debug.Assert(Columns != null, "Columns must be non null.");

            var fieldHeaders = new string[Columns.Count];

            for (var i = 0; i < fieldHeaders.Length; i++)
            {
                fieldHeaders[i] = Columns[i].Name;
            }

            return fieldHeaders;
        }

        /// <summary>
        /// Gets the current record index in the CSV file.
        /// <para>
        /// A value of <see cref="M:Int32.MinValue"/> means that the reader has not been initialized yet.
        /// Otherwise, a negative value means that no record has been read yet.
        /// </para>
        /// </summary>
        /// <value>The current record index in the CSV file.</value>
        protected long FileRecordIndex { get; private set; }

        /// <summary>
        /// Gets the current record index in the CSV file.
        /// <para>
        /// A value of <see cref="M:Int32.MinValue"/> means that the reader has not been initialized yet.
        /// Otherwise, a negative value means that no record has been read yet.
        /// </para>
        /// </summary>
        /// <value>The current record index in the CSV file.</value>
        public virtual long CurrentRecordIndex => FileRecordIndex;

        /// <summary>
        /// Indicates if one or more field are missing for the current record.
        /// Resets after each successful record read.
        /// </summary>
        public bool MissingFieldFlag { get; private set; }

        /// <summary>
        /// Indicates if a parse error occured for the current record.
        /// Resets after each successful record read.
        /// </summary>
        public bool ParseErrorFlag { get; private set; }

        /// <summary>
        /// Gets the field with the specified name and record position. <see cref="M:hasHeaders"/> must be <see langword="true"/>.
        /// </summary>
        /// <value>
        /// The field with the specified name and record position.
        /// </value>
        /// <exception cref="T:ArgumentNullException"><paramref name="field"/> is <see langword="null"/> or an empty string.</exception>
        /// <exception cref="T:InvalidOperationException">The CSV does not have headers (<see cref="M:HasHeaders"/> property is <see langword="false"/>).</exception>
        /// <exception cref="T:ArgumentException"><paramref name="field"/> not found.</exception>
        /// <exception cref="T:ArgumentOutOfRangeException">Record index must be > 0.</exception>
        /// <exception cref="T:InvalidOperationException">Cannot move to a previous record in forward-only mode.</exception>
        /// <exception cref="T:EndOfStreamException">Cannot read record at <paramref name="record"/>.</exception>
        /// <exception cref="T:MalformedCsvException">The CSV appears to be corrupt at the current position.</exception>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">The instance has been disposed of.</exception>
        public string this[int record, string field]
        {
            get
            {
                if (!MoveTo(record))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, ExceptionMessage.CannotReadRecordAtIndex, record));
                }

                return this[field];
            }
        }

        /// <summary>
        /// Gets the field at the specified index and record position.
        /// </summary>
        /// <value>
        /// The field at the specified index and record position.
        /// A <see langword="null"/> is returned if the field cannot be found for the record.
        /// </value>
        /// <exception cref="T:ArgumentOutOfRangeException"><paramref name="field"/> must be included in [0, <see cref="M:FieldCount"/>[.</exception>
        /// <exception cref="T:ArgumentOutOfRangeException">Record index must be > 0.</exception>
        /// <exception cref="T:InvalidOperationException">Cannot move to a previous record in forward-only mode.</exception>
        /// <exception cref="T:EndOfStreamException">Cannot read record at <paramref name="record"/>.</exception>
        /// <exception cref="T:MalformedCsvException">The CSV appears to be corrupt at the current position.</exception>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">The instance has been disposed of.</exception>
        public string this[int record, int field]
        {
            get
            {
                if (!MoveTo(record))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, ExceptionMessage.CannotReadRecordAtIndex, record));
                }

                return this[field];
            }
        }

        /// <summary>
        /// Gets the field with the specified name. <see cref="M:hasHeaders"/> must be <see langword="true"/>.
        /// </summary>
        /// <value>
        /// The field with the specified name.
        /// </value>
        /// <exception cref="T:ArgumentNullException"><paramref name="field"/> is <see langword="null"/> or an empty string.</exception>
        /// <exception cref="T:InvalidOperationException">The CSV does not have headers (<see cref="M:HasHeaders"/> property is <see langword="false"/>).</exception>
        /// <exception cref="T:ArgumentException"><paramref name="field"/> not found.</exception>
        /// <exception cref="T:MalformedCsvException">The CSV appears to be corrupt at the current position.</exception>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">The instance has been disposed of.</exception>
        public string this[string field]
        {
            get
            {
                if (string.IsNullOrEmpty(field))
                {
                    throw new ArgumentNullException(nameof(field));
                }

                if (!HasHeaders)
                {
                    throw new InvalidOperationException(ExceptionMessage.NoHeaders);
                }

                var index = GetFieldIndex(field);

                if (index < 0)
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, ExceptionMessage.FieldHeaderNotFound, field), "field");
                }

                return this[index];
            }
        }

        /// <summary>
        /// Gets the field at the specified index.
        /// </summary>
        /// <value>The field at the specified index.</value>
        /// <exception cref="T:ArgumentOutOfRangeException"><paramref name="field"/> must be included in [0, <see cref="M:FieldCount"/>[.</exception>
        /// <exception cref="T:InvalidOperationException">No record read yet. Call ReadLine() first.</exception>
        /// <exception cref="T:MalformedCsvException">The CSV appears to be corrupt at the current position.</exception>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">The instance has been disposed of.</exception>
        public virtual string this[int field] => ReadField(field, false, false);

        /// <summary>
        /// Ensures that the reader is initialized.
        /// </summary>
        private void EnsureInitialize()
        {
            if (!_initialized)
            {
                ReadNextRecord(true, false);
            }

            Debug.Assert(Columns != null);
            Debug.Assert(Columns.Count > 0 || (Columns.Count == 0 && (_fieldHeaderIndexes == null || _fieldHeaderIndexes.Count == 0)));
        }

        /// <summary>
        /// Gets the field index for the provided header.
        /// </summary>
        /// <param name="header">The header to look for.</param>
        /// <returns>The field index for the provided header. -1 if not found.</returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">The instance has been disposed of.</exception>
        public int GetFieldIndex(string header)
        {
            EnsureInitialize();

            if (_fieldHeaderIndexes != null && _fieldHeaderIndexes.TryGetValue(header, out int index))
            {
                return index;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Checks if a header exists in the current fieldHedaerIndexes
        /// </summary>
        /// <param name="header">The header to look for.</param>
        /// <returns>A flag indicating if the header exists</returns>
        public bool HasHeader(string header)
        {
            EnsureInitialize();

            if (string.IsNullOrEmpty(header))
            {
                throw new ArgumentNullException(nameof(header));
            }

            if (_fieldHeaderIndexes != null)
            {
                return _fieldHeaderIndexes.ContainsKey(header);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Copies the field array of the current record to a one-dimensional array, starting at the beginning of the target array.
        /// </summary>
        /// <param name="array"> The one-dimensional <see cref="T:Array"/> that is the destination of the fields of the current record.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:ArgumentOutOfRangeException"><paramref name="index"/> is les than zero or is equal to or greater than the length <paramref name="array"/>.</exception>
        /// <exception cref="InvalidOperationException">No current record.</exception>
        /// <exception cref="ArgumentException">The number of fields in the record is greater than the available space from <paramref name="index"/> to the end of <paramref name="array"/>.</exception>
        public void CopyCurrentRecordTo(string[] array, int index = 0)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (index < 0 || index >= array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, string.Empty);
            }

            if (FileRecordIndex < 0 || !_initialized)
            {
                throw new InvalidOperationException(ExceptionMessage.NoCurrentRecord);
            }

            if (array.Length - index < _fieldCount)
            {
                throw new ArgumentException(ExceptionMessage.NotEnoughSpaceInArray, nameof(array));
            }

            for (var i = 0; i < _fieldCount; i++)
            {
                array[index + i] = ParseErrorFlag ? null : this[i];
            }
        }

        /// <summary>
        /// Gets the current raw CSV data.
        /// </summary>
        /// <remarks>Used for exception handling purpose.</remarks>
        /// <returns>The current raw CSV data.</returns>
        public string GetCurrentRawData()
        {
            if (_buffer != null && _bufferLength > 0)
            {
                return new string(_buffer, 0, _bufferLength);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Indicates whether the specified Unicode character is categorized as white space.
        /// </summary>
        /// <param name="c">A Unicode character.</param>
        /// <returns><see langword="true"/> if <paramref name="c"/> is white space; otherwise, <see langword="false"/>.</returns>
        private bool IsWhiteSpace(char c)
        {
            // Handle cases where the delimiter is a whitespace (e.g. tab)
            if (c == Delimiter)
            {
                return false;
            }
            else
            {
                // See char.IsLatin1(char c) in Reflector
                if (c <= '\x00ff')
                {
                    return (c == ' ' || c == '\t');
                }
                else
                {
                    return (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.SpaceSeparator);
                }
            }
        }

        /// <summary>
        /// Moves to the specified record index.
        /// </summary>
        /// <param name="record">The record index.</param>
        /// <returns><c>true</c> if the operation was successful; otherwise, <c>false</c>.</returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">The instance has been disposed of.</exception>
        public virtual bool MoveTo(long record)
        {
            if (record < FileRecordIndex)
            {
                return false;
            }

            // Get number of record to read
            var offset = record - FileRecordIndex;

            while (offset > 0)
            {
                if (!ReadNextRecord())
                {
                    return false;
                }

                offset--;
            }

            return true;
        }

        /// <summary>
        /// Parses a new line delimiter.
        /// </summary>
        /// <param name="pos">The starting position of the parsing. Will contain the resulting end position.</param>
        /// <returns><see langword="true"/> if a new line delimiter was found; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">The instance has been disposed of.</exception>
        private bool ParseNewLine(ref int pos)
        {
            Debug.Assert(pos <= _bufferLength);

            // Check if already at the end of the buffer
            if (pos == _bufferLength)
            {
                pos = 0;

                if (!ReadBuffer())
                {
                    return false;
                }
            }

            var c = _buffer[pos];

            // Treat \r as new line only if it's not the delimiter
            if (c == '\r' && Delimiter != '\r')
            {
                pos++;

                // Skip following \n (if there is one)

                if (pos < _bufferLength)
                {
                    if (_buffer[pos] == '\n')
                    {
                        pos++;
                    }
                }
                else
                {
                    if (ReadBuffer())
                    {
                        if (_buffer[0] == '\n')
                        {
                            pos = 1;
                        }
                        else
                        {
                            pos = 0;
                        }
                    }
                }

                if (pos >= _bufferLength)
                {
                    ReadBuffer();
                    pos = 0;
                }

                return true;
            }
            else if (c == '\n')
            {
                pos++;

                if (pos >= _bufferLength)
                {
                    ReadBuffer();
                    pos = 0;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the character at the specified position is a new line delimiter.
        /// </summary>
        /// <param name="pos">The position of the character to verify.</param>
        /// <returns><see langword="true"/> if the character at the specified position is a new line delimiter; otherwise, <see langword="false"/>.</returns>
        private bool IsNewLine(int pos)
        {
            Debug.Assert(pos < _bufferLength);

            var c = _buffer[pos];

            if (c == '\n')
            {
                return true;
            }
            else if (c == '\r' && Delimiter != '\r')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Fills the buffer with data from the reader.
        /// </summary>
        /// <returns><see langword="true"/> if data was successfully read; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">The instance has been disposed of.</exception>
        private bool ReadBuffer()
        {
            if (_eof)
            {
                return false;
            }

            CheckDisposed();

            _bufferLength = _reader.Read(_buffer, 0, BufferSize);

            if (_bufferLength > 0)
            {
                return true;
            }
            else
            {
                _eof = true;
                _buffer = null;

                return false;
            }
        }

        /// <summary>
        /// Reads the field at the specified index.
        /// Any unread fields with an inferior index will also be read as part of the required parsing.
        /// </summary>
        /// <param name="field">The field index.</param>
        /// <param name="initializing">Indicates if the reader is currently initializing.</param>
        /// <param name="discardValue">Indicates if the value(s) are discarded.</param>
        /// <returns>
        /// The field at the specified index. 
        /// A <see langword="null"/> indicates that an error occured or that the last field has been reached during initialization.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="field"/> is out of range.</exception>
        /// <exception cref="InvalidOperationException">There is no current record.</exception>
        /// <exception cref="MissingFieldCsvException">The CSV data appears to be missing a field.</exception>
        /// <exception cref="MalformedCsvException">The CSV data appears to be malformed.</exception>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">The instance has been disposed of.</exception>
        private string ReadField(int field, bool initializing, bool discardValue)
        {
            if (!initializing)
            {
                var maxField = UseColumnDefaults ? Columns.Count : _fieldCount;
                if (field < 0 || field >= maxField)
                {
                    throw new ArgumentOutOfRangeException(nameof(field), field, string.Format(CultureInfo.InvariantCulture, ExceptionMessage.FieldIndexOutOfRange, field));
                }

                if (FileRecordIndex < 0)
                {
                    throw new InvalidOperationException(ExceptionMessage.NoCurrentRecord);
                }

                if (Columns.Count > field && !string.IsNullOrEmpty(Columns[field].OverrideValue))
                {
                    // Use the override value for this column.
                    return Columns[field].OverrideValue;
                }

                if (field >= _fieldCount)
                {
                    // Use the column default as UseColumnDefaults is true at this point
                    return Columns[field].DefaultValue;
                }

                // Directly return field if cached
                if (_fields[field] != null)
                {
                    return _fields[field];
                }

                if (MissingFieldFlag)
                {
                    return HandleMissingField(null, field, ref _nextFieldStart);
                }
            }

            CheckDisposed();

            var index = _nextFieldIndex;

            while (index < field + 1)
            {
                // Handle case where stated start of field is past buffer
                // This can occur because _nextFieldStart is simply 1 + last char position of previous field
                if (_nextFieldStart == _bufferLength)
                {
                    _nextFieldStart = 0;

                    // Possible EOF will be handled later (see Handle_EOF1)
                    ReadBuffer();
                }

                StringBuilder value = null;

                if (MissingFieldFlag)
                {
                    string result = HandleMissingField(value?.ToString(), index, ref _nextFieldStart);
                    if(value == null && result == string.Empty && MissingFieldAction == MissingFieldAction.ReplaceByEmpty)
                    {
                        value = new StringBuilder();
                    }
                }
                else if (_nextFieldStart == _bufferLength)
                {
                    // Handle_EOF1: Handle EOF here

                    // If current field is the requested field, then the value of the field is "" as in "f1,f2,f3,(\s*)"
                    // otherwise, the CSV is malformed

                    if (index == field)
                    {
                        if (!discardValue)
                        {
                            value = new StringBuilder();
                            _fields[index] = string.Empty;
                        }

                        MissingFieldFlag = true;
                    }
                    else
                    {
                        string result = HandleMissingField(value?.ToString(), index, ref _nextFieldStart);
                        if(value == null && result == string.Empty && MissingFieldAction == MissingFieldAction.ReplaceByEmpty)
                        {
                            value = new StringBuilder();
                        }
                    }
                }
                else
                {
                    // Trim spaces at start
                    if ((TrimmingOption & ValueTrimmingOptions.UnquotedOnly) != 0)
                    {
                        SkipWhiteSpaces(ref _nextFieldStart);
                    }

                    if (_eof)
                    {
                        value = new StringBuilder();
                        _fields[field] = string.Empty;

                        if (field < _fieldCount)
                        {
                            MissingFieldFlag = true;
                        }
                    }
                    else if (_buffer[_nextFieldStart] != Quote)
                    {
                        // Non-quoted field

                        var start = _nextFieldStart;
                        var pos = _nextFieldStart;

                        for (;;)
                        {
                            while (pos < _bufferLength)
                            {
                                var c = _buffer[pos];

                                if (c == Delimiter)
                                {
                                    _nextFieldStart = pos + 1;

                                    break;
                                }
                                else if (c == '\r' || c == '\n')
                                {
                                    _nextFieldStart = pos;
                                    _eol = true;

                                    break;
                                }
                                else
                                {
                                    pos++;
                                }
                            }

                            if (pos < _bufferLength)
                            {
                                break;
                            }
                            else
                            {
                                if (!discardValue)
                                {
                                    value = value ?? new StringBuilder();
                                    value.Append(_buffer, start, pos - start);
                                }

                                start = 0;
                                pos = 0;
                                _nextFieldStart = 0;

                                if (!ReadBuffer())
                                {
                                    break;
                                }
                            }
                        }

                        if (!discardValue)
                        {
                            if ((TrimmingOption & ValueTrimmingOptions.UnquotedOnly) == 0)
                            {
                                if (!_eof && pos > start)
                                {
                                    value = value ?? new StringBuilder();
                                    value.Append(_buffer, start, pos - start);
                                }
                            }
                            else
                            {
                                if (!_eof && pos > start)
                                {
                                    // Do the trimming
                                    pos--;
                                    while (pos > -1 && IsWhiteSpace(_buffer[pos]))
                                    {
                                        pos--;
                                    }

                                    pos++;

                                    if (pos > 0)
                                    {
                                        value = value ?? new StringBuilder();
                                        value.Append(_buffer, start, pos - start);
                                    }
                                }
                                else
                                {
                                    pos = -1;
                                }

                                // If pos <= 0, that means the trimming went past buffer start,
                                // and the concatenated value needs to be trimmed too.
                                if (pos <= 0)
                                {
                                    pos = value?.Length - 1 ?? -1;

                                    // Do the trimming
                                    while (pos > -1 && IsWhiteSpace(value[pos]))
                                    {
                                        pos--;
                                    }

                                    pos++;

                                    if (pos > 0 && pos != value.Length)
                                    {
                                        value.Length = pos;
                                    }
                                }
                            }

                            value = value ?? new StringBuilder();
                        }

                        if (_eol || _eof)
                        {
                            _eol = ParseNewLine(ref _nextFieldStart);

                            // Reaching a new line is ok as long as the parser is initializing or it is the last field
                            if (!initializing && index != _fieldCount - 1)
                            {
                                if (value != null && value.Length == 0)
                                {
                                    value = null;
                                }

                                string result = HandleMissingField(value?.ToString(), index, ref _nextFieldStart);
                                if(value == null && result == string.Empty && MissingFieldAction == MissingFieldAction.ReplaceByEmpty)
                                {
                                    value = new StringBuilder();
                                }
                            }
                        }

                        if (!discardValue)
                        {
                            _fields[index] = value?.ToString();
                        }
                    }
                    else
                    {
                        // Quoted field

                        // Skip quote
                        var start = _nextFieldStart + 1;
                        var pos = start;

                        var quoted = true;
                        var escaped = false;
                        var fieldLength = 0;

                        if ((TrimmingOption & ValueTrimmingOptions.QuotedOnly) != 0)
                        {
                            SkipWhiteSpaces(ref start);
                            pos = start;
                        }

                        for (;;)
                        {
                            while (pos < _bufferLength)
                            {
                                var c = _buffer[pos];

                                if (escaped)
                                {
                                    escaped = false;
                                    start = pos;
                                }
                                // IF current char is escape AND (escape and quote are different OR next char is a quote)
                                else if (c == Escape && (Escape != Quote || (pos + 1 < _bufferLength && _buffer[pos + 1] == Quote) || (pos + 1 == _bufferLength && _reader.Peek() == Quote)))
                                {
                                    if (!discardValue)
                                    {
                                        value = value ?? new StringBuilder();
                                        value.Append(_buffer, start, pos - start);
                                    }

                                    escaped = true;
                                }
                                else if (c == Quote)
                                {
                                    quoted = false;
                                    break;
                                }

                                fieldLength++;

                                if (MaxQuotedFieldLength.HasValue && fieldLength > MaxQuotedFieldLength.Value)
                                {
                                    HandleParseError(new MalformedCsvException(GetCurrentRawData(), _nextFieldStart, Math.Max(0, FileRecordIndex), index), ref _nextFieldStart);
                                    return null;
                                }

                                pos++;
                            }

                            if (!quoted)
                            {
                                break;
                            }
                            else
                            {
                                if (!discardValue && !escaped)
                                {
                                    value = value ?? new StringBuilder();
                                    value.Append(_buffer, start, pos - start);
                                }

                                start = 0;
                                pos = 0;
                                _nextFieldStart = 0;

                                if (!ReadBuffer())
                                {
                                    HandleParseError(new MalformedCsvException(GetCurrentRawData(), _nextFieldStart, Math.Max(0, FileRecordIndex), index), ref _nextFieldStart);
                                    return null;
                                }
                            }
                        }

                        if (!_eof)
                        {
                            // Append remaining parsed buffer content
                            if (!discardValue && pos > start)
                            {
                                value = value ?? new StringBuilder();
                                value.Append(_buffer, start, pos - start);
                            }

                            if (!discardValue && value != null && (TrimmingOption & ValueTrimmingOptions.QuotedOnly) != 0)
                            {
                                var newLength = value.Length;
                                while (newLength > 0 && IsWhiteSpace(value[newLength - 1]))
                                {
                                    newLength--;
                                }

                                if (newLength < value.Length)
                                {
                                    value.Length = newLength;
                                }
                            }

                            // Skip quote
                            _nextFieldStart = pos + 1;

                            // Skip whitespaces between the quote and the delimiter/eol
                            SkipWhiteSpaces(ref _nextFieldStart);

                            // Skip delimiter
                            bool delimiterSkipped;
                            if (_nextFieldStart < _bufferLength && _buffer[_nextFieldStart] == Delimiter)
                            {
                                _nextFieldStart++;
                                delimiterSkipped = true;
                            }
                            else if (_nextFieldStart < _bufferLength && (_buffer[_nextFieldStart] == '\r' || _buffer[_nextFieldStart] == '\n'))
                            {
                                _nextFieldStart++;
                                _eol = true;
                                delimiterSkipped = true;
                            }
                            else
                            {
                                delimiterSkipped = false;
                            }

                            // Skip new line delimiter if initializing or last field
                            // (if the next field is missing, it will be caught when parsed)
                            if (!_eof && !delimiterSkipped && (initializing || index == _fieldCount - 1))
                            {
                                _eol = ParseNewLine(ref _nextFieldStart);
                            }

                            // If no delimiter is present after the quoted field and it is not the last field, then it is a parsing error
                            if (!delimiterSkipped && !_eof && !(_eol || IsNewLine(_nextFieldStart)))
                            {
                                HandleParseError(new MalformedCsvException(GetCurrentRawData(), _nextFieldStart, Math.Max(0, FileRecordIndex), index), ref _nextFieldStart);
                            }
                        }

                        // If we are at the end, then verify we have all the fields
                        if (_eol || _eof)
                        {
                            if (!initializing && index < _fieldCount - 1)
                            {
                                string result = HandleMissingField(value?.ToString(), index, ref _nextFieldStart);
                                if(value == null && result == string.Empty && MissingFieldAction == MissingFieldAction.ReplaceByEmpty)
                                {
                                    value = new StringBuilder();
                                }
                            }
                        }

                        if (!discardValue)
                        {
                            value = value ?? new StringBuilder();
                            _fields[index] = value.ToString();
                        }
                    }
                }

                _nextFieldIndex = Math.Max(index + 1, _nextFieldIndex);

                if (index == field)
                {
                    // If initializing, return null to signify the last field has been reached

                    if (initializing)
                    {
                        if (_eol || _eof)
                        {
                            return null;
                        }
                        else
                        {
                            return value == null ? string.Empty : value.ToString();
                        }
                    }
                    else
                    {
                        return value?.ToString();
                    }
                }

                index++;
            }

            // Getting here is bad ...
            HandleParseError(new MalformedCsvException(GetCurrentRawData(), _nextFieldStart, Math.Max(0, FileRecordIndex), index), ref _nextFieldStart);
            return null;
        }

        /// <summary>
        /// Reads the next record.
        /// </summary>
        /// <returns><see langword="true"/> if a record has been successfully reads; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">The instance has been disposed of.</exception>
        public bool ReadNextRecord()
        {
            return ReadNextRecord(false, false);
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
        /// <param name="caseSensitiveHeaders">
        /// Indicates if the reader should be case-sensitive when parsing headers.
        /// </param>
        /// <returns><see langword="true"/> if a record has been successfully reads; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">The instance has been disposed of.</exception>
        protected virtual bool ReadNextRecord(bool onlyReadHeaders, bool skipToNextLine, bool caseSensitiveHeaders = false)
        {
            if (_eof)
            {
                if (_firstRecordInCache)
                {
                    _firstRecordInCache = false;
                    FileRecordIndex++;

                    return true;
                }
                else
                {
                    return false;
                }
            }

            CheckDisposed();

            if (!_initialized)
            {
                _buffer = new char[BufferSize];

                if (!ReadBuffer())
                {
                    return false;
                }

                if (!SkipEmptyAndCommentedLines(ref _nextFieldStart))
                {
                    return false;
                }

                // Keep growing _fields array until the last field has been found
                // and then resize it to its final correct size

                _fieldCount = 0;
                _fields = new string[16];

                while (ReadField(_fieldCount, true, false) != null)
                {
                    if (ParseErrorFlag)
                    {
                        _fieldCount = 0;
                        Array.Clear(_fields, 0, _fields.Length);
                        ParseErrorFlag = false;
                        _nextFieldIndex = 0;
                    }
                    else
                    {
                        _fieldCount++;

                        if (_fieldCount == _fields.Length)
                        {
                            Array.Resize(ref _fields, (_fieldCount + 1) * 2);
                        }
                    }
                }

                // _fieldCount contains the last field index, but it must contains the field count,
                // so increment by 1
                _fieldCount++;

                if (_fields.Length != _fieldCount)
                {
                    Array.Resize(ref _fields, _fieldCount);
                }

                var headerComparer = caseSensitiveHeaders ? StringComparer.CurrentCulture : StringComparer.CurrentCultureIgnoreCase;
                _fieldHeaderIndexes = new Dictionary<string, int>(_fieldCount, headerComparer);

                _initialized = true;

                // If headers are present, call ReadNextRecord again
                if (HasHeaders)
                {
                    // Don't count first record as it was the headers
                    FileRecordIndex = -1;

                    _firstRecordInCache = false;

                    for (var i = 0; i < _fields.Length; i++)
                    {
                        var headerName = _fields[i];
                        if (string.IsNullOrEmpty(headerName) || headerName.Trim().Length == 0)
                        {
                            headerName = DefaultHeaderName + i;
                        }

                        // Create it if we haven't already set it explicitly
                        var col = Columns.Count < i + 1 ? null : Columns[i];
                        if (col == null)
                        {
                            col = new Column
                            {
                                Name = headerName,
                                // Default to string if not assigned.
                                Type = typeof(string)
                            };

                            int existingIndex;
                            if (_fieldHeaderIndexes.TryGetValue(headerName, out existingIndex))
                            {
                                if (DuplicateHeaderEncountered == null)
                                {
                                    throw new DuplicateHeaderException(headerName, i);
                                }

                                var args = new DuplicateHeaderEventArgs(headerName, i, existingIndex);
                                DuplicateHeaderEncountered(this, args);
                                col.Name = args.HeaderName;
                            }

                            _fieldHeaderIndexes.Add(col.Name, i);
                            // Should be correct as we are going in ascending order.
                            Columns.Add(col);
                        }
                    }

                    // Proceed to first record
                    if (!onlyReadHeaders)
                    {
                        // Calling again ReadNextRecord() seems to be simpler, 
                        // but in fact would probably cause many subtle bugs because a derived class does not expect a recursive behavior
                        // so simply do what is needed here and no more.

                        if (!SkipEmptyAndCommentedLines(ref _nextFieldStart))
                        {
                            return false;
                        }

                        Array.Clear(_fields, 0, _fields.Length);
                        _nextFieldIndex = 0;
                        _eol = false;

                        FileRecordIndex++;
                        return true;
                    }
                }
                else
                {
                    // If we have explicity columne, now build up the reverse dictionary
                    for (var i = 0; i < Columns.Count; i++)
                    {
                        _fieldHeaderIndexes.Add(Columns[i].Name, i);
                    }

                    if (onlyReadHeaders)
                    {
                        _firstRecordInCache = true;
                        FileRecordIndex = -1;
                    }
                    else
                    {
                        _firstRecordInCache = false;
                        FileRecordIndex = 0;
                    }
                }
            }
            else
            {
                if (skipToNextLine)
                {
                    SkipToNewLine(ref _nextFieldStart);
                }
                else if (FileRecordIndex > -1 && !MissingFieldFlag)
                {
                    // If not already at end of record, move there
                    if (!_eol && !_eof)
                    {
                        HandleExtraFieldsInCurrentRecord(_nextFieldStart);
                    }
                }

                if (!_firstRecordInCache && !SkipEmptyAndCommentedLines(ref _nextFieldStart))
                {
                    return false;
                }

                if (HasHeaders || !_firstRecordInCache)
                {
                    _eol = false;
                }

                // Check to see if the first record is in cache.
                // This can happen when initializing a reader with no headers
                // because one record must be read to get the field count automatically
                if (_firstRecordInCache)
                {
                    _firstRecordInCache = false;
                }
                else
                {
                    Array.Clear(_fields, 0, _fields.Length);
                    _nextFieldIndex = 0;
                }

                MissingFieldFlag = false;
                ParseErrorFlag = false;
                FileRecordIndex++;
            }

            return true;
        }

        private void HandleExtraFieldsInCurrentRecord(int currentFieldIndex)
        {
            if (DefaultParseErrorAction == ParseErrorAction.AdvanceToNextLine)
            {
                SkipToNextRecord();
            }
            else
            {
                var exception = new MalformedCsvException(GetCurrentRawData(), _nextFieldStart, Math.Max(0, FileRecordIndex), currentFieldIndex);

                if (DefaultParseErrorAction == ParseErrorAction.RaiseEvent)
                {
                    var e = new ParseErrorEventArgs(exception, ParseErrorAction.ThrowException);
                    OnParseError(e);
                    SkipToNextRecord();
                }
                else if (DefaultParseErrorAction == ParseErrorAction.ThrowException)
                {
                    throw exception;
                }
            }
        }

        private void SkipToNextRecord()
        {
            if (!SupportsMultiline)
            {
                SkipToNewLine(ref _nextFieldStart);
            }
            else
            {
                while (ReadField(_nextFieldIndex, true, true) != null)
                {
                }
            }
        }

        /// <summary>
        /// Skips empty and commented lines.
        /// If the end of the buffer is reached, its content be discarded and filled again from the reader.
        /// </summary>
        /// <param name="pos">
        /// The position in the buffer where to start parsing. 
        /// Will contains the resulting position after the operation.
        /// </param>
        /// <returns><see langword="true"/> if the end of the reader has not been reached; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        ///    The instance has been disposed of.
        /// </exception>
        private bool SkipEmptyAndCommentedLines(ref int pos)
        {
            if (pos < _bufferLength)
            {
                DoSkipEmptyAndCommentedLines(ref pos);
            }

            while (pos >= _bufferLength && !_eof)
            {
                if (ReadBuffer())
                {
                    pos = 0;
                    DoSkipEmptyAndCommentedLines(ref pos);
                }
                else
                {
                    return false;
                }
            }

            return !_eof;
        }

        /// <summary>
        /// <para>Worker method.</para>
        /// <para>Skips empty and commented lines.</para>
        /// </summary>
        /// <param name="pos">
        /// The position in the buffer where to start parsing. 
        /// Will contains the resulting position after the operation.
        /// </param>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">The instance has been disposed of.</exception>
        private void DoSkipEmptyAndCommentedLines(ref int pos)
        {
            while (pos < _bufferLength)
            {
                if (_buffer[pos] == Comment)
                {
                    pos++;
                    SkipToNewLine(ref pos);
                }
                else if (SkipEmptyLines && ParseNewLine(ref pos))
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Skips whitespace characters.
        /// </summary>
        /// <param name="pos">The starting position of the parsing. Will contain the resulting end position.</param>
        /// <returns><see langword="true"/> if the end of the reader has not been reached; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">The instance has been disposed of.</exception>
        private bool SkipWhiteSpaces(ref int pos)
        {
            for (;;)
            {
                while (pos < _bufferLength && IsWhiteSpace(_buffer[pos]))
                {
                    pos++;
                }

                if (pos < _bufferLength)
                {
                    break;
                }
                else
                {
                    pos = 0;

                    if (!ReadBuffer())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Skips ahead to the next NewLine character.
        /// If the end of the buffer is reached, its content be discarded and filled again from the reader.
        /// </summary>
        /// <param name="pos">
        /// The position in the buffer where to start parsing. 
        /// Will contains the resulting position after the operation.
        /// </param>
        /// <returns><see langword="true"/> if the end of the reader has not been reached; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">The instance has been disposed of.</exception>
        private bool SkipToNewLine(ref int pos)
        {
            // ((pos = 0) == 0) is a little trick to reset position inline
            while ((pos < _bufferLength || (ReadBuffer() && ((pos = 0) == 0))) && !ParseNewLine(ref pos))
            {
                pos++;
            }

            return !_eof;
        }

        /// <summary>
        /// Handles a parsing error.
        /// </summary>
        /// <param name="error">The parsing error that occured.</param>
        /// <param name="pos">The current position in the buffer.</param>
        /// <exception cref="ArgumentNullException"><paramref name="error"/> is <see langword="null"/>.</exception>
        private void HandleParseError(MalformedCsvException error, ref int pos)
        {
            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            ParseErrorFlag = true;

            switch (DefaultParseErrorAction)
            {
                case ParseErrorAction.ThrowException:
                    throw error;

                case ParseErrorAction.RaiseEvent:
                    var e = new ParseErrorEventArgs(error, ParseErrorAction.ThrowException);
                    OnParseError(e);

                    switch (e.Action)
                    {
                        case ParseErrorAction.ThrowException:
                            throw e.Error;

                        case ParseErrorAction.RaiseEvent:
                            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, ExceptionMessage.ParseErrorActionInvalidInsideParseErrorEvent, e.Action), e.Error);

                        case ParseErrorAction.AdvanceToNextLine:
                            // already at EOL when fields are missing, so don't skip to next line in that case
                            if (!MissingFieldFlag && pos >= 0)
                            {
                                SkipToNewLine(ref pos);
                            }
                            break;

                        default:
                            throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, ExceptionMessage.ParseErrorActionNotSupported, e.Action), e.Error);
                    }
                    break;

                case ParseErrorAction.AdvanceToNextLine:
                    // already at EOL when fields are missing, so don't skip to next line in that case
                    if (!MissingFieldFlag && pos >= 0)
                    {
                        SkipToNewLine(ref pos);
                    }

                    break;

                default:
                    throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, ExceptionMessage.ParseErrorActionNotSupported, DefaultParseErrorAction), error);
            }
        }

        /// <summary>
        /// Handles a missing field error.
        /// </summary>
        /// <param name="value">The partially parsed value, if available.</param>
        /// <param name="fieldIndex">The missing field index.</param>
        /// <param name="currentPosition">The current position in the raw data.</param>
        /// <returns>
        /// The resulting value according to <see cref="M:MissingFieldAction"/>.
        /// If the action is set to <see cref="T:MissingFieldAction.TreatAsParseError"/>,
        /// then the parse error will be handled according to <see cref="DefaultParseErrorAction"/>.
        /// </returns>
        private string HandleMissingField(string value, int fieldIndex, ref int currentPosition)
        {
            if (fieldIndex < 0 || fieldIndex >= _fieldCount)
            {
                throw new ArgumentOutOfRangeException(nameof(fieldIndex), fieldIndex, string.Format(CultureInfo.InvariantCulture, ExceptionMessage.FieldIndexOutOfRange, fieldIndex));
            }

            MissingFieldFlag = true;

            for (var i = fieldIndex + 1; i < _fieldCount; i++)
            {
                _fields[i] = null;
            }

            if (value != null)
            {
                return value;
            }
            else
            {
                switch (MissingFieldAction)
                {
                    case MissingFieldAction.ParseError:
                        HandleParseError(new MissingFieldCsvException(GetCurrentRawData(), currentPosition, Math.Max(0, FileRecordIndex), fieldIndex), ref currentPosition);
                        return value;

                    case MissingFieldAction.ReplaceByEmpty:
                        return string.Empty;

                    case MissingFieldAction.ReplaceByNull:
                        return null;

                    default:
                        throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, ExceptionMessage.MissingFieldActionNotSupported, MissingFieldAction));
                }
            }
        }

        /// <summary>
        /// Validates the state of the data reader.
        /// </summary>
        /// <param name="validations">The validations to accomplish.</param>
        /// <exception cref="InvalidOperationException">No current record.</exception>
        /// <exception cref="InvalidOperationException">This operation is invalid when the reader is closed.</exception>
        private void ValidateDataReader(DataReaderValidations validations)
        {
            if ((validations & DataReaderValidations.IsInitialized) != 0 && !_initialized)
            {
                throw new InvalidOperationException(ExceptionMessage.NoCurrentRecord);
            }

            if ((validations & DataReaderValidations.IsNotClosed) != 0 && _isDisposed)
            {
                throw new InvalidOperationException(ExceptionMessage.ReaderClosed);
            }
        }

        /// <summary>
        /// Copy the value of the specified field to an array.
        /// </summary>
        /// <param name="field">The index of the field.</param>
        /// <param name="fieldOffset">The offset in the field value.</param>
        /// <param name="destinationArray">The destination array where the field value will be copied.</param>
        /// <param name="destinationOffset">The destination array offset.</param>
        /// <param name="length">The number of characters to copy from the field value.</param>
        /// <returns></returns>
        private long CopyFieldToArray(int field, long fieldOffset, Array destinationArray, int destinationOffset, int length)
        {
            EnsureInitialize();

            if (field < 0 || field >= _fieldCount)
            {
                throw new ArgumentOutOfRangeException(nameof(field), field, string.Format(CultureInfo.InvariantCulture, ExceptionMessage.FieldIndexOutOfRange, field));
            }

            if (fieldOffset < 0 || fieldOffset >= int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(fieldOffset));
            }

            // Array.Copy(...) will do the remaining argument checks

            if (length == 0)
            {
                return 0;
            }

            var value = this[field];

            if (value == null)
            {
                value = string.Empty;
            }

            Debug.Assert(fieldOffset < int.MaxValue);

            Debug.Assert(destinationArray.GetType() == typeof(char[]) || destinationArray.GetType() == typeof(byte[]));

            if (destinationArray.GetType() == typeof(char[]))
            {
                Array.Copy(value.ToCharArray((int) fieldOffset, length), 0, destinationArray, destinationOffset, length);
            }
            else
            {
                var chars = value.ToCharArray((int)fieldOffset, length);
                var source = new byte[chars.Length];

                for (var i = 0; i < chars.Length; i++)
                {
                    source[i] = Convert.ToByte(chars[i]);
                }

                Array.Copy(source, 0, destinationArray, destinationOffset, length);
            }

            return length;
        }

#if !NETSTANDARD1_3
        int System.Data.IDataReader.RecordsAffected
        {
            get
            {
                // For SELECT statements, -1 must be returned.
                return -1;
            }
        }

        bool System.Data.IDataReader.IsClosed
        {
            get
            {
                return _eof;
            }
        }

        bool System.Data.IDataReader.NextResult()
        {
            ValidateDataReader(DataReaderValidations.IsNotClosed);

            return false;
        }

        void System.Data.IDataReader.Close()
        {
            Dispose();
        }

        bool System.Data.IDataReader.Read()
        {
            ValidateDataReader(DataReaderValidations.IsNotClosed);

            return ReadNextRecord();
        }

        int System.Data.IDataReader.Depth
        {
            get
            {
                ValidateDataReader(DataReaderValidations.IsNotClosed);

                return 0;
            }
        }

        System.Data.DataTable System.Data.IDataReader.GetSchemaTable()
        {
            EnsureInitialize();
            ValidateDataReader(DataReaderValidations.IsNotClosed);

            var schema = new DataTable("SchemaTable")
            {
                Locale = CultureInfo.InvariantCulture,
                MinimumCapacity = _fieldCount
            };

            schema.Columns.Add(SchemaTableColumn.AllowDBNull, typeof(bool)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.BaseColumnName, typeof(string)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.BaseSchemaName, typeof(string)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.BaseTableName, typeof(string)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.ColumnName, typeof(string)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.ColumnOrdinal, typeof(int)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.ColumnSize, typeof(int)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.DataType, typeof(object)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.IsAliased, typeof(bool)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.IsExpression, typeof(bool)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.IsKey, typeof(bool)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.IsLong, typeof(bool)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.IsUnique, typeof(bool)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.NumericPrecision, typeof(short)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.NumericScale, typeof(short)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.ProviderType, typeof(int)).ReadOnly = true;

            schema.Columns.Add(SchemaTableOptionalColumn.BaseCatalogName, typeof(string)).ReadOnly = true;
            schema.Columns.Add(SchemaTableOptionalColumn.BaseServerName, typeof(string)).ReadOnly = true;
            schema.Columns.Add(SchemaTableOptionalColumn.IsAutoIncrement, typeof(bool)).ReadOnly = true;
            schema.Columns.Add(SchemaTableOptionalColumn.IsHidden, typeof(bool)).ReadOnly = true;
            schema.Columns.Add(SchemaTableOptionalColumn.IsReadOnly, typeof(bool)).ReadOnly = true;
            schema.Columns.Add(SchemaTableOptionalColumn.IsRowVersion, typeof(bool)).ReadOnly = true;

            // null marks columns that will change for each row
            object[] schemaRow =
            {
                true,                    // 00- AllowDBNull
                null,                    // 01- BaseColumnName
                string.Empty,            // 02- BaseSchemaName
                string.Empty,            // 03- BaseTableName
                null,                    // 04- ColumnName
                null,                    // 05- ColumnOrdinal
                int.MaxValue,            // 06- ColumnSize
                typeof(string),          // 07- DataType
                false,                   // 08- IsAliased
                false,                   // 09- IsExpression
                false,                   // 10- IsKey
                false,                   // 11- IsLong
                false,                   // 12- IsUnique
                DBNull.Value,            // 13- NumericPrecision
                DBNull.Value,            // 14- NumericScale
                (int) DbType.String,     // 15- ProviderType

                string.Empty,            // 16- BaseCatalogName
                string.Empty,            // 17- BaseServerName
                false,                   // 18- IsAutoIncrement
                false,                   // 19- IsHidden
                true,                    // 20- IsReadOnly
                false                    // 21- IsRowVersion
            };

            IList<Column> columns;
            if (Columns.Count > 0)
            {
                columns = Columns;
            }
            else
            {
                columns = new List<Column>();
                for (var i = 0; i < _fieldCount; i++)
                {
                    columns.Add(new Column
                    {
                        Name = DefaultHeaderName + i,
                        Type = typeof(string)
                    });
                }
            }

            for (var i = 0; i < columns.Count; i++)
            {
                schemaRow[1] = columns[i].Name; // Base column name
                schemaRow[4] = columns[i].Name; // Column name
                schemaRow[5] = i;               // Column ordinal
                schemaRow[7] = columns[i].Type; // Data type

                schema.Rows.Add(schemaRow);
            }

            return schema;
        }

        int IDataRecord.GetInt32(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);

            var value = this[i];

            return int.Parse(value ?? string.Empty, CultureInfo.CurrentCulture);
        }

        object IDataRecord.this[string name]
        {
            get
            {
                ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
                return this[name];
            }
        }

        object IDataRecord.this[int i]
        {
            get
            {
                ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
                return FieldValue(i);
            }
        }

        object IDataRecord.GetValue(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);

            return ((IDataRecord)this).IsDBNull(i) ? DBNull.Value : FieldValue(i);
        }

        bool IDataRecord.IsDBNull(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return NullValue == null ? string.IsNullOrEmpty(this[i]) : string.Equals(this[i], NullValue, StringComparison.OrdinalIgnoreCase);
        }

        long IDataRecord.GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);

            return CopyFieldToArray(i, fieldOffset, buffer, bufferoffset, length);
        }

        byte IDataRecord.GetByte(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return byte.Parse(this[i], CultureInfo.CurrentCulture);
        }

        Type IDataRecord.GetFieldType(int i)
        {
            EnsureInitialize();
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);

            if (i < 0 || i >= _fieldCount)
            {
                throw new ArgumentOutOfRangeException(nameof(i), i, string.Format(CultureInfo.InvariantCulture, ExceptionMessage.FieldIndexOutOfRange, i));
            }

            if (Columns == null || i < 0 || i >= Columns.Count)
            {
                return typeof(string);
            }
            var column = Columns[i];
            return column.Type;
        }

        decimal IDataRecord.GetDecimal(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return decimal.Parse(this[i], CultureInfo.CurrentCulture);
        }

        int IDataRecord.GetValues(object[] values)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);

            var record = (IDataRecord)this;

            for (var i = 0; i < _fieldCount; i++)
            {
                values[i] = record.GetValue(i);
            }

            return _fieldCount;
        }

        string IDataRecord.GetName(int i)
        {
            EnsureInitialize();
            ValidateDataReader(DataReaderValidations.IsNotClosed);

            if (i < 0 || i >= FieldCount)
            {
                throw new ArgumentOutOfRangeException(nameof(i), i, string.Format(CultureInfo.InvariantCulture, ExceptionMessage.FieldIndexOutOfRange, i));
            }

            if (i >= Columns.Count)
            {
                return null;
            }

            return Columns[i].Name;
        }

        long IDataRecord.GetInt64(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return Int64.Parse(this[i], CultureInfo.CurrentCulture);
        }

        double IDataRecord.GetDouble(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return Double.Parse(this[i], CultureInfo.CurrentCulture);
        }

        bool IDataRecord.GetBoolean(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);

            var value = this[i];

            if (int.TryParse(value, out var result))
            {
                return (result != 0);
            }

            return bool.Parse(value);
        }

        Guid IDataRecord.GetGuid(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return new Guid(this[i]);
        }

        DateTime IDataRecord.GetDateTime(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return DateTime.Parse(this[i], CultureInfo.CurrentCulture);
        }

        int IDataRecord.GetOrdinal(string name)
        {
            EnsureInitialize();
            ValidateDataReader(DataReaderValidations.IsNotClosed);

            if (!_fieldHeaderIndexes.TryGetValue(name, out var index))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, ExceptionMessage.FieldHeaderNotFound, name), "name");
            }

            return index;
        }

        string IDataRecord.GetDataTypeName(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return typeof(string).FullName;
        }

        float IDataRecord.GetFloat(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return float.Parse(this[i], CultureInfo.CurrentCulture);
        }

        IDataReader IDataRecord.GetData(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);

            return i == 0 ? this : null;
        }

        long IDataRecord.GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);

            return CopyFieldToArray(i, fieldoffset, buffer, bufferoffset, length);
        }

        string IDataRecord.GetString(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return this[i];
        }

        char IDataRecord.GetChar(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return char.Parse(this[i]);
        }

        short IDataRecord.GetInt16(int i)
        {
            ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return short.Parse(this[i], CultureInfo.CurrentCulture);
        }
#endif
        object FieldValue(int i)
        {
            var value = this[i];
            if (Columns == null || i < 0 || i >= Columns.Count)
            {
                return value;
            }
            var column = Columns[i];
            return column.Convert(value);
        }

        /// <summary>
        /// Returns an <see cref="T:RecordEnumerator"/>  that can iterate through CSV records.
        /// </summary>
        /// <returns>An <see cref="T:RecordEnumerator"/>  that can iterate through CSV records.</returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        ///    The instance has been disposed of.
        /// </exception>
        public CsvReader.RecordEnumerator GetEnumerator()
        {
            return new CsvReader.RecordEnumerator(this);
        }

        /// <summary>
        /// Returns an <see cref="T:System.Collections.Generics.IEnumerator"/>  that can iterate through CSV records.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.Generics.IEnumerator"/>  that can iterate through CSV records.</returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        ///    The instance has been disposed of.
        /// </exception>
        IEnumerator<string[]> IEnumerable<string[]>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Returns an <see cref="T:System.Collections.IEnumerator"/>  that can iterate through CSV records.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"/>  that can iterate through CSV records.</returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        ///    The instance has been disposed of.
        /// </exception>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

#if DEBUG
#if !NETSTANDARD1_3
        /// <summary>
        /// Contains the stack when the object was allocated.
        /// </summary>
        private System.Diagnostics.StackTrace _allocStack;
#endif
#endif

        /// <summary>
        /// Contains the disposed status flag.
        /// </summary>
        private bool _isDisposed = false;

        /// <summary>
        /// Contains the locking object for multi-threading purpose.
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// Occurs when the instance is disposed of.
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Gets a value indicating whether the instance has been disposed of.
        /// </summary>
        /// <value>
        ///     <see langword="true"/> if the instance has been disposed of; otherwise, <see langword="false"/>.
        /// </value>
        public bool IsDisposed => _isDisposed;

        /// <summary>
        /// Raises the <see cref="M:Disposed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected virtual void OnDisposed(EventArgs e)
        {
            Disposed?.Invoke(this, e);
        }

        /// <summary>
        /// Checks if the instance has been disposed of, and if it has, throws an <see cref="T:System.ComponentModel.ObjectDisposedException"/>; otherwise, does nothing.
        /// </summary>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        ///     The instance has been disposed of.
        /// </exception>
        /// <remarks>
        ///     Derived classes should call this method at the start of all methods and properties that should not be accessed after a call to <see cref="M:Dispose()"/>.
        /// </remarks>
        protected void CheckDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>
        /// Releases all resources used by the instance.
        /// </summary>
        /// <remarks>
        /// Calls <see cref="M:Dispose(Boolean)"/> with the disposing parameter set to <see langword="true"/> to free unmanaged and managed resources.
        /// </remarks>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Releases the unmanaged resources used by this instance and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            // Refer to http://www.bluebytesoftware.com/blog/PermaLink,guid,88e62cdf-5919-4ac7-bc33-20c06ae539ae.aspx
            // Refer to http://www.gotdotnet.com/team/libraries/whitepapers/resourcemanagement/resourcemanagement.aspx

            // No exception should ever be thrown except in critical scenarios.
            // Unhandled exceptions during finalization will tear down the process.
            if (!_isDisposed)
            {
                try
                {
                    // Dispose-time code should call Dispose() on all owned objects that implement the IDisposable interface. 
                    // "owned" means objects whose lifetime is solely controlled by the container. 
                    // In cases where ownership is not as straightforward, techniques such as HandleCollector can be used.  
                    // Large managed object fields should be nulled out.

                    // Dispose-time code should also set references of all owned objects to null, after disposing them. This will allow the referenced objects to be garbage collected even if not all references to the "parent" are released. It may be a significant memory consumption win if the referenced objects are large, such as big arrays, collections, etc. 
                    if (disposing)
                    {
                        // Acquire a lock on the object while disposing.

                        if (_reader != null)
                        {
                            lock (_lock)
                            {
                                if (_reader != null)
                                {
                                    _reader.Dispose();

                                    _reader = null;
                                    _buffer = null;
                                    _eof = true;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    // Ensure that the flag is set
                    _isDisposed = true;

                    // Catch any issues about firing an event on an already disposed object.
                    try
                    {
                        OnDisposed(EventArgs.Empty);
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the instance is reclaimed by garbage collection.
        /// </summary>
        ~CsvReader()
        {
#if DEBUG
#if !NETSTANDARD1_3
            Debug.WriteLine("FinalizableObject was not disposed" + _allocStack.ToString());
#endif
#endif
            Dispose(false);
        }
    }
}
