#region Using directives

using System;
using System.IO;
using System.Text;

using Microsoft.VisualBasic.FileIO;

#endregion

namespace CsvReaderDemo
{
	public sealed class TextFieldParserBenchmark
	{
		private TextFieldParserBenchmark()
		{
		}

		public static object Run(object[] args)
		{
			if (args.Length == 1)
				Run((string) args[0]);
			else
				Run((string) args[0], (int) args[1]);

			return null;
		}

		public static void Run(string path)
		{
			Run(path, -1);
		}

		public static void Run(string path, int field)
		{
			using (TextFieldParser csv = new TextFieldParser(new StreamReader(path)))
			{
				csv.TextFieldType = FieldType.Delimited;
				csv.TrimWhiteSpace = true;
				csv.HasFieldsEnclosedInQuotes = true;
				csv.Delimiters = new string[] { "," };

				string[] fields;
				while ((fields = csv.ReadFields()) != null)
				{
				}
			}
		}
	}
}
