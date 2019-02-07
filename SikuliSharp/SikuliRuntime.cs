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
		private ISikuliVersion _version;
		private Process _process;
		private IAsyncTwoWayStreamsHandler _asyncTwoWayStreamsHandler;
		private readonly ISikuliScriptProcessFactory _sikuliScriptProcessFactory;

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
			if (_process != null) throw new InvalidOperationException("This Sikuli session has already been started");

			_version = _sikuliScriptProcessFactory.GetSikuliVersion();
			_process = _sikuliScriptProcessFactory.Start(_version);

			_asyncTwoWayStreamsHandler = _asyncDuplexStreamsHandlerFactory.Create(_process.StandardOutput, _process.StandardError, _process.StandardInput);
			if(_version.ReadyMarker != null)
				_asyncTwoWayStreamsHandler.ReadUntil(SikuliReadyTimeoutSeconds, _version.ReadyMarker);
			foreach (var command in _version.InitialCommands ?? new string[0])
				_asyncTwoWayStreamsHandler.WriteLine(command);
		}

		public void Stop(bool ignoreErrors = false)
		{
			if (_process == null) return;

			_asyncTwoWayStreamsHandler.WriteLine(ExitCommand);

			if (!_process.HasExited)
			{
				if (!_process.WaitForExit(500))
					_process.Kill();
			}

			string errors = null;
			if (!ignoreErrors)
				errors = _process.StandardError.ReadToEnd();

			_asyncTwoWayStreamsHandler.WaitForExit();

			_asyncTwoWayStreamsHandler.Dispose();
			_asyncTwoWayStreamsHandler = null;
			_process.Dispose();
			_process = null;

			if (!ignoreErrors && !String.IsNullOrEmpty(errors))
				throw new SikuliException("Sikuli Errors: " + errors);
		}

		public string Run(string command, string resultPrefix, double timeoutInSeconds)
		{
			if(_process == null || _process.HasExited)
				throw new InvalidOperationException("The Sikuli process is not running");

			#if(DEBUG)
			Debug.WriteLine(command);
			#endif
			_asyncTwoWayStreamsHandler.WriteLine(command);
			_asyncTwoWayStreamsHandler.WriteLine("");
			_asyncTwoWayStreamsHandler.WriteLine("");

			var result = _asyncTwoWayStreamsHandler.ReadUntil(timeoutInSeconds, ErrorMarker, resultPrefix);

			if (result.IndexOf(ErrorMarker, StringComparison.Ordinal) > -1)
			{
				result = result + Environment.NewLine + string.Join(Environment.NewLine, _asyncTwoWayStreamsHandler.ReadUpToNow(0.1d));
				if (result.Contains("FindFailed"))
					throw new SikuliFindFailedException(result);
				throw new SikuliException(result);
			}

			return result;
		}

		public void Dispose()
		{
			Stop(true);
		}
	}
}