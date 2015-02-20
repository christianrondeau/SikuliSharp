using System.IO;
using NUnit.Framework;
using SikuliSharp.Tests.Utils;

namespace SikuliSharp.Tests.Unit
{
	[TestFixture]
	public class FilePatternTests
	{
		[Test]
		public void CanValidateThatFileExists()
		{
			new FilePattern(ResourcesUtil.RedLabelPatternPath, 0.5f).Validate();
			Assert.Pass("Did not throw");
		}

		[Test]
		[ExpectedException(typeof(FileNotFoundException), ExpectedMessage = @"Could not find image file specified in pattern: X:\DOES_NOT_EXIST.png")]
		public void CanValidateThatFileDoesNotExist()
		{
			new FilePattern(@"X:\DOES_NOT_EXIST.png", 1f).Validate();
		}

		[Test]
		public void CanConvertToSikuliScript()
		{
			Assert.That(new FilePattern(@"X:\MyImage.png", 0.3f).ToSikuliScript(), Is.EqualTo(@"Pattern(""X:\\MyImage.png"").similar(0.3)"));
		}
	}
}