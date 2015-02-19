using System;
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
			string consoleOutput;
			using (var project = new TemporarySikuliProject())
			{
				File.WriteAllText(project.ScriptPath, string.Format("if exists({0}): print \"YES\"", pattern.ToSikuliScript()));

				consoleOutput = _exec.ExecuteProject(project.ProjectPath);
			}

			return consoleOutput.Contains("YES");
		}
	}
}