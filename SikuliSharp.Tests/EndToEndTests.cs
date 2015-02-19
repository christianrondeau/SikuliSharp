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
		private Process _process;

		[SetUp]
		public void BeforeEach()
		{
			_binPath = Path.GetDirectoryName(new Uri(GetType().Assembly.CodeBase).LocalPath);
			_sikuli = new Sikuli();
			_process = StartTestApplication();
		}

		[TearDown]
		public void AfterEach()
		{
			if (_process != null)
				_process.CloseMainWindow();
		}

		[Test]
		public void CanClickOnAButtonAndDetectStateChange()
		{
			var redLabelPattern = Patterns.FromFile(Path.Combine(_binPath, "Patterns", "RedLabel.png"), 0.9f);
			var greenLabelPattern = Patterns.FromFile(Path.Combine(_binPath, "Patterns", "GreenLabel.png"), 0.9f);
			var testButtonPattern = Patterns.FromFile(Path.Combine(_binPath, "Patterns", "TestButton.png"), 0.9f);

			Console.WriteLine("Assert Red Label Exists");
			Assert.That(_sikuli.Exists(redLabelPattern), Is.True);

			Console.WriteLine("Assert Green Label does not Exist");
			Assert.That(_sikuli.Exists(greenLabelPattern), Is.False);

			Console.WriteLine("Assert Click on Test Button");
			Assert.That(_sikuli.Click(testButtonPattern), Is.True);

			Console.WriteLine("Assert Green Label Exists");
			Assert.That(_sikuli.Exists(greenLabelPattern), Is.True);
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
