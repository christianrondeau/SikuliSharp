namespace SikuliSharp
{
	public class Region : IRegion
	{
		private int _x, _y, _w, _h;

		public void Validate()
		{
		}

		public string ToSikuliScript()
		{
			return $"Region({_x}, {_y}, {_w}, {_h})";
		}

		public Region()
		{

		}

		public Region(Region region)
		{
			_x = region.GetX();
			_y = region.GetY();
			_w = region.GetW();
			_h = region.GetH();
		}

		public Region(int x, int y, int w, int h)
		{
			_x = x;
			_y = y;
			_w = w;
			_h = h;
		}

		public void SetX(int x)
		{
			_x = x;
		}

		public void SetY(int y)
		{
			_y = y;
		}

		public void SetW(int w)
		{
			_w = w;
		}

		public void SetH(int h)
		{
			_h = h;
		}

		public void MoveTo(int x, int y)
		{
			_x = x;
			_y = y;
		}

		public void SetROI(int x, int y, int w, int h)
		{
			_x = x;
			_y = y;
			_w = w;
			_h = h;
		}

		public int GetX()
		{
			return _x;
		}

		public int GetY()
		{
			return _y;
		}

		public int GetW()
		{
			return _w;
		}

		public int GetH()
		{
			return _h;
		}

		public Location GetTopLeft()
		{
			return new Location(_x, _y);
		}

		public Location GetTopRight()
		{
			return new Location(_x + _w - 1, _y);
		}

		public Location GetBottomLeft()
		{
			return new Location(_x, _y + _h - 1);
		}

		public Location GetBottomRight()
		{
			return new Location(_x + _w - 1, _y + _h - 1);
		}

		public Region Offset(int x, int y)
		{
			return new Region(_x + x, _y + y, _w, _h);
		}

		public Region Offset(Point point)
		{
			return new Region(_x + point.X, _y + point.Y, _w, _h);
		}

		public Region Grow(int range)
		{
			return new Region(_x - range, _y - range, _w + range, _h + range);
		}

		public Region Grow(int width, int height)
		{
			return new Region(_x - width, _y - height, _w + width, _h + height);
		}

		public Region Grow(int left, int right, int top, int bottom)
		{
			return new Region(_x - left, _y - top, _w + right, _h + bottom);
		}

		public Region Above(int range)
		{
			return new Region(_x, _y - range, _w, range);
		}

		public Region Below(int range)
		{
			return new Region(_x, _y + _h, _w, range);
		}

		public Region Left(int range)
		{
			return new Region(_x - range, _y, range, _w);
		}

		public Region Right(int range)
		{
			return new Region(_x + _w, _y, _h, range);
		}
	}

	public interface IRegion
	{
		void Validate();
		string ToSikuliScript();
	}
}
