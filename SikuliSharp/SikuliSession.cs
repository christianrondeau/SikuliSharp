using System;

namespace SikuliSharp
{
	public interface ISikuliSession : IDisposable
	{
		bool Exists(IPattern pattern);
		bool Click(IPattern pattern);
	}

	public class SikuliSession : ISikuliSession
	{
		private readonly ISikuliRuntime _runtime;

		public SikuliSession(ISikuliRuntime sikuliRuntime)
		{
			_runtime = sikuliRuntime;
			_runtime.Start();
		}

		public bool Exists(IPattern pattern)
		{
			return RunCommand("exists", pattern);
		}

		public bool Click(IPattern pattern)
		{
			return RunCommand("click", pattern);
		}

		private bool RunCommand(string command, IPattern pattern)
		{
			pattern.Validate();

			var script = string.Format("print \"SIKULI#: YES\" if {0}({1}) else \"SIKULI#: NO\"", command, pattern.ToSikuliScript());
			var result = _runtime.Run(script, "SIKULI#: ");
			return result.Contains("SIKULI#: YES");
		}

		public void Dispose()
		{
			_runtime.Stop();
		}
	}
}