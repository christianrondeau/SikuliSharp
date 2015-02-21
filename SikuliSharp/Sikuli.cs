using System;
using System.IO;

namespace SikuliSharp
{
	public static class Sikuli
	{
		public static ISikuliSession CreateSession()
		{
			return new SikuliSession(
				new SikuliRuntime(
					new AsyncTwoWayStreamsHandlerFactory(),
					new SikuliScriptProcessFactoryFactory()
					)
				);
		}

		public static string RunProject(string projectPath)
		{
			if (projectPath == null) throw new ArgumentNullException("projectPath");

			if(!Directory.Exists(projectPath))
				throw new DirectoryNotFoundException(string.Format("Project not found in path '{0}'", projectPath));

			var processFactory = new SikuliScriptProcessFactoryFactory();
			using (var process = processFactory.Start(string.Format("-r {0}", projectPath)))
			{
				var output = process.StandardOutput.ReadToEnd();
				process.WaitForExit();
				return output;
			}
		}
	}
}
