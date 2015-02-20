using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using SikuliSharp.Tests.Utils;

namespace SikuliSharp.Tests
{
	[TestFixture]
	public class EndToEndTests
	{
		[SetUp]
		public void BeforeEach()
		{
		}

		[TearDown]
		public void AfterEach()
		{
		}

		[Test]
		public void CanClickOnAButtonAndDetectStateChange()
		{

			using (ISikuliSession sikuli = new SikuliSession())
			{
				using (var process = StartTestApplication(ResourcesUtil.BinPath))
				{
					var redLabelPattern = Patterns.FromFile(ResourcesUtil.RedLabelPatternPath, 0.9f);
					var greenLabelPattern = Patterns.FromFile(ResourcesUtil.GreenLabelPatternPath, 0.9f);
					var testButtonPattern = Patterns.FromFile(ResourcesUtil.TestButtonPatternPath, 0.9f);

					Console.WriteLine("Assert Red Label Exists");
					Assert.That(sikuli.Exists(redLabelPattern), Is.True);

					Console.WriteLine("Assert Green Label does not Exist");
					Assert.That(sikuli.Exists(greenLabelPattern), Is.False);

					Console.WriteLine("Assert Click on Test Button");
					Assert.That(sikuli.Click(testButtonPattern), Is.True);

					Console.WriteLine("Assert Green Label Exists");
					Assert.That(sikuli.Exists(greenLabelPattern), Is.True);

					process.CloseMainWindow();
					process.WaitForExit();
				}
			}
		}

		private Process StartTestApplication(string binPath)
		{
			var testAppPath = Path.Combine(binPath, "SikuliSharp.TestApplication.exe");
			var process = Process.Start(new ProcessStartInfo(testAppPath));
			if (process == null) Assert.Fail("Cannot start process: process null");
			return process;
		}
	}
}
