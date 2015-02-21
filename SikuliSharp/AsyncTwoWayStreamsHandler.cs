using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SikuliSharp
{
	public interface IAsyncDuplexStreamsHandlerFactory
	{
		IAsyncTwoWayStreamsHandler Create(TextReader output, TextWriter input);
	}

	public class AsyncDuplexStreamsHandlerFactory : IAsyncDuplexStreamsHandlerFactory
	{
		public IAsyncTwoWayStreamsHandler Create(TextReader output, TextWriter input)
		{
			return new AsyncTwoWayStreamsHandler(output, input);
		}
	}

	public interface IAsyncTwoWayStreamsHandler : IDisposable
	{
		string ReadUntil(double timeoutInSeconds, params string[] expectedStrings);
		void WriteLine(string command);
		void WaitForExit();
	}

	public class AsyncTwoWayStreamsHandler : IAsyncTwoWayStreamsHandler
	{
		private readonly TextWriter _input;
		private readonly TextReader _output;
		private readonly Task _task;
		private readonly BlockingCollection<string> _pendingOutputLines = new BlockingCollection<string>();

		public AsyncTwoWayStreamsHandler(TextReader output, TextWriter input)
		{
			_input = input;
			_output = output;

			_task = new Task(ReadLinesAsync);
			_task.Start();
		}

		public string ReadUntil(double timeoutInSeconds, params string[] expectedStrings)
		{
			while (true)
			{
				string line;
				if (timeoutInSeconds > 0)
				{
					var timeout = TimeSpan.FromSeconds(timeoutInSeconds);
					if (!_pendingOutputLines.TryTake(out line, timeout))
						throw new TimeoutException(string.Format(@"No result in alloted time: {0:ss\.ffff}s", timeout));
				}
				else
				{
					line = _pendingOutputLines.Take();
				}

				if (expectedStrings.Any(s => line.IndexOf(s, StringComparison.Ordinal) > -1))
				{
					return line;
				}
			}
		}

		public void WriteLine(string command)
		{
			_input.WriteLine(command);
		}

		public void WaitForExit()
		{
			_task.Wait();
		}

		public void Dispose()
		{
			if (_task != null) _task.Dispose();
		}

		private void ReadLinesAsync()
		{
			string line;
			while ((line = _output.ReadLine()) != null)
			{
				#if(DEBUG)
				Debug.WriteLine("SIKULI> " + line ?? "");
				#endif
				_pendingOutputLines.Add(line);
			}
		}
	}
}