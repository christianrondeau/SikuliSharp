using System;
using System.Diagnostics;
using System.IO;

namespace SikuliSharp
{
	public class Sikuli : ISikuli
	{
		private readonly ISikuliExec _exec;

		public Sikuli() : this(new SikuliExec())
		{
		}

		public Sikuli(ISikuliExec exec)
		{
			if (exec == null) throw new ArgumentNullException("exec");
			_exec = exec;
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
			string consoleOutput;
			using (var project = new TemporarySikuliProject())
			{
				var script = string.Format("if {0}({1}): print \"YES\"\nelse: print \"NO\"", command, pattern.ToSikuliScript());

				#if(DEBUG)
				Debug.WriteLine("Script Output:");
				Debug.WriteLine(script);
				#endif

				File.WriteAllText(project.ScriptPath, script);

				consoleOutput = _exec.ExecuteProject(project.ProjectPath);
			}

			return consoleOutput.Contains("YES");
		}
	}
}