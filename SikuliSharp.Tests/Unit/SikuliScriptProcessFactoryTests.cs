using System;
using System.IO;
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

		[Test, ExpectedException(ExpectedMessage = "Environment variable SIKULI_HOME not set. Please verify that Sikuli is installed (sikuli-script.jar must be present) and create a SIKULI_HOME environment variable. You may need to restart your command prompt or IDE.")]
		public void ThrowsIfSikuliHomeIsNotSet()
		{
			Environment.SetEnvironmentVariable("SIKULI_HOME", null, EnvironmentVariableTarget.Process);
			var processFactory = new SikuliScriptProcessFactory();
			processFactory.Start(processFactory.GetSikuliVersion());
		}

		[Test]
		public void ThrowsIfSikuliHomeIsSetAndFileDoesNotExist()
		{
			Environment.SetEnvironmentVariable("SIKULI_HOME", "%USERPROFILE%", EnvironmentVariableTarget.Process);
			var processFactory = new SikuliScriptProcessFactory();
			var ex = Assert.Throws<NotSupportedException>(() => processFactory.Start(processFactory.GetSikuliVersion()));
			Assert.That(ex.Message.StartsWith("Could not find a known Sikuli version in SIKULI_HOME"));
		}
	}
}