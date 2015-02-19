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
			//TODO: Check for JAVA_HOME,  java in PATH or throw
			//TODO: Find the SIKULI_HOME path, or dynamically install
			//TODO: Run in interactive mode for faster feedback (instead of launching for every action)
			const string sikuliHome = @"E:\Dev\Tools\SikuliX";
			var sikuliScriptJarPath = Path.Combine(sikuliHome, "sikuli-script.jar");
			return RunExternalExe("java", " -jar \"" + sikuliScriptJarPath + "\" -r \"" + projectPath + "\"");
		}

		public string RunExternalExe(string filename, string arguments)
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
				var output =  stdOutput.ToString();
				if(output.StartsWith("[error]"))
					throw new Exception(output);
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