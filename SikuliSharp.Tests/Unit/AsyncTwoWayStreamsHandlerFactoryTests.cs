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
			var sr = new StringReader("");
			var sw = new StringWriter();

			Assert.That(new AsyncDuplexStreamsHandlerFactory().Create(sr, sw), Is.TypeOf<AsyncTwoWayStreamsHandler>());
		}
	}
}