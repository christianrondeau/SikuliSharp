using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SikuliSharp
{
	public interface ISikuliSession : IDisposable
	{
		bool Exists(IPattern pattern, float timeoutInSeconds = 0);
		bool Click(IPattern pattern);
		bool Click(IPattern pattern, Point offset);
		bool Click(IRegion region);
		bool DoubleClick(IPattern pattern);
		bool DoubleClick(IPattern pattern, Point offset);
		bool DoubleClick(IRegion region);
		bool Wait(IPattern pattern, float timeoutInSeconds = 0);
		bool WaitVanish(IPattern pattern, float timeoutInSeconds = 0);
		bool Type(string text);
		bool Hover(IPattern pattern);
		bool Hover(IPattern pattern, Point offset);
		bool Hover(IRegion region);
		bool RightClick(IPattern pattern);
		bool RightClick(IPattern pattern, Point offset);
		bool RightClick(IRegion region);
		bool DragDrop(IPattern fromPattern, IPattern toPattern);
		bool DragDrop(IRegion fromRegion, IRegion toRegion);
		bool Highlight(IRegion region);
		bool Highlight(IRegion region, string color);
		bool Highlight(IRegion region, double seconds);
		bool Highlight(IRegion region, double seconds, string color);
		Match Find(IPattern pattern);
	}

	public class SikuliSession : ISikuliSession
	{
		private static readonly Regex InvalidTextRegex = new Regex(@"[\r\n\t\x00-\x1F]", RegexOptions.Compiled);
		private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;
		private readonly ISikuliRuntime _runtime;

		public SikuliSession(ISikuliRuntime sikuliRuntime)
		{
			_runtime = sikuliRuntime;
			_runtime.Start();
		}

		//IPattern Commands

		public bool Exists(IPattern pattern, float timeoutInSeconds = 0f)
		{
			return RunCommand("exists", pattern, timeoutInSeconds);
		}

		public bool Click(IPattern pattern)
		{
			return RunCommand("click", pattern, 0);
		}

		public bool Click(IPattern pattern, Point offset)
		{
			return RunCommand("click", new WithOffsetPattern(pattern, offset), 0);
		}

		public bool DoubleClick(IPattern pattern)
		{
			return RunCommand("doubleClick", pattern, 0);
		}

		public bool DoubleClick(IPattern pattern, Point offset)
		{
			return RunCommand("doubleClick", new WithOffsetPattern(pattern, offset), 0);
		}

		public bool Wait(IPattern pattern, float timeoutInSeconds = 0f)
		{
			return RunCommand("wait", pattern, timeoutInSeconds);
		}

		public bool WaitVanish(IPattern pattern, float timeoutInSeconds = 0f)
		{
			return RunCommand("waitVanish", pattern, timeoutInSeconds);
		}

		public bool Type(string text)
		{
			if (InvalidTextRegex.IsMatch(text))
				throw new ArgumentException("Text cannot contain control characters. Escape them before, e.g. \\n should be \\\\n", "text");

			var script = string.Format(
				"print \"SIKULI#: YES\" if type(\"{0}\") == 1 else \"SIKULI#: NO\"",
				text
				);

			var result = _runtime.Run(script, "SIKULI#: ", 0d);
			return result.Contains("SIKULI#: YES");
		}

		public bool Hover(IPattern pattern)
		{
			return RunCommand("hover", pattern, 0);
		}

		public bool Hover(IPattern pattern, Point offset)
		{
			return RunCommand("hover", new WithOffsetPattern(pattern, offset), 0);
		}

		public bool RightClick(IPattern pattern)
		{
			return RunCommand("rightClick", pattern, 0);
		}

		public bool RightClick(IPattern pattern, Point offset)
		{
			return RunCommand("rightClick", new WithOffsetPattern(pattern, offset), 0);
		}

		public bool DragDrop(IPattern fromPattern, IPattern toPattern)
		{
			return RunCommand("dragDrop", fromPattern, toPattern, 0);
		}

		public Match Find(IPattern pattern)
		{
			var returnstring = RunCommandWithReturn("find", pattern, 0);
			return new Match(returnstring);
		}

		protected bool RunCommand(string command, IPattern pattern, float commandParameter)
		{
			pattern.Validate();

			var script = string.Format(
				"print \"SIKULI#: YES\" if {0}({1}{2}) else \"SIKULI#: NO\"",
				command,
				pattern.ToSikuliScript(),
				ToSukuliFloat(commandParameter)
				);

			var result = _runtime.Run(script, "SIKULI#: ", commandParameter * 1.5d); // Failsafe
			return result.Contains("SIKULI#: YES");
		}

		protected bool RunCommand(string command, IPattern fromPattern, IPattern toPattern, float commandParameter)
		{
			fromPattern.Validate();
			toPattern.Validate();

			var script = string.Format(
				"print \"SIKULI#: YES\" if {0}({1},{2}{3}) else \"SIKULI#: NO\"",
				command,
				fromPattern.ToSikuliScript(),
				toPattern.ToSikuliScript(),
				ToSukuliFloat(commandParameter)
				);

			var result = _runtime.Run(script, "SIKULI#: ", commandParameter * 1.5d); // Failsafe
			return result.Contains("SIKULI#: YES");
		}

		protected string RunCommandWithReturn(string command, IPattern pattern, float commandParameter)
		{
			pattern.Validate();

			var script = string.Format(
				"print \"SIKULI#: \" + {0}({1}{2}).toString()",
				command,
				pattern.ToSikuliScript(),
				ToSukuliFloat(commandParameter)
				);

			var result = _runtime.Run(script, "SIKULI#: ", commandParameter * 1.5d); // Failsafe
			Console.WriteLine(result);
			if (!result.Contains("SIKULI#:")) throw new Exception("Command failed");
			return result;
		}

		private static string ToSukuliFloat(float timeoutInSeconds)
		{
			return timeoutInSeconds > 0f ? ", " + timeoutInSeconds.ToString("0.####") : "";
		}

		//IRegion Commands

		public bool Click(IRegion region)
		{
			return RunCommand("click", region, 0);
		}

		public bool DoubleClick(IRegion region)
		{
			return RunCommand("doubleClick", region, 0);
		}

		public bool Hover(IRegion region)
		{
			return RunCommand("hover", region, 0);
		}

		public bool RightClick(IRegion region)
		{
			return RunCommand("rightClick", region, 0);
		}

		public bool DragDrop(IRegion fromRegion, IRegion toRegion)
		{
			return RunCommand("dragDrop", fromRegion, toRegion, 0);
		}

		public bool Highlight(IRegion region)
		{
			return RunDotCommand("highlight", region, null, 0);
		}

		public bool Highlight(IRegion region, string color)
		{
			string paramString = String.Format("'{0}'", color);
			return RunDotCommand("highlight", region, paramString, 0);
		}

		public bool Highlight(IRegion region, double seconds)
		{
			string paramString = seconds.ToString("0.####", InvariantCulture);
			return RunDotCommand("highlight", region, paramString, 0);
		}

		public bool Highlight(IRegion region, double seconds, string color)
		{
			string paramString = String.Format("{0}, '{1}'", seconds.ToString("0.####", InvariantCulture), color);
			return RunDotCommand("highlight", region, paramString, 0);
		}

		protected bool RunDotCommand(string command, IRegion region, string paramString, float commandParameter)
		{
			region.Validate();

			var script = string.Format(
				"print \"SIKULI#: YES\" if {0}.{1}({2}) else \"SIKULI#: NO\"",
				region.ToSikuliScript(),
				command,
				paramString
				);

			var result = _runtime.Run(script, "SIKULI#: ", commandParameter * 1.5d); // Failsafe
			return result.Contains("SIKULI#: YES");
		}

		protected bool RunCommand(string command, IRegion region, float commandParameter)
		{
			region.Validate();

			var script = string.Format(
				"print \"SIKULI#: YES\" if {0}({1}) else \"SIKULI#: NO\"",
				command,
				region.ToSikuliScript()
				);

			var result = _runtime.Run(script, "SIKULI#: ", commandParameter * 1.5d); // Failsafe
			return result.Contains("SIKULI#: YES");
		}

		protected bool RunCommand(string command, IRegion fromRegion, IRegion toRegion, float commandParameter)
		{
			fromRegion.Validate();
			toRegion.Validate();

			var script = string.Format(
				"print \"SIKULI#: YES\" if {0}({1},{2}) else \"SIKULI#: NO\"",
				command,
				fromRegion.ToSikuliScript(),
				toRegion.ToSikuliScript()
				);

			var result = _runtime.Run(script, "SIKULI#: ", commandParameter * 1.5d); // Failsafe
			return result.Contains("SIKULI#: YES");
		}

		public void Dispose()
		{
			_runtime.Stop();
		}
	}
}