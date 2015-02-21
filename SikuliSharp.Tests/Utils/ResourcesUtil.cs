using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace SikuliSharp.Tests.Utils
{
	public class ResourcesUtil
	{
		public static string BinPath { get; private set; }
		public static string DataFolder { get; private set; }
		public static string TestButtonPatternPath { get; private set; }
		public static string GreenLabelPatternPath { get; private set; }
		public static string RedLabelPatternPath { get; private set; }

		static ResourcesUtil()
		{
			BinPath = Path.GetDirectoryName(new Uri(typeof(ResourcesUtil).Assembly.CodeBase).LocalPath);

			if(String.IsNullOrEmpty(BinPath))
				throw new Exception("Unable to find out the assembly codebase path");

			DataFolder = Path.Combine(BinPath, "Data.sikuli");
			RedLabelPatternPath = Path.Combine(DataFolder, "RedLabel.png");
			GreenLabelPatternPath = Path.Combine(DataFolder, "GreenLabel.png");
			TestButtonPatternPath = Path.Combine(DataFolder, "TestButton.png");
		}

		public static TestApplication StartTestApplication()
		{
			var testAppPath = Path.Combine(BinPath, "SikuliSharp.TestApplication.exe");
			var process = Process.Start(new ProcessStartInfo(testAppPath));
			if (process == null) Assert.Fail("Cannot start process: process null");
			return new TestApplication(process);
		}

		public class TestApplication : IDisposable
		{
			private readonly Process _process;

			public TestApplication(Process process)
			{
				_process = process;
			}

			public void Dispose()
			{
				if (!_process.HasExited)
				{
					_process.CloseMainWindow();
					_process.WaitForExit();
				}

				_process.Dispose();
			}
		}
	}
}