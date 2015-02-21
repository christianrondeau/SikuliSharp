SikuliSharp
===========

For more information, go to: https://github.com/christianrondeau/SikuliSharp

If you have trouble or find a bug, create an issue: https://github.com/christianrondeau/SikuliSharp/issues

Getting Started
---------------

1. Download Java: http://java.com/en/download/
3. Download and install Sikuli: https://launchpad.net/sikuli/+download and http://www.sikulix.com/quickstart.html
   Select the option to run scripts from command line! The file `sikuli-scripts.jar` should be installed
4. Create an environment variable `SIKULI_HOME` that points to your Sikuli install folder

Usage Example
-------------

// Using the interactive mode

using(var session = new Sikuli.CreateSession())
{
  var pattern = Patterns.FromFile(@"C:\Patterns\MyPattern.png"); 
  Assert.That(session.Exists(pattern), Is.True);
}

// Running a sikuli project

Sikuli.RunProject(@"C:\Projects\MyProject.sikuli");
