using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace SikuliSharp
{
	public interface ISikuliScriptProcessFactory
	{
		Process Start(string args);
	}

	public class SikuliScriptProcessFactory : ISikuliScriptProcessFactory
	{
		public Process Start(string args)
		{
			var javaPath = GuessJavaPath();
			var sikuliHome = MakeEmptyNull(Environment.GetEnvironmentVariable("SIKULI_HOME"));
			if (sikuliHome == null) throw new Exception("Environment variable SIKULI_HOME not set. Please verify that Sikuli is installed (sikuli-script.jar must be present) and create a SIKULI_HOME environment variable. You may need to restart your command prompt or IDE.");
			var sikuliScriptJarPath = DetectSikuliPath(sikuliHome);
			var javaArguments = string.Format("-jar \"{0}\" {1}", sikuliScriptJarPath, args);

#if (DEBUG)
			Debug.WriteLine("Launching Sikuli: \"" + javaPath + "\" " + javaArguments);
#endif

			var process = new Process
			{
				StartInfo =
				{
					FileName = javaPath,
					Arguments = javaArguments,
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

		private static string DetectSikuliPath(string sikuliHome)
		{
			var sikuliScript101JarPath = Path.Combine(sikuliHome, "sikuli-script.jar");
			if (File.Exists(sikuliScript101JarPath))
				return sikuliScript101JarPath;
			
			var sikuliScript110JarPath = Path.Combine(sikuliHome, "sikulix.jar");
			if (File.Exists(sikuliScript110JarPath))
				return sikuliScript110JarPath;

			throw new FileNotFoundException(
				string.Format("Neither sikuli-script.jar nor sikulix.jar were found in the path referenced in SIKULI_HOME environment variable \"{0}\"", sikuliHome));
		}

		public static string GuessJavaPath()
		{
			var javaHome = MakeEmptyNull(Environment.GetEnvironmentVariable("JAVA_HOME"))
						   ?? MakeEmptyNull(GetJavaPathFromRegistry(RegistryView.Registry64))
						   ?? MakeEmptyNull(GetJavaPathFromRegistry(RegistryView.Registry32));

			if (String.IsNullOrEmpty(javaHome))
				throw new Exception("Java path not found. Is it installed? If yes, set the JAVA_HOME environment vairable.");

			var javaPath = Path.Combine(javaHome, "bin", "java.exe");

			if (!File.Exists(javaPath))
				throw new Exception(string.Format("Java executable not found in expected folder: {0}. If you have multiple Java installations, you may want to set the JAVA_HOME environment variable.", javaPath));

			return javaPath;
		}

		public static string GetJavaPathFromRegistry(RegistryView view)
		{
			const string jreKey = "SOFTWARE\\JavaSoft\\Java Runtime Environment";
			using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view).OpenSubKey(jreKey))
			{
				if (baseKey == null)
					return null;

				var currentVersion = baseKey.GetValue("CurrentVersion").ToString();
				using (var homeKey = baseKey.OpenSubKey(currentVersion))
				{
					if (homeKey != null) return homeKey.GetValue("JavaHome").ToString();
				}
			}
			return null;
		}

		private static string MakeEmptyNull(string value)
		{
			return value == "" ? null : value;
		}
	}
}