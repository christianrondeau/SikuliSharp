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
        private Point point;

        public Location(Point pt)
        {
            point = pt;
        }

        public string ToSikuliScript()
        {
            return "Location(" + point.X + "," + point.Y + ")";
        }
    }
}