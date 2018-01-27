#### 4.0.0 (2018-01-30)
* Support .NET Framework 2.0+ and .NET Standard 1.3+
* Use DateTime.TryParseExact if needed (@milcondoin)
* Override column capability (@spintronic)
* Fix handling of duplicate headers (@jonreis)
* Reduced exception overhead (@spintronic)
* Added NullBytesRemoval functionality to reduce memory usage and runtime (@andrewpsy)
* Fixed: Exception in GetFieldType() when hasHeaders is false (@molopony)
* Fixed: Exception when reading data and hasHeaders is false (@molopony)
* Fixed: ArgumentOutOfRangeException when hasHeaders is false (@molopony)

#### 3.9.1 (2016-04-09)
* Added max field length for quoted fields (@criteo)
* Strengthen Debug.Assert condition in GetSchemaWithoutHeaders (@joshgo)
* Added test for quoted fields not throwing an exception (@joshgo)
* Added HasHeader function to allow checking if a header existing without throwing exception (@kiran94)

#### 3.9 (2015-06-20)
* Completely replaced use of _fieldHeaders internally with Columns
* Fixed IDataReader.GetSchemaTable to return Column.Type rather than always saying string
* Introduced UseColumnDefault to allow Column.Default value to be returned when a column doesn't exist in the source data.
* Changing handling of extra fields to respect DefaultParseErrorAction (thanks to @gap777)
* Added support for null values and some tests (thanks to @fretje)

#### 3.8.3 (2015-03-21)
* Can now handle binary and varbinary columsn (thanks to @fretje)

#### 3.8.2 (2014-02-27)
* Adding Column metadata so we can use with SqlBulkCopy.

#### 3.8.1 (2011-11-10)
* Fixed bug with missing field handling.
* Converted solution to VS 2010 (still targets .NET 2.0).

#### 3.8 (2011-07-05)
* Empty header names in CSV files are now replaced by a default name that can be customized via the new DefaultHeaderName property (by default, it is "Column" + column index).

#### 3.7.2 (2011-05-17)
* Fixed a bug when handling missing fields.
* Strongly named the main assembly.

#### 3.7.1 (2010-11-03)
* Fixed a bug when handling whitespaces at the end of a file.

#### 3.7 (2010-03-30)
* Breaking: Added more field value trimming options.

#### 3.6.2 (2008-10-09)
* Fixed a bug when calling MoveTo in a particular action sequence;
* Fixed a bug when extra fields are present in a multiline record;
* Fixed a bug when there is a parse error while initializing.

#### 3.6.1 (2008-07-16)
* Fixed a bug with RecordEnumerator caused by reusing the same array over each iteration.

#### 3.6 (2008-07-09)
* Added a web demo project;
* Fixed a bug when loading CachedCsvReader into a DataTable and the CSV has no header.

#### 3.5 (2007-11-28)
* Fixed a bug when initializing CachedCsvReader without having read a record first.

#### 3.4 (2007-10-23)
* Fixed a bug with the IDataRecord implementation where GetValue/GetValues should return DBNull.Value when the field value is empty or null;
* Fixed a bug where no exception is raised if a delimiter is not present after a non final quoted field;
* Fixed a bug when trimming unquoted fields and whitespaces span over two buffers.

#### 3.3 (2007-01-14)
* Added the option to turn off skipping empty lines via the property SkipEmptyLines (on by default);
* Fixed a bug with the handling of a delimiter at the end of a record preceded by a quoted field.

#### 3.2 (2006-12-11)
* Slightly modified the way missing fields are handled;
* Fixed a bug where the call to CsvReader.ReadNextRecord() would return false for a CSV file containing only one line ending with a new line character and no header.

#### 3.1.2 (2006-08-06)
* Updated dispose pattern;
* Fixed a bug when SupportsMultiline is false;
* Fixed a bug where the IDataReader schema column "DataType" returned DbType.String instead of typeof(string).

#### 3.1.1 (2006-07-25)
* Added a SupportsMultiline property to help boost performance when multi-line support is not needed;
* Added two new constructors to support common scenarios;
* Added support for when the base stream returns a length of 0;
* Fixed a bug when the FieldCount property is accessed before having read any record;
* Fixed a bug when the delimiter is a whitespace;
* Fixed a bug in ReadNextRecord(...) by eliminating its recursive behavior when initializing headers;
* Fixed a bug when EOF is reached when reading the first record;
* Fixed a bug where no exception would be thrown if the reader has reached EOF and a field is missing.

#### 3.0 (2006-05-15)
* Introduced equal support for .NET 1.1 and .NET 2.0;
* Added extensive support for malformed CSV files;
* Added complete support for data-binding;
* Made available the current raw data;
* Field headers are now accessed via an array (breaking change);
* Made field headers case insensitive (thanks to Marco Dissel for the suggestion);
* Relaxed restrictions when the reader has been disposed;
* CsvReader supports 2^63 records;
* Added more test coverage;
* Upgraded to .NET 2.0 release version;
* Fixed an issue when accessing certain properties without having read any data (notably FieldHeaders).

#### 2.0 (2005-08-10)
* Ported code to .NET 2.0 (July 2005 CTP);
* Thoroughly debugged via extensive unit testing (special thanks to shriop);
* Improved speed (now 15 times faster than OLEDB);
* Consumes half the memory than version 1.0;
* Can specify a custom buffer size;
* Full Unicode support;
* Auto-detects line ending, be it \r, \n, or \r\n;
* Better exception handling;
* Supports the "field1\rfield2\rfield3\n" pattern (used by Unix);
* Parsing code completely refactored, resulting in much cleaner code.

#### 1.1 (2005-01-15)
* 1.1: Added support for multi-line fields.

#### 1.0 (2005-01-09)
* 1.0: First release.