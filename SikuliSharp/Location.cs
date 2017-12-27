using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SikuliSharp
{
    public interface ILocation
    {
        string ToSikuliScript();
    }

    public class Location : ILocation
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

        public string ToSikuliScript()
        {
            return "Location(" + _point.X + "," + _point.Y + ")";
        }
    }
}