using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using SikuliSharp.Tests.Utils;

namespace SikuliSharp.Tests.Unit
{
	[TestFixture]
	public class AsyncTwoWayStreamsHandlerTests
	{
		[Test]
		public void CanWrite()
		{
			var sr = new StreamReader(new MemoryStream());
			var sb = new StringBuilder();
			var sw = new StringWriter(sb);

			var handler = new AsyncTwoWayStreamsHandler(sr, sw);

			handler.WriteLine("Should end up in the StreamWriter");
			handler.WriteLine("With a line break after each");

			Assert.That(sb.ToString(), Is.EqualTo("Should end up in the StreamWriter" + Environment.NewLine + "With a line break after each" + Environment.NewLine));
		}

		[Test]
		public void CanReadUntilStringIsFound()
		{
			var sr = new StringReader(String.Join(
				Environment.NewLine,
				"This line should be ignored because it comes before",
				"This line should be taken because it includes MARKER in it",
				"This line should be ignored because it comes after"
				));
			var sw = new StringWriter();

			var handler = new AsyncTwoWayStreamsHandler(sr, sw);

			Assert.That(handler.ReadUntil(0, "MARKER"), Is.EqualTo("This line should be taken because it includes MARKER in it"));
		}

		[Test]
		[ExpectedException(typeof(TimeoutException), ExpectedMessage = "No result in alloted time: 00.1000s")]
		public void ReadUntilTimeoutThrows()
		{
			var stream = new BlockingStream();
			var sr = new StreamReader(stream);
			var sw = new StringWriter();

			var handler = new AsyncTwoWayStreamsHandler(sr, sw);

			Assert.That(handler.ReadUntil(0.1, "MARKER"), Is.EqualTo("This line should be taken because it includes MARKER in it"));
		}

		[Test]
		public void WaitForExitExitsImmediatelyWhenNothingToWaitFor()
		{
			var sr = new StringReader("");
			var sw = new StringWriter();

			var handler = new AsyncTwoWayStreamsHandler(sr, sw);

			handler.WaitForExit();
			Assert.Pass("Successfully skipped waiting since there was nothing to do");
		}

		[Test]
		public void WaitForExitBlocksWhenStillReading()
		{
			var blockingStream = new BlockingStream();
			var sr = new StreamReader(blockingStream);
			var sw = new StringWriter();

			var handler = new AsyncTwoWayStreamsHandler(sr, sw);

			var stopWatch = new Stopwatch();
			stopWatch.Start();

			new Task(() =>
			{
				Thread.Sleep(100);
				blockingStream.Unblock();
			}).Start();
			handler.WaitForExit();

			stopWatch.Stop();

			if (stopWatch.ElapsedMilliseconds < 100)
				Assert.Fail("WaitForExit returned before the timer finished");
			else
				Assert.Pass("Successfully waited for the stream to end");
		}
	}
}