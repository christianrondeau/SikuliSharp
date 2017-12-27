using System;
using System.Globalization;
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

		public static IPattern Location(int x, int y)
		{
			return new Location(x, y);
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
			if (path == null) throw new ArgumentNullException(nameof(path));
			if (similarity < 0 || similarity > 1) throw new ArgumentOutOfRangeException(nameof(similarity), similarity, "similarity must be between 0 and 1");

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
			return string.Format(NumberFormatInfo.InvariantInfo, "Pattern(\"{0}\").similar({1})", _path.Replace(@"\", @"\\"), _similarity);
		}
	}

	public class WithOffsetPattern : IPattern
	{
		private readonly IPattern _pattern;
		private readonly Point _offset;

		public WithOffsetPattern(IPattern pattern, Point offset)
		{
			if (pattern == null) throw new ArgumentNullException(nameof(pattern));
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
			return $"{_pattern.ToSikuliScript()}.targetOffset({_offset.X}, {_offset.Y})";
		}
	}

    public class Location : IPattern
    {
        private Point _point;

	    public Location(int x, int y)
			: this(new Point(x, y))
		{
		}

        public Location(Point pt)
        {
            _point = pt;
        }

	    public void Validate()
	    {
		    if(_point.X < 0 || _point.Y < 0)
				throw new Exception("Cannot target a negative position");
	    }

	    public string ToSikuliScript()
        {
            return $"Location({_point.X},{_point.Y})";
        }
    }
}