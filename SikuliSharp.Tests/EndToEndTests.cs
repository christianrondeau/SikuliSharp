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

					Console.WriteLine("Assert Red Label Exists");
					Assert.That(session.Exists(redLabelPattern), Is.True);

					Console.WriteLine("Assert Green Label does not Exist");
					Assert.That(session.Exists(greenLabelPattern), Is.False);

					Console.WriteLine("Assert Click on Test Button");
					Assert.That(session.Click(testButtonPattern), Is.True);

					Console.WriteLine("Assert Green Label Exists");
					Assert.That(session.Exists(greenLabelPattern), Is.True);
				}
			}
		}
	}
}
