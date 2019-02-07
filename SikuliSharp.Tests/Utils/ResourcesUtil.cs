using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
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
		public static string BlackOffsetLabelPatternPath { get; set; }
		public static string WhiteOffsetLabelPatternPath { get; set; }
		public static string OffsetButtonPatternPath { get; set; }

		static ResourcesUtil()
		{
			BinPath = Path.GetDirectoryName(new Uri(typeof(ResourcesUtil).Assembly.CodeBase).LocalPath);

			if(String.IsNullOrEmpty(BinPath))
				throw new Exception("Unable to find out the assembly codebase path");

			DataFolder = Path.Combine(BinPath, "Data.sikuli");
			RedLabelPatternPath = Path.Combine(DataFolder, "RedLabel.png");
			GreenLabelPatternPath = Path.Combine(DataFolder, "GreenLabel.png");
			TestButtonPatternPath = Path.Combine(DataFolder, "TestButton.png");
			BlackOffsetLabelPatternPath = Path.Combine(DataFolder, "BlackOffsetLabel.png");
			WhiteOffsetLabelPatternPath = Path.Combine(DataFolder, "WhiteOffsetLabel.png");
			OffsetButtonPatternPath = Path.Combine(DataFolder, "OffsetButton.png");
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
			[DllImport("user32.dll")]
			public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

			public struct Rect
			{
				public int Left { get; set; }
				public int Top { get; set; }
				public int Right { get; set; }
				public int Bottom { get; set; }
			}

			private readonly Process _process;

			public TestApplication(Process process)
			{
				_process = process;
			}

			public Point GetWindowLocation()
			{
				var rect = new Rect();
				GetWindowRect(_process.MainWindowHandle, ref rect);
				return new Point(rect.Left, rect.Top);
			}

			public void Dispose()
			{
				if (!_process.HasExited)
				{
					_process.CloseMainWindow();
					if (!_process.WaitForExit(1000))
					{
						_process.Kill();
					}
				}

				_process.Dispose();
			}
		}
	}
}