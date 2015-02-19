namespace SikuliSharp
{
	public class Pattern : IPattern
	{
		public static IPattern FromFile(string path)
		{
			return new FilePattern();
		}
	}

	public class FilePattern : IPattern
	{
	}
}