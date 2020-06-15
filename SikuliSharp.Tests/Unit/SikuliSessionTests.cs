﻿using System;
using System.Collections.Generic;
using System.Threading;
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

		public IEnumerable<CommandTestData> CommandsTestSource
		{
			get
			{
				yield return new CommandTestData
				{
					Timeout = 15.8518352508545f,
					ExpectedCommand = "print \"SIKULI#: YES\" if exists(::PATTERN::, " + string.Format(Thread.CurrentThread.CurrentCulture, "{0:00.0000}", 10.56789f) + ") else \"SIKULI#: NO\"",
					Method = (session, pattern) => session.Exists(pattern, 10.56789f)
				};

				yield return new CommandTestData
				{
					Timeout = 0f,
					ExpectedCommand = "print \"SIKULI#: YES\" if click(::PATTERN::) else \"SIKULI#: NO\"",
					Method = (session, pattern) => session.Click(pattern)
				};

				yield return new CommandTestData
				{
					Timeout = 0f,
					ExpectedCommand = "print \"SIKULI#: YES\" if click(::PATTERN::.targetOffset(0, -100)) else \"SIKULI#: NO\"",
					Method = (session, pattern) => session.Click(pattern, new Point(0, -100))
				};

				yield return new CommandTestData
				{
					Timeout = 14998.5f,
					ExpectedCommand = "print \"SIKULI#: YES\" if wait(::PATTERN::, 9999) else \"SIKULI#: NO\"",
					Method = (session, pattern) => session.Wait(pattern, 9999f)
				};

				yield return new CommandTestData
				{
					Timeout = 0f,
					ExpectedCommand = "print \"SIKULI#: YES\" if waitVanish(::PATTERN::) else \"SIKULI#: NO\"",
					// ReSharper disable once RedundantArgumentDefaultValue
					Method = (session, pattern) => session.WaitVanish(pattern, 0f)
				};

				yield return new CommandTestData
				{
					Timeout = 0f,
					ExpectedCommand = "print \"SIKULI#: YES\" if hover(::PATTERN::) else \"SIKULI#: NO\"",
					Method = (session, pattern) => session.Hover(pattern)
				};

				yield return new CommandTestData
				{
					Timeout = 0f,
					ExpectedCommand = "print \"SIKULI#: YES\" if hover(::PATTERN::.targetOffset(0, -100)) else \"SIKULI#: NO\"",
					Method = (session, pattern) => session.Hover(pattern, new Point(0, -100))
				};

				yield return new CommandTestData
				{
					Timeout = 0f,
					ExpectedCommand = "print \"SIKULI#: YES\" if rightClick(::PATTERN::) else \"SIKULI#: NO\"",
					Method = (session, pattern) => session.RightClick(pattern)
				};

				yield return new CommandTestData
				{
					Timeout = 0f,
					ExpectedCommand = "print \"SIKULI#: YES\" if rightClick(::PATTERN::.targetOffset(0, -100)) else \"SIKULI#: NO\"",
					Method = (session, pattern) => session.RightClick(pattern, new Point(0, -100))
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

		[Test, TestCaseSource("CommandsTestSource")]
		public void CommandCanReturnTrue(CommandTestData test)
		{
			_runtime.Run(test.ExpectedCommand, ExpectedResultPrefix, test.Timeout).Returns(ExpectedResultYes);

			Assert.That(test.Method(_session, _pattern), Is.True);

			_pattern.Received(1).Validate();
			_runtime.Received(1).Run(test.ExpectedCommand, ExpectedResultPrefix, test.Timeout);
		}

		[Test, TestCaseSource("CommandsTestSource")]
		public void CommandCanReturnFalse(CommandTestData test)
		{
			_runtime.Run(test.ExpectedCommand, ExpectedResultPrefix, test.Timeout).Returns(ExpectedResultNo);

			Assert.That(test.Method(_session, _pattern), Is.False);

			_pattern.Received(1).Validate();
			_runtime.Received(1).Run(test.ExpectedCommand, ExpectedResultPrefix, test.Timeout);
		}

		[Test]
		public void TypeCanReturnTrue()
		{
			const string expectedTypeCommand = "print \"SIKULI#: YES\" if type(\"\\tTEXT\\n\") == 1 else \"SIKULI#: NO\"";

			_runtime.Run(expectedTypeCommand, ExpectedResultPrefix, 0d).Returns(ExpectedResultYes);

			Assert.That(_session.Type(@"\tTEXT\n"), Is.True);

			_runtime.Received(1).Run(expectedTypeCommand, ExpectedResultPrefix, 0d);
		}

		[Test]
		public void TypeCanReturnFalse()
		{
			const string expectedTypeCommand = "print \"SIKULI#: YES\" if type(\"CTRL+C is \\0x3\") == 1 else \"SIKULI#: NO\"";

			_runtime.Run(expectedTypeCommand, ExpectedResultPrefix, 0d).Returns(ExpectedResultNo);

			Assert.That(_session.Type(@"CTRL+C is \0x3"), Is.False);

			_runtime.Received(1).Run(expectedTypeCommand, ExpectedResultPrefix, 0d);
		}

		public IEnumerable<string> InvalidTypeTestSource
		{
			get
			{
				yield return "\x04";
				yield return "\r";
				yield return "\n";
				yield return "\t";
				yield return "\0";
			}
		}

		[Test, TestCaseSource("InvalidTypeTestSource"), ExpectedException(typeof(ArgumentException))]
		public void TypeWithInvalidTextThrows(string text)
		{
			_session.Type(text);
		}
	}
}
