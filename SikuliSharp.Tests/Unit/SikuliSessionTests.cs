using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;

namespace SikuliSharp.Tests.Unit
{
	[TestFixture]
	public class SikuliSessionTests
	{
		private const string ExpectedResultYes = "SIKULI#: YES";
		private const string ExpectedResultNo = "SIKULI#: NO";
		private const string ExpectedResultPrefix = "SIKULI#: ";
		private const string FakePatternScriptOutput = "::PATTERN::";

		private SikuliSession _session;
		private IPattern _pattern;
		private ISikuliRuntime _runtime;

		public class CommandTestData
		{
			public string ExpectedCommand { get; set; }
			public Func<ISikuliSession, IPattern, bool> Method { get; set; }
			public float Timeout { get; set; }
		}

		public IEnumerable<CommandTestData> TestCaseData
		{
			get
			{
				yield return new CommandTestData
				{
					Timeout = 0f,
					ExpectedCommand = "print \"SIKULI#: YES\" if exists(::PATTERN::) else \"SIKULI#: NO\"",
					Method = (session, pattern) => session.Exists(pattern)
				};

				yield return new CommandTestData
				{
					Timeout = 10.56789f,
					ExpectedCommand = "print \"SIKULI#: YES\" if click(::PATTERN::, 10.5679) else \"SIKULI#: NO\"",
					Method = (session, pattern) => session.Click(pattern, 10.56789f)
				};

				yield return new CommandTestData
				{
					Timeout = 9999f,
					ExpectedCommand = "print \"SIKULI#: YES\" if wait(::PATTERN::, 9999.0000) else \"SIKULI#: NO\"",
					Method = (session, pattern) => session.Wait(pattern, 9999f)
				};
			}
		}

		[SetUp]
		public void Setup()
		{
			_pattern = Substitute.For<IPattern>();
			_pattern.ToSikuliScript().Returns(FakePatternScriptOutput);

			_runtime = Substitute.For<ISikuliRuntime>();
			_session = new SikuliSession(_runtime);
		}

		[Test, TestCaseSource("TestCaseData")]
		public void CommandCanReturnTrue(CommandTestData test)
		{
			_runtime.Run(test.ExpectedCommand, ExpectedResultPrefix, test.Timeout).Returns(ExpectedResultYes);

			Assert.That(test.Method(_session, _pattern), Is.True);

			_pattern.Received(1).Validate();
			_runtime.Received(1).Run(test.ExpectedCommand, ExpectedResultPrefix, test.Timeout);
		}

		[Test, TestCaseSource("TestCaseData")]
		public void CommandCanReturnFalse(CommandTestData test)
		{
			_runtime.Run(test.ExpectedCommand, ExpectedResultPrefix, test.Timeout).Returns(ExpectedResultNo);

			Assert.That(test.Method(_session, _pattern), Is.False);

			_pattern.Received(1).Validate();
			_runtime.Received(1).Run(test.ExpectedCommand, ExpectedResultPrefix, test.Timeout);
		}
	}
}
