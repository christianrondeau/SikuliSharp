using System;
using System.Diagnostics;

namespace SikuliSharp
{
	public interface ISikuliRuntime : IDisposable
	{
		void Start();
		void Stop();
		string Run(string command, string resultPrefix);
	}

	public class SikuliRuntime : ISikuliRuntime
	{
		private readonly IAsyncTwoWayStreamsHandlerFactory _asyncTwoWayStreamsHandlerFactory;
		private bool _isRunning;
		private Process _process;
		private IAsyncTwoWayStreamsHandler _asyncTwoWayStreamsHandler;
		private readonly ISikuliScriptProcessFactory _sikuliScriptProcessFactory;

		private const string InteractiveConsoleReadyMarker = "... use ctrl-d to end the session";
		private const string ErrorMarker = "[error]";
		private const string ExitCommand = "exit()";

		public SikuliRuntime(IAsyncTwoWayStreamsHandlerFactory asyncTwoWayStreamsHandlerFactory, ISikuliScriptProcessFactory sikuliScriptProcessFactory)
		{
			if (asyncTwoWayStreamsHandlerFactory == null) throw new ArgumentNullException("asyncTwoWayStreamsHandlerFactory");
			if (sikuliScriptProcessFactory == null) throw new ArgumentNullException("sikuliScriptProcessFactory");
			_asyncTwoWayStreamsHandlerFactory = asyncTwoWayStreamsHandlerFactory;
			_sikuliScriptProcessFactory = sikuliScriptProcessFactory;
		}

		public void Start()
		{
			if (_isRunning) return;

			_process = _sikuliScriptProcessFactory.Start("-i");

			_asyncTwoWayStreamsHandler = _asyncTwoWayStreamsHandlerFactory.Create(_process.StandardOutput, _process.StandardInput);

			_asyncTwoWayStreamsHandler.ReadUntil(InteractiveConsoleReadyMarker);
		}

		public void Stop()
		{
			if (!_isRunning) return;

			_asyncTwoWayStreamsHandler.WriteLine(ExitCommand);
			
			if(!_process.WaitForExit(500))
				_process.Kill();
			_asyncTwoWayStreamsHandler.WaitForExit();

			_asyncTwoWayStreamsHandler.Dispose();
			_asyncTwoWayStreamsHandler = null;
			_process.Dispose();
			_process = null;

			_isRunning = false;
		}

		public string Run(string command, string resultPrefix)
		{
			#if(DEBUG)
			Debug.WriteLine(command);
			#endif
			_asyncTwoWayStreamsHandler.WriteLine(command);
			_asyncTwoWayStreamsHandler.WriteLine("");
			_asyncTwoWayStreamsHandler.WriteLine("");

			var result = _asyncTwoWayStreamsHandler.ReadUntil(ErrorMarker, resultPrefix);

			if(result.IndexOf(ErrorMarker, StringComparison.Ordinal) > -1)
				throw new Exception("Sikuli Error: " + result);

			return result;
		}

		public void Dispose()
		{
			if (_process != null)
			{
				if (!_process.HasExited)
					Stop();
				_process.Dispose();
				_process = null;
			}

			if (_asyncTwoWayStreamsHandler != null)
			{
				_asyncTwoWayStreamsHandler.Dispose();
				_asyncTwoWayStreamsHandler = null;
			}
		}
	}
}