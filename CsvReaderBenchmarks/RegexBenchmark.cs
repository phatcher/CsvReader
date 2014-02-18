#region Using directives

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace CsvReaderDemo
{
	public sealed class RegexBenchmark
	{
		private RegexBenchmark()
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
			Regex regex = new Regex(@"
				\G(^|,)
				""
				(?<field> (?> [^""]*) (?> """" [^""]* )* )
				""
				|
				(?<field> [^"",]* )",
				RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

			int fieldGroupIndex = regex.GroupNumberFromName("field");

			using (StreamReader csv = new StreamReader(path))
			{
				string s;

				if (field == -1)
				{
					while ((s = csv.ReadLine()) != null)
					{
						MatchCollection m = regex.Matches(s);

						for (int i = 0; i < m.Count; i += 2)
							s = m[i].Groups[fieldGroupIndex].Value;
					}
				}
				else
				{
					while ((s = csv.ReadLine()) != null)
					{
						MatchCollection m = regex.Matches(s);

						s = m[field << 1].Groups[fieldGroupIndex].Value;
					}
				}
			}
		}
	}
}
