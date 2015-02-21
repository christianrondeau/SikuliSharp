using System;
using System.IO;

namespace SikuliSharp
{
	public class Patterns
	{
		public const float DefaultSimilarity = 0.7f;

		public static IPattern FromFile(string path, float similarity = DefaultSimilarity)
		{
			return new FilePattern(path, similarity);
		}
	}

	public interface IPattern
	{
		void Validate();
		string ToSikuliScript();
	}

	public class FilePattern : IPattern
	{
		private readonly string _path;
		private readonly float _similarity;

		public FilePattern(string path, float similarity)
		{
			if (path == null) throw new ArgumentNullException("path");
			if (similarity < 0 || similarity > 1) throw new ArgumentOutOfRangeException("similarity", similarity, "similarity must be between 0 and 1");

			_path = path;
			_similarity = similarity;
		}

		public void Validate()
		{
			if (!File.Exists(_path))
				throw new FileNotFoundException("Could not find image file specified in pattern: " + _path, _path);
		}

		public string ToSikuliScript()
		{
			return string.Format("Pattern(\"{0}\").similar({1})", _path.Replace(@"\", @"\\"), _similarity);
		}
	}

	public class WithOffsetPattern : IPattern
	{
		private readonly IPattern _pattern;
		private readonly Point _offset;

		public WithOffsetPattern(IPattern pattern, Point offset)
		{
			if (pattern == null) throw new ArgumentNullException("pattern");
			_pattern = pattern;
			_offset = offset;
		}

		public void Validate()
		{
			if(_pattern is WithOffsetPattern)
				throw new Exception("Cannot use WithOffsetPattern with itself");

			_pattern.Validate();
		}

		public string ToSikuliScript()
		{
			return string.Format("{0}.targetOffset({1}, {2})", _pattern.ToSikuliScript(), _offset.X, _offset.Y);
		}
	}
}