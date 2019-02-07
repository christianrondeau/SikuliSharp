using System;

namespace SikuliSharp
{
	public interface ISikuliVersion
	{
		string Arguments { get; }
		string ReadyMarker { get; }
		string[] InitialCommands { get; }
		ISikuliVersion WithProject(string projectPath, string args);
	}

	public class Sikuli101Version : ISikuliVersion
	{
		private readonly string _jar;
		public string ReadyMarker =>  "... use ctrl-d to end the session";
		public string[] InitialCommands => null;
		public string Arguments { get; private set; }

		public Sikuli101Version(string sikuliScriptJar)
		{
			_jar = sikuliScriptJar;
			Arguments = string.Format("-jar \"{0}\" -i", _jar);
		}

		public ISikuliVersion WithProject(string projectPath, string args)
		{
			Arguments = string.Format("-jar \"{0}\" -r {1} {2}", _jar, projectPath, args);
			return this;
		}
	}

	public class Sikuli110Version : ISikuliVersion
	{
		private readonly string _jar;
		public string ReadyMarker =>  "... use ctrl-d to end the session";
		public string[] InitialCommands => null;
		public string Arguments { get; private set; }

		public Sikuli110Version(string sikuliXJar)
		{
			_jar = sikuliXJar;
			Arguments = string.Format("-jar \"{0}\" -i", _jar);
		}

		public ISikuliVersion WithProject(string projectPath, string args)
		{
			Arguments = string.Format("-jar \"{0}\" -r {1} {2}", _jar, projectPath, args);
			return this;
		}
	}

	public class Sikuli114Version : ISikuliVersion
	{
		public string ReadyMarker => null; // "Use exit() or Ctrl-D (i.e. EOF) to exit";
		public string[] InitialCommands => new[]
		{
			"import org.sikuli.script.SikulixForJython",
			"from sikuli.Sikuli import *"
		};
		public string Arguments { get; }


		public Sikuli114Version(string apiJar, string jythonJar)
		{
			Arguments = string.Format(
				"-cp \"{0};{1}\" org.python.util.jython",
				apiJar,
				jythonJar
			);
		}

		public ISikuliVersion WithProject(string projectPath, string args)
		{
			throw new NotImplementedException();
		}
	}
}