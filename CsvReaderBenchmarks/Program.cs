using System;
using System.Diagnostics;

using LumenWorks.Framework.IO.Csv;

namespace CsvReaderDemo
{
	class Program
	{
		// StopWatch seems to not be accurate enough to be used here (divisions by zero occur when calculating MB/s).

		[System.Runtime.InteropServices.DllImport("Kernel32.dll")]
		private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

		[System.Runtime.InteropServices.DllImport("Kernel32.dll")]
		private static extern bool QueryPerformanceFrequency(out long lpFrequency);

		[STAThread()]
		static void Main(string[] args)
		{
			//const string TestFile1 = @"..\..\test1.csv";
			const string TestFile2 = @"..\..\test2.csv";
			const string TestFile3 = @"..\..\test3.csv";

			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

			if (args.Length > 0)
			{
				if (args.Length == 1)
				{
					string s = args[0].ToUpper();

					switch (s)
					{
						case "CSVREADER":
							CsvReaderBenchmark.Run(TestFile3);
							return;
						case "OLEDB":
							OleDbBenchmark.Run(TestFile3);
							return;
						case "REGEX":
							RegexBenchmark.Run(TestFile3);
							return;
					}
				}

				Console.WriteLine("Possible values : CsvReader, OleDb, Regex");
				return;
			}

			const int Field = 72;
			long fileSize = new System.IO.FileInfo(TestFile2).Length / 1024 / 1024;

			for (int i = 1; i < 4; i++)
			{
				object csv;

				Console.WriteLine("Test pass #{0} - All fields\n", i);

				DoTest("CsvReader - No cache", fileSize, CsvReaderBenchmark.Run, TestFile2);
				csv = DoTest("CachedCsvReader - Run 1", fileSize, CachedCsvReaderBenchmark.Run1, TestFile2);
				DoTest("CachedCsvReader - Run 2", fileSize, CachedCsvReaderBenchmark.Run2, csv);
				DoTest("TextFieldParser", fileSize, TextFieldParserBenchmark.Run, TestFile2);
				DoTest("Regex", fileSize, RegexBenchmark.Run, TestFile2);

				// seems to not be working on Windows 7 with Office 2007 (and I'm not bothering to try to make it run on my machine)
				//DoTest("OleDb", fileSize, OleDbBenchmark.Run, TestFile2);

				Console.WriteLine();

				Console.WriteLine("Test pass #{0} - Field #{1} (middle)\n", i, Field);

				DoTest("CsvReader - No cache", fileSize, CsvReaderBenchmark.Run, TestFile2, Field);
				csv = DoTest("CachedCsvReader - Run 1", fileSize, CachedCsvReaderBenchmark.Run1, TestFile2, Field);
				DoTest("CachedCsvReader - Run 2", fileSize, CachedCsvReaderBenchmark.Run2, csv, Field);
				DoTest("TextFieldParser", fileSize, TextFieldParserBenchmark.Run, TestFile2, Field);
				DoTest("Regex", fileSize, RegexBenchmark.Run, TestFile2, Field);

				// seems to not be working on Windows 7 with Office 2007 (and I'm not bothering to try to make it run on my machine)
				//DoTest("OleDb", fileSize, OleDbBenchmark.Run, TestFile2, Field);

				Console.WriteLine();
				Console.WriteLine();
			}

			Console.WriteLine("Done");
			Console.ReadLine();
		}

		delegate object TestCallback(object[] args);

		static object DoTest(string name, long fileSize, TestCallback testCallback, params object[] args)
		{
			long start;
			long end;
			long frequency;
			long clocks;
			double time;
			double rate;

			QueryPerformanceFrequency(out frequency);

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			QueryPerformanceCounter(out start);
			object value = testCallback(args);
			QueryPerformanceCounter(out end);
			GetStats(start, end, frequency, fileSize, out clocks, out time, out rate);

			Console.WriteLine("{0} : {1} ticks, {2:f4} sec., {3:f4} MB/sec.", name.PadRight(25), clocks, time, rate);

			return value;
		}

		static void GetStats(long start, long end, long frequency, long fileSize, out long clocks, out double time, out double rate)
		{
			clocks = end - start;
			time = (double) clocks / frequency;
			rate = fileSize / time;
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (e.ExceptionObject != null)
				Console.WriteLine("Unhandled exception :\n\n'{0}'.", e.ExceptionObject.ToString());
			else
				Console.WriteLine("Unhandled exception occured.");

			Console.ReadLine();
		}
	}
}