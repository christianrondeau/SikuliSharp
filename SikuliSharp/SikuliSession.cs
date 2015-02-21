using System;
using System.Text.RegularExpressions;

namespace SikuliSharp
{
	public interface ISikuliSession : IDisposable
	{
		bool Exists(IPattern pattern, float timeoutInSeconds = 0);
		bool Click(IPattern pattern, float timeoutInSeconds = 0);
		bool Wait(IPattern pattern, float timeoutInSeconds = 0);
		bool WaitVanish(IPattern pattern, float timeoutInSeconds = 0);
		bool Type(string text);
	}

	public class SikuliSession : ISikuliSession
	{
		private static readonly Regex InvalidTextRegex = new Regex(@"[\r\n\t\x00-\x1F]", RegexOptions.Compiled);
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

		public bool WaitVanish(IPattern pattern, float timeoutInSeconds = 0f)
		{
			return RunCommand("waitVanish", pattern, timeoutInSeconds);
		}

		public bool Type(string text)
		{
			if(InvalidTextRegex.IsMatch(text))
				throw new ArgumentException("Text cannot contain control characters. Escape them before, e.g. \\n should be \\\\n", "text");

			var script = string.Format(
				"print \"SIKULI#: YES\" if type(\"{0}\") == 1 else \"SIKULI#: NO\"",
				text
				);

			var result = _runtime.Run(script, "SIKULI#: ", 0d);
			return result.Contains("SIKULI#: YES");
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