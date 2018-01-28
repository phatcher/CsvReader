using System.IO;

using LumenWorks.Framework.IO.Csv;

namespace CsvReaderBenchmarks
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
            else if (args.Length == 2)
            {
                Run((string) args[0], (int) args[1]);
            }
            else
            {
                return RunWithNullRemoval((string) args[0], (int) args[1], (bool) args[2]);
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
                        {
                            s = csv[i];
                        }
                    }
                }
                else
                {
                    while (csv.ReadNextRecord())
                    {
                        s = csv[field];
                    }
                }
            }
        }

        private static string RunWithNullRemoval(string path, int field, bool addMark)
        {
#if NETCOREAPP1_0
            var fileStream = new FileStream(path, FileMode.Open);
            using (var stream = new StreamReader(fileStream))
#else
            using (var stream = new StreamReader(path))
#endif
            using (var csv = new CsvReader(stream.BaseStream, false, stream.CurrentEncoding, addMark))
            {
                csv.SupportsMultiline = false;
                string cell = string.Empty;

                if (field == -1)
                {
                    while (csv.ReadNextRecord())
                    {
                        for (int i = 0; i < csv.FieldCount; i++)
                        {
                            cell = csv[i];
                        }
                    }
                    return string.Format(@"AddMark =({0}) LastCell =({1})", addMark, cell);
                }
                else
                {
                    while (csv.ReadNextRecord())
                    {
                        cell = csv[field];
                    }
                }
            }
            return string.Empty;
        }
    }
}
