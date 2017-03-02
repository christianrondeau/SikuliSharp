using System.IO;
using NUnit.Framework;

namespace SikuliSharp.Tests.Unit
{
	[TestFixture]
	public class AsyncTwoWayStreamsHandlerFactoryTests
	{
		[Test]
		public void CanCreateAnInstance()
		{
			var stdout = new StringReader("");
			var stderr = new StringReader("");
			var stdin = new StringWriter();

			Assert.That(new AsyncDuplexStreamsHandlerFactory().Create(stdout, stderr, stdin), Is.TypeOf<AsyncTwoWayStreamsHandler>());
		}
	}
}