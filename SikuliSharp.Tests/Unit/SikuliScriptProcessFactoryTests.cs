using System;
using NUnit.Framework;

namespace SikuliSharp.Tests.Unit
{
	[TestFixture]
	public class SikuliScriptProcessFactoryTests
	{
		[Test]
		public void CanGuessJavaPath()
		{
			Assert.That(SikuliScriptProcessFactory.GuessJavaPath(), Is.Not.Empty.Or.Null);
		}

		[Test]
		public void CanGuessJavaPathWhenJavaHomeNotSet()
		{
			Environment.SetEnvironmentVariable("JAVA_HOME", null, EnvironmentVariableTarget.Process);
			Assert.That(SikuliScriptProcessFactory.GuessJavaPath(), Is.Not.Empty.Or.Null);
		}
	}
}