using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SikuliSharp
{
	public interface ISikuliExec
	{
		string ExecuteProject(string projectPath);
	}

	public class SikuliExec : ISikuliExec
	{
		public string ExecuteProject(string projectPath)
		{
			var javaHome = GetPathEnvironmentVariable("JAVA_HOME");
			var sikuliHome = GetPathEnvironmentVariable("SIKULI_HOME");

			var sikuliScriptJarPath = Path.Combine(sikuliHome, "sikuli-script.jar");
			return RunExternalExe(Path.Combine(javaHome, "bin", "java"), " -jar \"" + sikuliScriptJarPath + "\" -r \"" + projectPath + "\"");
		}

		private string GetPathEnvironmentVariable(string name)
		{
			var value = Environment.GetEnvironmentVariable(name);
			if (String.IsNullOrEmpty(value)) throw new Exception(string.Format("Environment variables {0} not set", name));
			if (!Directory.Exists(value)) throw new Exception(string.Format("Environment variables {0} is set to a directory that does not exist: {1}", name, value));
			return value;
		}

		private string RunExternalExe(string filename, string arguments)
		{
			var process = new Process
			{
				StartInfo =
				{
					FileName = filename,
					Arguments = arguments,
					CreateNoWindow = true,
					WindowStyle = ProcessWindowStyle.Hidden,
					UseShellExecute = false,
					RedirectStandardError = true,
					RedirectStandardOutput = true
				}
			};

			var stdOutput = new StringBuilder();
			process.OutputDataReceived += (sender, args) => stdOutput.Append(args.Data);

			process.Start();
			process.BeginOutputReadLine();
			var stdError = process.StandardError.ReadToEnd();
			process.WaitForExit();

			if (process.ExitCode == 0)
			{
				var output = stdOutput.ToString();
				if (output.StartsWith("[error]"))
					throw new Exception(output);

#if(DEBUG)
				Debug.WriteLine("Command Output:");
				Debug.WriteLine(output);
#endif

				return output;
			}

			var message = new StringBuilder();

			message.AppendLine(stdOutput.ToString());

			if (!string.IsNullOrEmpty(stdError))
				message.AppendLine(stdError);

			throw new Exception("Finished with exit code = " + process.ExitCode + ": " + message);
		}
	}
}