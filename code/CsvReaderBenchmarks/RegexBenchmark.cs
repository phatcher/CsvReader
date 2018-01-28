using System.IO;
using System.Text.RegularExpressions;

namespace CsvReaderBenchmarks
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
            var regex = new Regex(@"
                \G(^|,)
                ""
                (?<field> (?> [^""]*) (?> """" [^""]* )* )
                ""
                |
                (?<field> [^"",]* )",
                RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

            var fieldGroupIndex = regex.GroupNumberFromName("field");

#if NETCOREAPP1_0
            var fileStream = new FileStream(path, FileMode.Open);
            using (var csv = new StreamReader(fileStream))
#else
            using (var csv = new StreamReader(path))
#endif
            {
                string s;

                if (field == -1)
                {
                    while ((s = csv.ReadLine()) != null)
                    {
                        var m = regex.Matches(s);

                        for (var i = 0; i < m.Count; i += 2)
                        {
                            s = m[i].Groups[fieldGroupIndex].Value;
                        }
                    }
                }
                else
                {
                    while ((s = csv.ReadLine()) != null)
                    {
                        var m = regex.Matches(s);

                        s = m[field << 1].Groups[fieldGroupIndex].Value;
                    }
                }
            }
        }
    }
}