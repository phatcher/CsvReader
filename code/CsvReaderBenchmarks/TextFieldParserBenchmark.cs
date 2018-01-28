using System.IO;

namespace CsvReaderBenchmarks
{
#if !NETCOREAPP1_0 && !NETCOREAPP2_0
    public sealed class TextFieldParserBenchmark
    {
        private TextFieldParserBenchmark()
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
            using (var csv = new Microsoft.VisualBasic.FileIO.TextFieldParser(new StreamReader(path)))
            {
                csv.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
                csv.TrimWhiteSpace = true;
                csv.HasFieldsEnclosedInQuotes = true;
                csv.Delimiters = new [] { "," };

                string[] fields;
                while ((fields = csv.ReadFields()) != null)
                {
                }
            }

        }
    }
#endif
}