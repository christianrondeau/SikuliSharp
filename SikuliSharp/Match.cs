using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SikuliSharp
{
	public class SikuliMatch :Region
	{
		private double _s;
		public SikuliMatch(string matchstring) : base()
		{
			Regex regex = new Regex(@"(?:.*)M\[(\d*),(\d*)\s(\d*)x(\d*)](.*)\s[S][:](.*)\s[C][:](.*)\s\[(.*)\]");
			Match match = regex.Match(matchstring);
			SetX(Convert.ToInt32(match.Groups[1].ToString()));
			SetY(Convert.ToInt32(match.Groups[2].ToString()));
			SetW(Convert.ToInt32(match.Groups[3].ToString()));
			SetH(Convert.ToInt32(match.Groups[4].ToString()));
			_s = double.Parse(match.Groups[6].ToString().Replace(',', '.'), System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture);
		}

		public double GetS()
		{
			return _s;
		}
	}
}
