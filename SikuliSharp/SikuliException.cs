using System;

namespace SikuliSharp
{
	public class SikuliException : Exception
	{
		public SikuliException(string message)
			: base(message)
		{
		}
	}
}