using System;

namespace SikuliSharp
{
	public interface ISikuliSession : IDisposable
	{
		bool Exists(IPattern pattern, float timeoutInSeconds = 0);
		bool Click(IPattern pattern, float timeoutInSeconds = 0);
		bool Wait(IPattern pattern, float timeoutInSeconds = 0);
	}

	public class SikuliSession : ISikuliSession
	{
		private readonly ISikuliRuntime _runtime;

		public SikuliSession(ISikuliRuntime sikuliRuntime)
		{
			_runtime = sikuliRuntime;
			_runtime.Start();
		}

		public bool Exists(IPattern pattern, float timeoutInSeconds = 0f)
		{
			return RunCommand("exists", pattern, timeoutInSeconds);
		}

		public bool Click(IPattern pattern, float timeoutInSeconds = 0f)
		{
			return RunCommand("click", pattern, timeoutInSeconds);
		}

		public bool Wait(IPattern pattern, float timeoutInSeconds = 0f)
		{
			return RunCommand("wait", pattern, timeoutInSeconds);
		}

		private bool RunCommand(string command, IPattern pattern, float timeoutInSeconds)
		{
			pattern.Validate();

			var script = string.Format(
				"print \"SIKULI#: YES\" if {0}({1}{2}) else \"SIKULI#: NO\"",
				command,
				pattern.ToSikuliScript(),
				timeoutInSeconds > 0f ? ", " + timeoutInSeconds.ToString("0.0000") : ""
				);

			var result = _runtime.Run(script, "SIKULI#: ", timeoutInSeconds);
			return result.Contains("SIKULI#: YES");
		}

		public void Dispose()
		{
			_runtime.Stop();
		}
	}
}