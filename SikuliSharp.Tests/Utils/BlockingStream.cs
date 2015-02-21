using System;
using System.IO;

namespace SikuliSharp.Tests.Utils
{
	public class BlockingStream : Stream
	{
		private bool _block = true;

		public void Unblock()
		{
			_block = false;
		}

		public override void Flush()
		{
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return 0;
		}

		public override void SetLength(long value)
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			// ReSharper disable once EmptyEmbeddedStatement
			while (_block) ;
			return 0;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
		}

		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanSeek
		{
			get { return false; }
		}

		public override bool CanWrite
		{
			get { return false; }
		}

		public override long Length
		{
			get { return 1; }
		}

		public override long Position
		{
			get { return 0; }
			set { throw new NotImplementedException(); }
		}
	}
}