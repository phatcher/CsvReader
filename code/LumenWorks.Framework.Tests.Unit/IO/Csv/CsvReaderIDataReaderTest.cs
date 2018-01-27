//	LumenWorks.Framework.Tests.Unit.IO.CSV.CsvReaderIDataReaderTest
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
//	copies or substantial portions of the Software.http://scottchacon.com/2011/08/31/github-flow.html
//
//	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//	INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
//	PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
//	FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//	ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
#if !NETCOREAPP1_0
using System.Data;
#endif
using System.Globalization;
using System.IO;

using NUnit.Framework;

using LumenWorks.Framework.IO.Csv;

namespace LumenWorks.Framework.Tests.Unit.IO.Csv
{
#if !NETCOREAPP1_0
    [TestFixture]
	public class CsvReaderIDataReaderTest
	{
	    [Test]
		public void CloseTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
			{
				IDataReader reader = csv;

				csv.ReadNextRecord();

				reader.Close();

				Assert.IsTrue(reader.IsClosed);
				Assert.IsTrue(csv.IsDisposed);
			}
		}

		[Test]
		public void GetSchemaTableWithHeadersTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
			{
				IDataReader reader = csv;

				var schema = reader.GetSchemaTable();

				Assert.AreEqual(CsvReaderSampleData.SampleData1RecordCount, schema.Rows.Count);

				foreach (DataColumn column in schema.Columns)
				{
					Assert.IsTrue(column.ReadOnly);
				}

				for (var index = 0; index < schema.Rows.Count; index++)
				{
					var column = schema.Rows[index];

					Assert.AreEqual(int.MaxValue, column["ColumnSize"]);
					Assert.AreEqual(DBNull.Value, column["NumericPrecision"]);
					Assert.AreEqual(DBNull.Value, column["NumericScale"]);
					Assert.AreEqual(false, column["IsUnique"]);
					Assert.AreEqual(false, column["IsKey"]);
					Assert.AreEqual(string.Empty, column["BaseServerName"]);
					Assert.AreEqual(string.Empty, column["BaseCatalogName"]);
					Assert.AreEqual(string.Empty, column["BaseSchemaName"]);
					Assert.AreEqual(string.Empty, column["BaseTableName"]);
					Assert.AreEqual(typeof(string), column["DataType"]);
					Assert.AreEqual(true, column["AllowDBNull"]);
					Assert.AreEqual((int) DbType.String, column["ProviderType"]);
					Assert.AreEqual(false, column["IsAliased"]);
					Assert.AreEqual(false, column["IsExpression"]);
					Assert.AreEqual(false, column["IsAutoIncrement"]);
					Assert.AreEqual(false, column["IsRowVersion"]);
					Assert.AreEqual(false, column["IsHidden"]);
					Assert.AreEqual(false, column["IsLong"]);
					Assert.AreEqual(true, column["IsReadOnly"]);

					Assert.AreEqual(index, column["ColumnOrdinal"]);

					switch (index)
					{
						case 0:
							Assert.AreEqual(CsvReaderSampleData.SampleData1Header0, column["ColumnName"]);
							Assert.AreEqual(CsvReaderSampleData.SampleData1Header0, column["BaseColumnName"]);
							break;
						case 1:
							Assert.AreEqual(CsvReaderSampleData.SampleData1Header1, column["ColumnName"]);
							Assert.AreEqual(CsvReaderSampleData.SampleData1Header1, column["BaseColumnName"]);
							break;
						case 2:
							Assert.AreEqual(CsvReaderSampleData.SampleData1Header2, column["ColumnName"]);
							Assert.AreEqual(CsvReaderSampleData.SampleData1Header2, column["BaseColumnName"]);
							break;
						case 3:
							Assert.AreEqual(CsvReaderSampleData.SampleData1Header3, column["ColumnName"]);
							Assert.AreEqual(CsvReaderSampleData.SampleData1Header3, column["BaseColumnName"]);
							break;
						case 4:
							Assert.AreEqual(CsvReaderSampleData.SampleData1Header4, column["ColumnName"]);
							Assert.AreEqual(CsvReaderSampleData.SampleData1Header4, column["BaseColumnName"]);
							break;
						case 5:
							Assert.AreEqual(CsvReaderSampleData.SampleData1Header5, column["ColumnName"]);
							Assert.AreEqual(CsvReaderSampleData.SampleData1Header5, column["BaseColumnName"]);
							break;
						default:
							throw new IndexOutOfRangeException();
					}
				}
			}
		}

		[Test]
		public void GetSchemaTableWithoutHeadersTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), false))
			{
				IDataReader reader = csv;

				var schema = reader.GetSchemaTable();

				Assert.AreEqual(CsvReaderSampleData.SampleData1RecordCount, schema.Rows.Count);

				foreach (DataColumn column in schema.Columns)
				{
					Assert.IsTrue(column.ReadOnly);
				}

				for (var index = 0; index < schema.Rows.Count; index++)
				{
					var column = schema.Rows[index];

					Assert.AreEqual(int.MaxValue, column["ColumnSize"]);
					Assert.AreEqual(DBNull.Value, column["NumericPrecision"]);
					Assert.AreEqual(DBNull.Value, column["NumericScale"]);
					Assert.AreEqual(false, column["IsUnique"]);
					Assert.AreEqual(false, column["IsKey"]);
					Assert.AreEqual(string.Empty, column["BaseServerName"]);
					Assert.AreEqual(string.Empty, column["BaseCatalogName"]);
					Assert.AreEqual(string.Empty, column["BaseSchemaName"]);
					Assert.AreEqual(string.Empty, column["BaseTableName"]);
					Assert.AreEqual(typeof(string), column["DataType"]);
					Assert.AreEqual(true, column["AllowDBNull"]);
					Assert.AreEqual((int) DbType.String, column["ProviderType"]);
					Assert.AreEqual(false, column["IsAliased"]);
					Assert.AreEqual(false, column["IsExpression"]);
					Assert.AreEqual(false, column["IsAutoIncrement"]);
					Assert.AreEqual(false, column["IsRowVersion"]);
					Assert.AreEqual(false, column["IsHidden"]);
					Assert.AreEqual(false, column["IsLong"]);
					Assert.AreEqual(true, column["IsReadOnly"]);

					Assert.AreEqual(index, column["ColumnOrdinal"]);

					Assert.AreEqual("Column" + index.ToString(CultureInfo.InvariantCulture), column["ColumnName"]);
					Assert.AreEqual("Column" + index.ToString(CultureInfo.InvariantCulture), column["BaseColumnName"]);
				}
			}
		}

		[Test]
		public void GetSchemaTableReaderClosedTest()
		{
		    Assert.Throws<InvalidOperationException>(() =>
		    {
		        using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
		        {
		            IDataReader reader = csv;
		            csv.ReadNextRecord();
		            reader.Close();

		            var result = reader.GetSchemaTable();
		        }
            });
		}

		[Test]
		public void NextResultTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
			{
				IDataReader reader = csv;
				Assert.IsFalse(reader.NextResult());

				csv.ReadNextRecord();
				Assert.IsFalse(reader.NextResult());
			}
		}

		[Test]
		public void NextResultReaderClosedTest()
		{
		    Assert.Throws<InvalidOperationException>(() =>
		    {
		        using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
		        {
		            IDataReader reader = csv;
		            csv.ReadNextRecord();
		            reader.Close();

		            var result = reader.NextResult();
		        }
            });
		}

		[Test]
		public void ReadTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
			{
				IDataReader reader = csv;

			    for (var i = 0; i < CsvReaderSampleData.SampleData1RecordCount; i++)
			    {
			        Assert.IsTrue(reader.Read());
			    }

			    Assert.IsFalse(reader.Read());
			}
		}

		[Test]
		public void ReadReaderClosedTest()
		{
		    Assert.Throws<InvalidOperationException>(() =>
		    {
		        using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
		        {
		            IDataReader reader = csv;
		            csv.ReadNextRecord();
		            reader.Close();

		            var result = reader.Read();
		        }
            });
		}

		[Test]
		public void DepthTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
			{
				IDataReader reader = csv;
				Assert.AreEqual(0, reader.Depth);

				csv.ReadNextRecord();
				Assert.AreEqual(0, reader.Depth);
			}
		}

		[Test]
		public void DepthReaderClosedTest()
		{
		    Assert.Throws<InvalidOperationException>(() =>
		    {
		        using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
		        {
		            IDataReader reader = csv;
		            csv.ReadNextRecord();
		            reader.Close();

		            var result = reader.Depth;
		        }
            });
		}

		[Test]
		public void IsClosedTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
			{
				IDataReader reader = csv;
				Assert.IsFalse(reader.IsClosed);

				csv.ReadNextRecord();
				Assert.IsFalse(reader.IsClosed);

				reader.Close();
				Assert.IsTrue(reader.IsClosed);
			}
		}

		[Test]
		public void RecordsAffectedTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
			{
				IDataReader reader = csv;
				Assert.AreEqual(-1, reader.RecordsAffected);

				csv.ReadNextRecord();
				Assert.AreEqual(-1, reader.RecordsAffected);

				reader.Close();
				Assert.AreEqual(-1, reader.RecordsAffected);
			}
		}

	    [Test]
		public void GetBooleanTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleTypedData1), true))
			{
				IDataReader reader = csv;

				var value = true;
				while (reader.Read())
				{
					Assert.AreEqual(value, reader.GetBoolean(reader.GetOrdinal(typeof(bool).FullName)));
				}
			}
		}

		[Test]
		public void GetByteTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleTypedData1), true))
			{
				IDataReader reader = csv;

				byte value = 1;
				while (reader.Read())
				{
					Assert.AreEqual(value, reader.GetByte(reader.GetOrdinal(typeof(byte).FullName)));
				}
			}
		}

		[Test]
		public void GetBytesTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleTypedData1), true))
			{
				IDataReader reader = csv;

				var temp = "abc".ToCharArray();
				var value = new byte[temp.Length];

				for (var i = 0; i < temp.Length; i++)
					value[i] = Convert.ToByte(temp[i]);

				while (reader.Read())
				{
					var csvValue = new byte[value.Length];

					var count = reader.GetBytes(reader.GetOrdinal(typeof(string).FullName), 0, csvValue, 0, value.Length);

					Assert.AreEqual(value.Length, count);
					Assert.AreEqual(value.Length, csvValue.Length);

					for (var i = 0; i < value.Length; i++)
						Assert.AreEqual(value[i], csvValue[i]);
				}
			}
		}

		[Test]
		public void GetCharTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleTypedData1), true))
			{
				IDataReader reader = csv;

				var value = 'a';
				while (reader.Read())
				{
					Assert.AreEqual(value, reader.GetChar(reader.GetOrdinal(typeof(char).FullName)));
				}
			}
		}

		[Test]
		public void GetCharsTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleTypedData1), true))
			{
				IDataReader reader = csv;

				var value = "abc".ToCharArray();
				while (reader.Read())
				{
					var csvValue = new char[value.Length];

					var count = reader.GetChars(reader.GetOrdinal(typeof(string).FullName), 0, csvValue, 0, value.Length);

					Assert.AreEqual(value.Length, count);
					Assert.AreEqual(value.Length, csvValue.Length);

					for (var i = 0; i < value.Length; i++)
						Assert.AreEqual(value[i], csvValue[i]);
				}
			}
		}

		[Test]
		public void GetDataTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleTypedData1), true))
			{
				IDataReader reader = csv;

				while (reader.Read())
				{
					Assert.AreSame(csv, reader.GetData(0));

					for (var i = 1; i < reader.FieldCount; i++)
						Assert.IsNull(reader.GetData(i));
				}
			}
		}

		[Test]
		public void GetDataTypeNameTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleTypedData1), true))
			{
				IDataReader reader = csv;

				while (reader.Read())
				{
					for (var i = 0; i < reader.FieldCount; i++)
						Assert.AreEqual(typeof(string).FullName, reader.GetDataTypeName(i));
				}
			}
		}

		[Test]
		public void GetDateTimeTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleTypedData1), true))
			{
				IDataReader reader = csv;

				var value = new DateTime(2001, 1, 1);
				while (reader.Read())
				{
					Assert.AreEqual(value, reader.GetDateTime(reader.GetOrdinal(typeof(DateTime).FullName)));
				}
			}
		}

		[Test]
		public void GetDecimalTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleTypedData1), true))
			{
				IDataReader reader = csv;

				decimal value = 1;
				while (reader.Read())
				{
					Assert.AreEqual(value, reader.GetDecimal(reader.GetOrdinal(typeof(decimal).FullName)));
				}
			}
		}

		[Test]
		public void GetDoubleTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleTypedData1), true))
			{
				IDataReader reader = csv;

				double value = 1;
				while (reader.Read())
				{
					Assert.AreEqual(value, reader.GetDouble(reader.GetOrdinal(typeof(double).FullName)));
				}
			}
		}

		[Test]
		public void GetFieldTypeTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleTypedData1), true))
			{
				IDataReader reader = csv;

				while (reader.Read())
				{
					for (var i = 0; i < reader.FieldCount; i++)
						Assert.AreEqual(typeof(string), reader.GetFieldType(i));
				}
			}
		}

		[Test]
		public void GetFloatTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleTypedData1), true))
			{
				IDataReader reader = csv;

				float value = 1;
				while (reader.Read())
				{
					Assert.AreEqual(value, reader.GetFloat(reader.GetOrdinal(typeof(float).FullName)));
				}
			}
		}

		[Test]
		public void GetGuidTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleTypedData1), true))
			{
				IDataReader reader = csv;

				var value = new Guid("{11111111-1111-1111-1111-111111111111}");
				while (reader.Read())
				{
					Assert.AreEqual(value, reader.GetGuid(reader.GetOrdinal(typeof(Guid).FullName)));
				}
			}
		}

		[Test]
		public void GetInt16Test()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleTypedData1), true))
			{
				IDataReader reader = csv;

				short value = 1;
				while (reader.Read())
				{
					Assert.AreEqual(value, reader.GetInt16(reader.GetOrdinal(typeof(short).FullName)));
				}
			}
		}

		[Test]
		public void GetInt32Test()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleTypedData1), true))
			{
				IDataReader reader = csv;

				var value = 1;
				while (reader.Read())
				{
					Assert.AreEqual(value, reader.GetInt32(reader.GetOrdinal(typeof(int).FullName)));
				}
			}
		}

		[Test]
		public void GetInt64Test()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleTypedData1), true))
			{
				IDataReader reader = csv;

				long value = 1;
				while (reader.Read())
				{
					Assert.AreEqual(value, reader.GetInt64(reader.GetOrdinal(typeof(long).FullName)));
				}
			}
		}

		[Test]
		public void GetNameTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
			{
				IDataReader reader = csv;

				while (reader.Read())
				{
					Assert.AreEqual(CsvReaderSampleData.SampleData1Header0, reader.GetName(0));
					Assert.AreEqual(CsvReaderSampleData.SampleData1Header1, reader.GetName(1));
					Assert.AreEqual(CsvReaderSampleData.SampleData1Header2, reader.GetName(2));
					Assert.AreEqual(CsvReaderSampleData.SampleData1Header3, reader.GetName(3));
					Assert.AreEqual(CsvReaderSampleData.SampleData1Header4, reader.GetName(4));
					Assert.AreEqual(CsvReaderSampleData.SampleData1Header5, reader.GetName(5));
				}
			}
		}

		[Test]
		public void GetOrdinalTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
			{
				IDataReader reader = csv;

				while (reader.Read())
				{
					Assert.AreEqual(0, reader.GetOrdinal(CsvReaderSampleData.SampleData1Header0));
					Assert.AreEqual(1, reader.GetOrdinal(CsvReaderSampleData.SampleData1Header1));
					Assert.AreEqual(2, reader.GetOrdinal(CsvReaderSampleData.SampleData1Header2));
					Assert.AreEqual(3, reader.GetOrdinal(CsvReaderSampleData.SampleData1Header3));
					Assert.AreEqual(4, reader.GetOrdinal(CsvReaderSampleData.SampleData1Header4));
					Assert.AreEqual(5, reader.GetOrdinal(CsvReaderSampleData.SampleData1Header5));
				}
			}
		}

		[Test]
		public void GetStringTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleTypedData1), true))
			{
				IDataReader reader = csv;

				var value = "abc";
				while (reader.Read())
				{
					Assert.AreEqual(value, reader.GetString(reader.GetOrdinal(typeof(string).FullName)));
				}
			}
		}

		[Test]
		public void GetValueTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
			{
				IDataReader reader = csv;

				var values = new string[CsvReaderSampleData.SampleData1RecordCount];

				while (reader.Read())
				{
					for (var i = 0; i < reader.FieldCount; i++)
					{
						var value = reader.GetValue(i);

						if (string.IsNullOrEmpty(csv[i]))
							Assert.AreEqual(DBNull.Value, value);

						values[i] = value.ToString();
					}

					CsvReaderSampleData.CheckSampleData1(csv.HasHeaders, csv.CurrentRecordIndex, values);
				}
			}
		}

		[Test]
		public void GetValuesTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
			{
				IDataReader reader = csv;

				var objValues = new object[CsvReaderSampleData.SampleData1RecordCount];
				var values = new string[CsvReaderSampleData.SampleData1RecordCount];

				while (reader.Read())
				{
					Assert.AreEqual(CsvReaderSampleData.SampleData1RecordCount, reader.GetValues(objValues));

					for (var i = 0; i < reader.FieldCount; i++)
					{
					    if (string.IsNullOrEmpty(csv[i]))
					    {
					        Assert.AreEqual(DBNull.Value, objValues[i]);
					    }

					    values[i] = objValues[i].ToString();
					}

					CsvReaderSampleData.CheckSampleData1(csv.HasHeaders, csv.CurrentRecordIndex, values);
				}
			}
		}

		[Test]
		public void IsDBNullTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleTypedData1), true))
			{
				IDataReader reader = csv;

				while (reader.Read())
				{
					Assert.IsTrue(reader.IsDBNull(reader.GetOrdinal(typeof(DBNull).FullName)));
				}
			}
		}

		[Test]
		public void IsDBNullWithNullValueTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleTypedData1), true, 
				CsvReader.DefaultDelimiter, CsvReader.DefaultQuote, CsvReader.DefaultEscape, CsvReader.DefaultComment,
				ValueTrimmingOptions.UnquotedOnly, CsvReaderSampleData.SampleNullValue))
			{
				IDataReader reader = csv;

				while (reader.Read())
				{
					Assert.IsTrue(reader.IsDBNull(reader.GetOrdinal(CsvReaderSampleData.DbNullWithNullValueHeader)));
					Assert.IsFalse(reader.IsDBNull(reader.GetOrdinal(typeof(DBNull).FullName)));
				}
			}
		}

		[Test]
		public void FieldCountTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
			{
				IDataReader reader = csv;

				Assert.AreEqual(CsvReaderSampleData.SampleData1RecordCount, reader.FieldCount);
			}
		}

		[Test]
		public void IndexerByFieldNameTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
			{
				IDataReader reader = csv;

				var values = new string[CsvReaderSampleData.SampleData1RecordCount];

				while (reader.Read())
				{
					values[0] = (string) reader[CsvReaderSampleData.SampleData1Header0];
					values[1] = (string) reader[CsvReaderSampleData.SampleData1Header1];
					values[2] = (string) reader[CsvReaderSampleData.SampleData1Header2];
					values[3] = (string) reader[CsvReaderSampleData.SampleData1Header3];
					values[4] = (string) reader[CsvReaderSampleData.SampleData1Header4];
					values[5] = (string) reader[CsvReaderSampleData.SampleData1Header5];

					CsvReaderSampleData.CheckSampleData1(csv.HasHeaders, csv.CurrentRecordIndex, values);
				}
			}
		}

		[Test]
		public void IndexerByFieldIndexTest()
		{
			using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
			{
				IDataReader reader = csv;

				var values = new string[CsvReaderSampleData.SampleData1RecordCount];

				while (reader.Read())
				{
					for (var i = 0; i < reader.FieldCount; i++)
						values[i] = (string) reader[i];

					CsvReaderSampleData.CheckSampleData1(csv.HasHeaders, csv.CurrentRecordIndex, values);
				}
			}
		}

		[Test]
		public void HasNoHeadersTest()
		{
			using (IDataReader reader = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), false))
			{
				var recordCount = 0;
				while (reader.Read())
				{
					var values = new object[reader.FieldCount];
					reader.GetValues(values);
					reader.GetFieldType(0);
					recordCount++;
				}

				Assert.AreNotEqual(0, recordCount);
			}
		}
	}
#endif
}