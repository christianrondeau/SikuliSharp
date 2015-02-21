using System;
using NSubstitute;
using NUnit.Framework;

namespace SikuliSharp.Tests.Unit
{
	[TestFixture]
	public class WithOffsetPatternTests
	{
		private IPattern _originalPattern;

		[SetUp]
		public void GivenAnOriginalPattern()
		{
			_originalPattern = Substitute.For<IPattern>();
		}

		[Test, ExpectedException(typeof(Exception))]
		public void ValidateThrowsIfDecoratedPatternIsItself()
		{
			var withOffsetPattern = new WithOffsetPattern(new WithOffsetPattern(_originalPattern, new Point(0, 0)), new Point(0, 0));

			withOffsetPattern.Validate();
		}

		[Test]
		public void ValidateCallsDecoratedPattern()
		{
			var withOffsetPattern = new WithOffsetPattern(_originalPattern, new Point(0, 0));

			withOffsetPattern.Validate();

			_originalPattern.Received().Validate();
		}

		[Test]
		public void AddsOffsetStringToOriginalPatternString()
		{
			var withOffsetPattern = new WithOffsetPattern(_originalPattern, new Point(-10, 200));
			_originalPattern.ToSikuliScript().Returns("ORIGINAL");

			Assert.That(withOffsetPattern.ToSikuliScript(), Is.EqualTo("ORIGINAL.targetOffset(-10, 200)"));
		}
	}
}