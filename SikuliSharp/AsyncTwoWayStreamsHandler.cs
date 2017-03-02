using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SikuliSharp
{
	public interface IAsyncDuplexStreamsHandlerFactory
	{
		IAsyncTwoWayStreamsHandler Create(TextReader stdout, TextReader stderr, TextWriter stdin);
	}

	public class AsyncDuplexStreamsHandlerFactory : IAsyncDuplexStreamsHandlerFactory
	{
		public IAsyncTwoWayStreamsHandler Create(TextReader stdout, TextReader stderr, TextWriter stdin)
		{
			return new AsyncTwoWayStreamsHandler(stdout, stderr, stdin);
		}
	}

	public interface IAsyncTwoWayStreamsHandler : IDisposable
	{
		string ReadUntil(double timeoutInSeconds, params string[] expectedStrings);
		IEnumerable<string> ReadUpToNow(double timeoutInSeconds);
		void WriteLine(string command);
		void WaitForExit();
	}

	public class AsyncTwoWayStreamsHandler : IAsyncTwoWayStreamsHandler
	{
		private readonly TextReader _stdout;
		private readonly TextReader _stderr;
		private readonly TextWriter _stdin;
		private readonly Task _readStderrTask;
		private readonly Task _readStdoutTask;
		private readonly BlockingCollection<string> _pendingOutputLines = new BlockingCollection<string>();

		public AsyncTwoWayStreamsHandler(TextReader stdout, TextReader stderr, TextWriter stdin)
		{
			_stdout = stdout;
			_stderr = stderr;
			_stdin = stdin;

			_readStdoutTask = new Task(ReadStdoutAsync);
			_readStdoutTask.Start();

			_readStderrTask = new Task(ReadStderrAsync);
			_readStderrTask.Start();
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

		public IEnumerable<string> ReadUpToNow(double timeoutInSeconds)
		{
			while (true)
			{
				string line;
				if (_pendingOutputLines.TryTake(out line, TimeSpan.FromSeconds(timeoutInSeconds)))
					yield return line;
				else
					yield break;
			}
		}

		public void WriteLine(string command)
		{
			_stdin.WriteLine(command);
		}

		public void WaitForExit()
		{
			_readStdoutTask.Wait();
			_readStderrTask.Wait();
		}

		public void Dispose()
		{
			if (_readStdoutTask != null) _readStdoutTask.Dispose();
			if (_readStderrTask != null) _readStderrTask.Dispose();
		}

		private void ReadStdoutAsync()
		{
			ReadStdAsync(_stdout);
		}

		private void ReadStderrAsync()
		{
			ReadStdAsync(_stderr, "[error] ");
		}

		private void ReadStdAsync(TextReader output, string prefix = null)
		{
			string line;
			while ((line = output.ReadLine()) != null)
			{
				if (prefix != null) line = prefix + line;
#if (DEBUG)
				Debug.WriteLine("SIKULI> " + line);
#endif
				_pendingOutputLines.Add(line);
			}
		}
	}
}