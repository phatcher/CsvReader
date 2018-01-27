using System;
using System.IO;
using System.Text;

using LumenWorks.Framework.IO.Csv;

namespace CsvReaderDemo
{
	public sealed class CsvReaderBenchmark
	{
		private CsvReaderBenchmark()
		{
		}

		public static object Run(object[] args)
		{
		    if (args.Length == 1)
		    {
		        Run((string) args[0]);
		    }
		    else
		    {
		        Run((string) args[0], (int) args[1]);
		    }

		    return null;
		}

		public static void Run(string path)
		{
			Run(path, -1);
		}

		public static void Run(string path, int field)
		{
#if NETCOREAPP1_0
		    var fileStream = new FileStream(path, FileMode.Open);
		    using (var csv = new CsvReader(new StreamReader(fileStream), false))
#else
            using (var csv = new CsvReader(new StreamReader(path), false))
#endif
			{
				csv.SupportsMultiline = false;
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
