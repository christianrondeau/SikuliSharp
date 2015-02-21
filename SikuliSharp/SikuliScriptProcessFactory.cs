using System;
using System.Diagnostics;
using System.IO;

namespace SikuliSharp
{
	public interface ISikuliScriptProcessFactory
	{
		Process Start(string args);
	}

	public class SikuliScriptProcessFactoryFactory : ISikuliScriptProcessFactory
	{
		public Process Start(string args)
		{
			var javaHome = GetPathEnvironmentVariable("JAVA_HOME");
			var javaPath = Path.Combine(javaHome, "bin", "java.exe");
			if (!File.Exists(javaPath))
				throw new Exception(string.Format("Java executable referenced from JAVA_HOME does not exist: {0}", javaPath));

			var sikuliHome = GetPathEnvironmentVariable("SIKULI_HOME");
			var sikuliScriptJarPath = Path.Combine(sikuliHome, "sikuli-script.jar");
			if (!File.Exists(sikuliScriptJarPath))
				throw new Exception(string.Format("Sikuli JAR references from SIKULI_HOME does not exist: {0}", sikuliScriptJarPath));

			var process = new Process
			{
				StartInfo =
				{
					FileName = javaPath,
					Arguments = string.Format("-jar \"{0}\" {1}", sikuliScriptJarPath, args),
					CreateNoWindow = true,
					WindowStyle = ProcessWindowStyle.Hidden,
					UseShellExecute = false,
					RedirectStandardInput = true,
					RedirectStandardError = true,
					RedirectStandardOutput = true
				}
			};

			process.Start();

			return process;
		}

		private static string GetPathEnvironmentVariable(string name)
		{
			var value = Environment.GetEnvironmentVariable(name);
			if (String.IsNullOrEmpty(value)) throw new Exception(string.Format("Environment variables {0} not set", name));
			return value;
		}
	}
}