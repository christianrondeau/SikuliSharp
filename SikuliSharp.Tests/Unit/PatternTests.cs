using NUnit.Framework;

namespace SikuliSharp.Tests.Unit
{
	[TestFixture]
	public class PatternTests
	{
		[Test]
		public void CanCreateFilePattern()
		{
			Assert.That(Pattern.FromFile(@"C:\Test.png"), Is.TypeOf<FilePattern>());
		}
	}
}
