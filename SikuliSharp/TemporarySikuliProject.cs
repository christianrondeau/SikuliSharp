using System;
using System.IO;

namespace SikuliSharp
{
	internal class TemporarySikuliProject : IDisposable
	{
		private const string SikuliProjectFolderExtension = ".sikuli";
		private const string SikuliPythonScriptExtension = ".py";

		public string ProjectName { get; private set; }
		public string ProjectPath { get; private set; }
		public string ScriptPath { get; private set; }

		public TemporarySikuliProject()
		{
			ProjectName = Guid.NewGuid().ToString();
			ProjectPath = Path.Combine(Path.GetTempPath(), ProjectName + SikuliProjectFolderExtension);
			ScriptPath = Path.Combine(ProjectPath, ProjectName + SikuliPythonScriptExtension);
			Directory.CreateDirectory(ProjectPath);
		}

		public void Dispose()
		{
			if (!String.IsNullOrEmpty(ProjectPath) && Directory.Exists(ProjectPath))
				Directory.Delete(ProjectPath, true);
		}
	}
}