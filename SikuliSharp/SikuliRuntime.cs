using System;
using System.Diagnostics;
using System.IO;

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

		public SikuliRuntime(IAsyncTwoWayStreamsHandlerFactory asyncTwoWayStreamsHandlerFactory)
		{
			if (asyncTwoWayStreamsHandlerFactory == null) throw new ArgumentNullException("asyncTwoWayStreamsHandlerFactory");
			_asyncTwoWayStreamsHandlerFactory = asyncTwoWayStreamsHandlerFactory;
		}

		public void Start()
		{
			if (_isRunning) return;

			var javaHome = GetPathEnvironmentVariable("JAVA_HOME");
			var javaPath = Path.Combine(javaHome, "bin", "java.exe");
			if (!File.Exists(javaPath))
				throw new Exception(string.Format("Java executable referenced from JAVA_HOME does not exist: {0}", javaPath));

			var sikuliHome = GetPathEnvironmentVariable("SIKULI_HOME");
			var sikuliScriptJarPath = Path.Combine(sikuliHome, "sikuli-script.jar");
			if (!File.Exists(sikuliScriptJarPath))
				throw new Exception(string.Format("Sikuli JAR references from SIKULI_HOME does not exist: {0}", sikuliScriptJarPath));

			_process = new Process
			{
				StartInfo =
				{
					FileName = javaPath,
					Arguments = string.Format("-jar \"{0}\" -i", sikuliScriptJarPath),
					CreateNoWindow = true,
					WindowStyle = ProcessWindowStyle.Hidden,
					UseShellExecute = false,
					RedirectStandardInput = true,
					RedirectStandardError = true,
					RedirectStandardOutput = true
				}
			};

			_process.Start();

			_asyncTwoWayStreamsHandler = _asyncTwoWayStreamsHandlerFactory.Create(_process.StandardOutput, _process.StandardInput);

			_asyncTwoWayStreamsHandler.ReadUntil("... use ctrl-d to end the session");
		}

		public void Stop()
		{
			if (!_isRunning) return;

			_asyncTwoWayStreamsHandler.WriteLine("exit()");
			
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

			return _asyncTwoWayStreamsHandler.ReadUntil(resultPrefix);
		}

		private static string GetPathEnvironmentVariable(string name)
		{
			var value = Environment.GetEnvironmentVariable(name);
			if (String.IsNullOrEmpty(value)) throw new Exception(string.Format("Environment variables {0} not set", name));
			return value;
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