using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace SikuliSharp.Tests
{
	[TestFixture]
    public class EndToEndTests
    {
		private string _binPath;
		private ISikuli _sikuli;

		[SetUp]
		public void BeforeEach()
		{
			_binPath = Path.GetDirectoryName(new Uri(GetType().Assembly.CodeBase).LocalPath);
			_sikuli = new Sikuli();
		}

		[Test]
		public void CanClickOnAButtonAndDetectStateChange()
		{
			Process process = null;

			try
			{
				process = StartTestApplication();

				Assert.That(_sikuli.Exists(Patterns.FromFile(Path.Combine(_binPath, "Patterns", "RedLabel.png"))), Is.True);
			}
			finally
			{
				if (process != null)
					process.CloseMainWindow();
			}
		}

		private Process StartTestApplication()
		{
			var testAppPath = Path.Combine(_binPath, "SikuliSharp.TestApplication.exe");
			var process = Process.Start(new ProcessStartInfo(testAppPath));
			if (process == null) Assert.Fail("Cannot start process: process null");
			return process;
		}
    }
}
