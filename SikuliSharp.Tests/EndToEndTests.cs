using System;
using System.Threading;
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
					Is.StringContaining("SikuliSharp.Tests.Success")
					);
			}
		}

		[Test]
		public void CanRunSikuliCommands()
		{
			using (var session = Sikuli.CreateSession())
			{
				using (var app = ResourcesUtil.StartTestApplication())
				{
					var redLabelPattern = Patterns.FromFile(ResourcesUtil.RedLabelPatternPath, 0.9f);
					var greenLabelPattern = Patterns.FromFile(ResourcesUtil.GreenLabelPatternPath, 0.9f);
					var testButtonPattern = Patterns.FromFile(ResourcesUtil.TestButtonPatternPath, 0.9f);
					var blackOffsetLabelPattern = Patterns.FromFile(ResourcesUtil.BlackOffsetLabelPatternPath, 0.9f);
					var whiteOffsetLabelPattern = Patterns.FromFile(ResourcesUtil.WhiteOffsetLabelPatternPath, 0.9f);

					Assert.That(session.Wait(redLabelPattern), Is.True, "Wait for application startup");

					Assert.That(session.Exists(blackOffsetLabelPattern), Is.True, "Black offset label should exist");

					Assert.That(session.Click(testButtonPattern, new Point(125, 0)), Is.True, "Click on the offset button by offsetting from the test button");

					Assert.That(session.Exists(whiteOffsetLabelPattern), Is.True, "White offset label should exist");

					var appLocation = app.GetWindowLocation();
					Assert.That(session.Click(new Location(appLocation.X + 350, appLocation.Y + 100)), Is.True, "Click on offset button by location");

					Assert.That(session.Exists(blackOffsetLabelPattern), Is.True, "Black offset label should exist");

					session.Highlight(session.Find(blackOffsetLabelPattern), "blue");

					Assert.That(session.Wait(redLabelPattern), Is.True, "Red label should exist");

					Assert.That(session.Exists(greenLabelPattern), Is.False, "Green label should not exist yet");

					Assert.That(session.Hover(testButtonPattern), Is.True, "Hover over Test Button");

					Assert.That(session.Hover(new Location(appLocation.X + 350, appLocation.Y + 100)), Is.True, "Hover outside Test Button");

					Assert.That(session.Click(testButtonPattern), Is.True, "Click on test button");

					Assert.That(session.Exists(greenLabelPattern), Is.False, "Green label should still not exist (a 5s timer is shown)");

					Assert.Throws<SikuliFindFailedException>(() => session.Wait(greenLabelPattern, 1), "Wait for green label, but not long enough should now work");

					Assert.That(session.Wait(greenLabelPattern, 10), Is.True, "Wait for green label long enough should work");

					Assert.That(session.Exists(greenLabelPattern, 10), Is.True, "Green label should now exist");

					Assert.That(session.Type("x"), Is.True, "Type 'x' to exit");

					Assert.That(session.WaitVanish(greenLabelPattern, 10), Is.True, "Wait for green label to vanish after app has exited");
				}
			}
		}
	}
}
