using System;
using NUnit.Framework;
using SikuliSharp.Tests.Utils;

namespace SikuliSharp.Tests
{
	[TestFixture]
	public class EndToEndTests
	{
		[Test]
		public void CanRunSikuliProject()
		{
			using (ResourcesUtil.StartTestApplication())
			{
				Assert.That(
					Sikuli.RunProject(ResourcesUtil.DataFolder),
					Is.StringMatching(@"\[log\] CLICK on L\(\d+,\d+\)@S\(0\)\[d\+,\d+ \d+x\d+\]\r\nSuccess\r\n")
					);
			}
		}

		[Test]
		public void CanRunSikuliCommands()
		{
			using (var session = Sikuli.CreateSession())
			{
				using (ResourcesUtil.StartTestApplication())
				{
					var redLabelPattern = Patterns.FromFile(ResourcesUtil.RedLabelPatternPath, 0.9f);
					var greenLabelPattern = Patterns.FromFile(ResourcesUtil.GreenLabelPatternPath, 0.9f);
					var testButtonPattern = Patterns.FromFile(ResourcesUtil.TestButtonPatternPath, 0.9f);

					Assert.That(session.Exists(redLabelPattern), Is.True, "Red label should exist");

					Assert.That(session.Exists(greenLabelPattern), Is.False, "Green label should not exist yet");

					Assert.That(session.Click(testButtonPattern), Is.True, "Click on test button");

					Assert.That(session.Exists(greenLabelPattern), Is.False, "Green label should still not exist (a 5s timer is shown)");

					Assert.Throws<TimeoutException>(() => session.Wait(greenLabelPattern, 1), "Wait for green label, but not long enough should now work");

					Assert.That(session.Wait(greenLabelPattern, 10), Is.True, "Wait for green label long enough should work");

					Assert.That(session.Exists(greenLabelPattern), Is.True, "Green label should now exist");
				}
			}
		}
	}
}
