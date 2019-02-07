# SikuliSharp

[![Join the chat at https://gitter.im/christianrondeau/SikuliSharp](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/christianrondeau/SikuliSharp?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

Yet another implementation of a [Sikuli](http://www.sikulix.com/) wrapper for using it in .NET.

## Why Another Sikuli Wrapper?

There are already existing wrappers, [sikuli-integrator](https://code.google.com/p/sikuli-integrator/) and [sikuli4net](http://sourceforge.net/projects/sikuli4net/), but I had trouble running them, they use an additional level of wrapping, they do not seem very active, and especially they used a lot of static classes, which makes it difficult to extend. I then decided to try building an implementation myself.

## How to Use

Check these steps first:

1. [Download Java](http://java.com/en/download/)
2. [Download Sikuli 1.1.0](https://raiman.github.io/SikuliX1/downloads.html):
	* [jython-standalone-2.7.1.jar](https://repo1.maven.org/maven2/org/python/jython-standalone/2.7.1/jython-standalone-2.7.1.jar)  
	* [sikulixapi.jar](https://raiman.github.io/SikuliX1/sikulixapi.jar)  
3. Create an environment variable `SIKULI_HOME` that points to the Sikuli folder that contains the .jar files

Here is a simple example:

```c#
using(var session = Sikuli.CreateSession())
{
  var pattern = Patterns.FromFile(@"C:\Patterns\MyPattern.png"); 
  if(session.Exists(pattern))
  {
  	  Console.WriteLine("The pattern exists!");
  }
}
```

You can also simply run a project (1.0.1 and 1.1.0 only, 1.1.4 not supported yet):

```c#
Sikuli.RunProject(@"C:\Projects\MyProject.sikuli");
```

## How Does it Work

A `SikuliSession` launches an instance of the Sikuli interactive script engine using a Jython interactive console. All commands are sent to the console's standard input, and the output is then parsed.

## Documentation

Remember that this library simply wraps Sikuli; the same limitations apply. You can't use your computer while tests are running since Sikuli takes control of the mouse, and patterns may require fine tuning (using `similarity`).

### `Sikuli`

The `Sikuli` object is the main entry point for all operations. It contains two methods:

* `CreateSession`, which returns an `ISikuliSession`, with which you can execute Sikuli commands
* `RunProject` which simply runs a `.sikuli` project and returns the console output

### `SikuliSession`

All commands must be run within a `ISikuliSession`.

```c#
using (var session = Sikuli.CreateSession())
{
  // Execute commands here
}
```

All commands run against the `ISikuliSession` instance. They also can receive a `timeoutSeconds` parameter. If left empty, commands will wait "forever".

* `session.Exists` checks if the pattern exists on the screen
* `session.Click` Click to the `Point offset` distance from the pattern
* `session.DoubleClick` Double-click on the pattern if it exists on the screen
* `session.RightClick` Right-click to the `Point offset` distance from the pattern
* `session.Wait` tries to click on the pattern if it exists on the screen
* `session.WaitVanish` waits for the pattern to disappear from the screen
* `session.Type` sends the characters to the application; don't forget to double-escape special characters (e.g. `"\\n"` should be `"\\\\n"` or `@"\\n"`)
* `session.Hover` Hover to the `Point offset` distance from the pattern
* `session.DragDrop` Drags from a pattern to another
* `session.Find` Drags from a pattern to another
* `session.Highlight` Highlights an element on the screen

### `Patterns`

Creating a pattern from a file path

```c#
var pattern = Patterns.FromFile(@"C:\Patterns\MyPattern.png"); 
```

You can also specify a similarity (between `0f` an `1f`)

```c#
var pattern = Patterns.FromFile(@"C:\Patterns\MyPattern.png", 0.6f); 
```

### `SikuliRuntime`

If you need more functions, you can create your own. Here is an example:

```c#
using(var runtime = Sikuli.CreateRuntime())
{
  runtime.Start();

  var result = runtime.Run(
    @"print ""RESULT: OK"" if exists(""C:\\Patterns\\MyPattern.png"") else ""RESULT: FAIL""",
    "RESULT:",
    0d
    );

  Assert.That(result, Is.StringContaining("RESULT: OK"));
}
```

You **must** print a string that will show up regardless of whether the test succeeded or not. If you don't provide a timeout and the `resultPrefix` parameter is not printed in the console, the runtime will hang.

Also remember that this sends [Jython](http://www.jython.org/) to the console. Therefore, you must double-escape strings accordingly.

## Contributions

This project is open for contributions through pull requests or feedback. This project is too small to have a contribution guide yet, but usual rules apply: make sure all tests work and try to keep the same coding style.

Here are some improvement ideas:

* Get rid of the yellow banner ([should be solved in 1.1.0](https://bugs.launchpad.net/sikuli/+bug/1221062))
* Implement other sikuli functions
* It may be interesting to provide other `IPattern` implementation, e.g. embedded resources
* If possible, install Sikuli at runtime... not sure about this one though. Maybe a dedicated function such as `Sikuli.InstallSikuli();`

## License

Copyright (c) 2019 Christian Rondeau, [The MIT License](LICENSE.md)
