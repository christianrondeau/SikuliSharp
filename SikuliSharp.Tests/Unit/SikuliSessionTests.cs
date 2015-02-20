using NSubstitute;
using NUnit.Framework;

namespace SikuliSharp.Tests.Unit
{
	[TestFixture]
	public class SikuliSessionTests
	{
		private const string ExpectedResultYes = "SIKULI#: YES";
		private const string ExpectedResultNo = "SIKULI#: NO";
		private const string ExpectedExistsCommand = "print \"SIKULI#: YES\" if exists(::PATTERN::) else \"SIKULI#: NO\"";
		private const string ExpectedClickCommand = "print \"SIKULI#: YES\" if click(::PATTERN::) else \"SIKULI#: NO\"";
		private const string ExpectedResultPrefix = "SIKULI#: ";
		private const string FakePatternScriptOutput = "::PATTERN::";

		private SikuliSession _session;
		private IPattern _pattern;
		private ISikuliRuntime _runtime;

		[SetUp]
		public void Setup()
		{
			_pattern = Substitute.For<IPattern>();
			_pattern.ToSikuliScript().Returns(FakePatternScriptOutput);

			_runtime = Substitute.For<ISikuliRuntime>();
			_session = new SikuliSession(_runtime);
		}

		[Test]
		public void ExistsReturnsTrue()
		{
			_runtime.Run(ExpectedExistsCommand, ExpectedResultPrefix).Returns(ExpectedResultYes);

			Assert.That(_session.Exists(_pattern), Is.True);

			_pattern.Received(1).Validate();
			_runtime.Received(1).Run(ExpectedExistsCommand, ExpectedResultPrefix);
		}

		[Test]
		public void ExistsReturnsFalse()
		{
			_runtime.Run(ExpectedExistsCommand, ExpectedResultPrefix).Returns(ExpectedResultNo);

			Assert.That(_session.Exists(_pattern), Is.False);

			_pattern.Received(1).Validate();
			_runtime.Received(1).Run(ExpectedExistsCommand, ExpectedResultPrefix);
		}

		[Test]
		public void ClickReturnsTrue()
		{
			_runtime.Run(ExpectedClickCommand, ExpectedResultPrefix).Returns(ExpectedResultYes);

			Assert.That(_session.Click(_pattern), Is.True);

			_pattern.Received(1).Validate();
			_runtime.Received(1).Run(ExpectedClickCommand, ExpectedResultPrefix);
		}

		[Test]
		public void ClickReturnsFalse()
		{
			_runtime.Run(ExpectedClickCommand, ExpectedResultPrefix).Returns(ExpectedResultNo);

			Assert.That(_session.Click(_pattern), Is.False);

			_pattern.Received(1).Validate();
			_runtime.Received(1).Run(ExpectedClickCommand, ExpectedResultPrefix);
		}
	}
}
