using System;
using System.IO;

namespace SikuliSharp.Tests.Utils
{
	public class ResourcesUtil
	{
		public static string BinPath { get; private set; }

		public static string TestButtonPatternPath { get; set; }
		public static string GreenLabelPatternPath { get; set; }
		public static string RedLabelPatternPath { get; set; }

		static ResourcesUtil()
		{
			BinPath = Path.GetDirectoryName(new Uri(typeof(ResourcesUtil).Assembly.CodeBase).LocalPath);

			if(String.IsNullOrEmpty(BinPath))
				throw new Exception("Unable to find out the assembly codebase path");

			RedLabelPatternPath = Path.Combine(BinPath, "Patterns", "RedLabel.png");
			GreenLabelPatternPath = Path.Combine(BinPath, "Patterns", "GreenLabel.png");
			TestButtonPatternPath = Path.Combine(BinPath, "Patterns", "TestButton.png");
		}
	}
}