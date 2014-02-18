#region Using directives

using System;
using System.IO;
using System.Text;

using LumenWorks.Framework.IO.Csv;

#endregion

namespace CsvReaderDemo
{
	public sealed class CachedCsvReaderBenchmark
	{
		private CachedCsvReaderBenchmark()
		{
		}

		public static object Run1(object[] args)
		{
			if (args.Length == 1)
				return Run1((string) args[0]);
			else
				return Run1((string) args[0], (int) args[1]);
		}

		public static object Run2(object[] args)
		{
			if (args.Length == 1)
				Run2((CachedCsvReader) args[0]);
			else
				Run2((CachedCsvReader) args[0], (int) args[1]);

			return null;
		}

		public static CachedCsvReader Run1(string path)
		{
			return Run1(path, -1);
		}

		public static CachedCsvReader Run1(string path, int field)
		{
			CachedCsvReader csv = new CachedCsvReader(new StreamReader(path), false);

			string s;

			if (field == -1)
			{
				while (csv.ReadNextRecord())
				{
					for (int i = 0; i < csv.FieldCount; i++)
						s = csv[i];
				}
			}
			else
			{
				while (csv.ReadNextRecord())
					s = csv[field];
			}

			return csv;
		}

		public static void Run2(CachedCsvReader csv)
		{
			Run2(csv, -1);
		}

		public static void Run2(CachedCsvReader csv, int field)
		{
			using (csv)
			{
				string s;

				if (field == -1)
				{
					while (csv.ReadNextRecord())
					{
						for (int i = 0; i < csv.FieldCount; i++)
							s = csv[i];
					}
				}
				else
				{
					while (csv.ReadNextRecord())
						s = csv[field];
				}
			}
		}
		
	}
}
