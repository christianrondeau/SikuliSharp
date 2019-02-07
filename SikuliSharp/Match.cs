using System;
using System.Globalization;
using Regex = System.Text.RegularExpressions;

namespace SikuliSharp
{
	public class Match : Region
	{
		private static readonly Regex.Regex MatchRegex = new Regex.Regex(@"(?:.*)M\[(\d*),(\d*)\s(\d*)x(\d*)](.*)\s[S][:](.*)\s[C][:](.*)\s\[(.*)\]", Regex.RegexOptions.Compiled);

		private double _s;

		public Match(string matchstring)
		{
			var match = MatchRegex.Match(matchstring);
			SetX(Convert.ToInt32(match.Groups[1].ToString()));
			SetY(Convert.ToInt32(match.Groups[2].ToString()));
			SetW(Convert.ToInt32(match.Groups[3].ToString()));
			SetH(Convert.ToInt32(match.Groups[4].ToString()));
			_s = double.Parse(match.Groups[6].ToString().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture);
		}

		public double GetS()
		{
			return _s;
		}
	}
}
