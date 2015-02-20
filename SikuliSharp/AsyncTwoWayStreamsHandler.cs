using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SikuliSharp
{
	public interface IAsyncTwoWayStreamsHandlerFactory
	{
		IAsyncTwoWayStreamsHandler Create(StreamReader output, StreamWriter input);
	}

	public class AsyncTwoWayStreamsHandlerFactory : IAsyncTwoWayStreamsHandlerFactory
	{
		public IAsyncTwoWayStreamsHandler Create(StreamReader output, StreamWriter input)
		{
			return new AsyncTwoWayStreamsHandler(output, input);
		}
	}

	public interface IAsyncTwoWayStreamsHandler : IDisposable
	{
		string ReadUntil(params string[] expectedStrings);
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

			_task = new Task(AsyncOutputStreamReader);
			_task.Start();
		}

		public string ReadUntil(params string[] expectedStrings)
		{
			while (true)
			{
				var line = _pendingOutputLines.Take();
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
			if (_input != null) _input.Dispose();
			if (_output != null) _output.Dispose();
		}

		private void AsyncOutputStreamReader()
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