using System;
using System.Diagnostics;

namespace SikuliSharp
{
	public interface ISikuliRuntime : IDisposable
	{
		void Start();
		void Stop(bool ignoreErrors = false);
		string Run(string command, string resultPrefix, double timeoutInSeconds);
	}

	public class SikuliRuntime : ISikuliRuntime
	{
		private readonly IAsyncDuplexStreamsHandlerFactory _asyncDuplexStreamsHandlerFactory;
		private bool _isRunning;
		private Process _process;
		private IAsyncTwoWayStreamsHandler _asyncTwoWayStreamsHandler;
		private readonly ISikuliScriptProcessFactory _sikuliScriptProcessFactory;

		private const string InteractiveConsoleReadyMarker = "... use ctrl-d to end the session";
		private const string ErrorMarker = "[error]";
		private const string ExitCommand = "exit()";
		private const int SikuliReadyTimeoutSeconds = 30;

		public SikuliRuntime(IAsyncDuplexStreamsHandlerFactory asyncDuplexStreamsHandlerFactory, ISikuliScriptProcessFactory sikuliScriptProcessFactory)
		{
			if (asyncDuplexStreamsHandlerFactory == null) throw new ArgumentNullException("asyncDuplexStreamsHandlerFactory");
			if (sikuliScriptProcessFactory == null) throw new ArgumentNullException("sikuliScriptProcessFactory");
			_asyncDuplexStreamsHandlerFactory = asyncDuplexStreamsHandlerFactory;
			_sikuliScriptProcessFactory = sikuliScriptProcessFactory;
		}

		public void Start()
		{
			if (_isRunning) return;

			_process = _sikuliScriptProcessFactory.Start("-i");

			_asyncTwoWayStreamsHandler = _asyncDuplexStreamsHandlerFactory.Create(_process.StandardOutput, _process.StandardInput);

			_asyncTwoWayStreamsHandler.ReadUntil(SikuliReadyTimeoutSeconds, InteractiveConsoleReadyMarker);
		}

		public void Stop(bool ignoreErrors = false)
		{
			if (!_isRunning) return;

			_asyncTwoWayStreamsHandler.WriteLine(ExitCommand);
			
			if(!_process.WaitForExit(500))
				_process.Kill();

			if (!ignoreErrors)
			{
				var errors = _process.StandardError.ReadToEnd();
				if (!String.IsNullOrEmpty(errors))
					throw new Exception("Sikuli Errors: " + errors);
			}

			_asyncTwoWayStreamsHandler.WaitForExit();

			_asyncTwoWayStreamsHandler.Dispose();
			_asyncTwoWayStreamsHandler = null;
			_process.Dispose();
			_process = null;

			_isRunning = false;
		}

		public string Run(string command, string resultPrefix, double timeoutInSeconds)
		{
			#if(DEBUG)
			Debug.WriteLine(command);
			#endif
			_asyncTwoWayStreamsHandler.WriteLine(command);
			_asyncTwoWayStreamsHandler.WriteLine("");
			_asyncTwoWayStreamsHandler.WriteLine("");

			var result = _asyncTwoWayStreamsHandler.ReadUntil(timeoutInSeconds, ErrorMarker, resultPrefix);

			if(result.IndexOf(ErrorMarker, StringComparison.Ordinal) > -1)
				throw new Exception("Sikuli Error: " + result);

			return result;
		}

		public void Dispose()
		{
			if (_process != null)
			{
				if (!_process.HasExited)
					Stop(true);

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