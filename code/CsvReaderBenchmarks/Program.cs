using System;
using System.IO;

namespace CsvReaderBenchmarks
{
    internal class Program
    {
        // StopWatch seems to not be accurate enough to be used here (divisions by zero occur when calculating MB/s).

        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        [STAThread()]
        private static void Main(string[] args)
        {
            const string TestFile2 = @"test2.csv";
            const string TestFile3 = @"test3.csv";

#if !NETCOREAPP1_0
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif

            if (args.Length > 0)
            {
                if (args.Length == 1)
                {
                    switch (args[0].ToUpperInvariant())
                    {
                        case "CSVREADER":
                            CsvReaderBenchmark.Run(TestFile3);
                            return;

                        case "NULLREMOVAL":
                            PerformanceTestWithNullRemovalStreamReader();
                            return;

                        case "STRINGBUILDER":
                            PerformanceTestWithStringBuilder();
                            return;

#if !NETCOREAPP1_0 && !NETCOREAPP2_0
                        case "OLEDB":
                            OleDbBenchmark.Run(TestFile3);
                            return;
#endif
                        case "REGEX":
                            RegexBenchmark.Run(TestFile3);
                            return;

                        default:
                            Console.WriteLine(@"Possible values: CsvReader, NullRemoval, StringBuilder, OleDb, Regex");
                            break;
                    }
                }

                return;
            }

            const int field = 72;
            var fileSize = new FileInfo(TestFile2).Length / 1024 / 1024;

            for (var i = 1; i < 4; i++)
            {
                Console.WriteLine("Test pass #{0} - All fields\n", i);

                DoTest("CsvReader - No cache", fileSize, CsvReaderBenchmark.Run, TestFile2);
#if !NETCOREAPP1_0
                object csv = DoTest("CachedCsvReader - Run 1", fileSize, CachedCsvReaderBenchmark.Run1, TestFile2);
                DoTest("CachedCsvReader - Run 2", fileSize, CachedCsvReaderBenchmark.Run2, csv);
#endif
#if !NETCOREAPP1_0 && !NETCOREAPP2_0
                DoTest("TextFieldParser", fileSize, TextFieldParserBenchmark.Run, TestFile2);
#endif
                DoTest("Regex", fileSize, RegexBenchmark.Run, TestFile2);

                // seems to not be working on Windows 7 with Office 2007 (and I'm not bothering to try to make it run on my machine)
                //DoTest("OleDb", fileSize, OleDbBenchmark.Run, TestFile2);

                Console.WriteLine();

                Console.WriteLine("Test pass #{0} - Field #{1} (middle)\n", i, field);

                DoTest("CsvReader - No cache", fileSize, CsvReaderBenchmark.Run, TestFile2, field);
#if !NETCOREAPP1_0
                csv = DoTest("CachedCsvReader - Run 1", fileSize, CachedCsvReaderBenchmark.Run1, TestFile2, field);
                DoTest("CachedCsvReader - Run 2", fileSize, CachedCsvReaderBenchmark.Run2, csv, field);
#endif
#if !NETCOREAPP1_0 && !NETCOREAPP2_0
                DoTest("TextFieldParser", fileSize, TextFieldParserBenchmark.Run, TestFile2, field);
#endif
                DoTest("Regex", fileSize, RegexBenchmark.Run, TestFile2, field);

                // seems to not be working on Windows 7 with Office 2007 (and I'm not bothering to try to make it run on my machine)
#if !NETCOREAPP1_0 && !NETCOREAPP2_0
                //DoTest("OleDb", fileSize, OleDbBenchmark.Run, TestFile2, Field);
#endif

                Console.WriteLine();
                Console.WriteLine();
            }

            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private delegate object TestCallback(object[] args);

        private static object DoTest(string name, long fileSize, TestCallback testCallback, params object[] args)
        {
            QueryPerformanceFrequency(out long frequency);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            QueryPerformanceCounter(out long start);
            var value = testCallback(args);
            QueryPerformanceCounter(out long end);
            GetStats(start, end, frequency, fileSize, out long clocks, out double time, out double rate);

            Console.WriteLine("{0} : {1} ticks, {2:f4} sec., {3:f4} MB/sec.", name.PadRight(25), clocks, time, rate);

            return value;
        }

        private static void GetStats(long start, long end, long frequency, long fileSize, out long clocks, out double time, out double rate)
        {
            clocks = end - start;
            time = (double) clocks / frequency;
            rate = fileSize / time;
        }

#if !NETCOREAPP1_0
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject != null)
            {
                Console.WriteLine("Unhandled exception :\n\n'{0}'.", e.ExceptionObject);
            }
            else
            {
                Console.WriteLine("Unhandled exception occured.");
            }
            Console.ReadLine();
        }
#endif

        private static void PerformanceTestWithNullRemovalStreamReader()
        {
            var path = string.Empty;
            try
            {
                path = GenerateCsvFile();
                var fileSize = new FileInfo(path).Length / 1024 / 1024;
                DoTest("CsvReader -     without using NullRemovalStreamReader", fileSize, CsvReaderBenchmark.Run, path);
                Console.WriteLine();
                var result = DoTest("CsvReader - with NullRemovalStreamReader without mark", fileSize, CsvReaderBenchmark.Run, path, -1, false);
                Console.WriteLine(result + Environment.NewLine);
                result = DoTest("CsvReader - with NullRemovalStreamReader with    mark", fileSize, CsvReaderBenchmark.Run, path, -1, true);
                Console.WriteLine(result);
            }
            finally
            {
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        private static void PerformanceTestWithStringBuilder()
        {
            var path = string.Empty;
            try
            {
                path = GenerateCsvFile(false, 5);
                var fileSize = new FileInfo(path).Length / 1024 / 1024;
                DoTest("CsvReader - performance test with large cell", fileSize, CsvReaderBenchmark.Run, path);
            }
            finally
            {
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        private static string GenerateCsvFile(bool testNull = true, int x = 20)
        {
            // generate x million bytes; file size will be a little over x MB
            long count = x * 1024 * 1024;
            var path = Path.GetTempFileName();

            using (var sw = File.AppendText(path))
            {
                for (var i = 1; i <= 5; i++)
                {
                    sw.WriteLine("cell{0}1,cell{0}2,cell{0}3", i);
                }
                if (testNull)
                {
                    sw.Write("cell61,cell62,cell63 followed by " + count + " null bytes");
                    sw.Write(new char[count]);
                }
                else
                {
                    sw.Write("cell61,cell62,cell63 followed by " + count + " 'a' characters=>");
                    var data = new char [count];
                    for (var i = 0; i < count; i++)
                    {
                        data[i] = 'a';
                    }
                    sw.Write(data);
                }
            }
            return path;
        }
    }
}
