using NUnit.Framework;

namespace SikuliSharp.Tests.Unit
{
	[TestFixture]
	public class PatternsTests
	{
		[Test]
		public void CanCreateFilePattern()
		{
			Assert.That(Patterns.FromFile(@"C:\Test.png"), Is.TypeOf<FilePattern>());
		}
	}
}