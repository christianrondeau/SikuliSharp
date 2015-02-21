# SikuliSharp

Yet another implementation of a [Sikuli](http://www.sikulix.com/) wrapper for using it in .NET.

## Why Another Sikuli Wrapper?

There are already existing wrappers, [sikuli-integrator](https://code.google.com/p/sikuli-integrator/) and [sikuli4net](http://sourceforge.net/projects/sikuli4net/), but I had trouble running them, they use an additional level of wrapping, they do not seem very active, and especially they used a lot of static classes, which makes it difficult to extend. I then decided to try building an implementation myself.

## How to Use

Check these steps first:

1. [Download Java](http://java.com/en/download/)
2. [Download Sikuli 1.0.1](https://launchpad.net/sikuli/sikulix/1.0.1), then [install it](http://www.sikulix.com/quickstart.html)
   Select the option to run scripts from command line - the file `sikuli-scripts.jar` must be installed
3. Create an environment variable `SIKULI_HOME` that points to your Sikuli install folder

Here is a simple example using [NUnit](http://www.nunit.org/):

```c#
using(var session = new Sikuli.CreateSession())
{
  var pattern = Patterns.FromFile(@"C:\Patterns\MyPattern.png"); 
  Assert.That(session.Exists(pattern), Is.True);
}
```

You can also simply run a project:

```c#
Sikuli.RunProject(@"C:\Projects\MyProject.sikuli");
```

## How Does it Work

A `SikuliSession` launches an instance of the Sikuli interactive script engine using `java.exe -jar sikuli-script.jar -i`. All commands are sent to the interactive console, and the output is then parsed.

## Documentation

Remember that this library simply wraps Sikuli; the same limitations apply. You can't use your computer while tests are running since Sikuli takes control of the mouse, and patterns may require fine tuning (using `similarity`).

### `Sikuli`

The `Sikuli` object is the main entry point for all operations. It contains two methods:

* `CreateSession`, which returns an `ISikuliSession`, with which you can execute Sikuli commands
* `RunProject` which simply runs a `.sikuli` project and returns the console output

### `SikuliSession`

All actions must happen within a `ISikuliSession`.

```c#
using (var session = Sikuli.CreateSession())
{
  // Do stuff here
}
```

All commands run against a `SikuliSession` instance. Also, all commands take a second `timeoutSeconds` parameter, that if left empty, will wait "forever".

* `session.Exists(pattern)` checks if the pattern exists on the screen
* `session.Click(pattern)` tries to click on the pattern if it exists on the screen
* `session.Wait(pattern)` tries to click on the pattern if it exists on the screen
* `session.WaitVanish(pattern)` waits for the pattern to disappear from the screen
* `session.Type(text)` sends the characters to the application; don't forget to escape special characters!

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
    "print \"RESULT: OK\" if exists(\"C:\\\\Patterns\\\\MyPattern.png\") else \"RESULT: FAIL\"",
    "RESULT:",
    0d
    );

  Assert.That(result, Is.StringContaining("RESULT:OK"));
}
```

Note that if you don't provide a timeout and the `resultPrefix` parameter is not printed in the console, the runtime will hang.


## Contributions

This project is open for contributions through pull requests or feedback. This project is too small to have a contribution guide yet, but usual rules apply: make sure all tests work and try to keep the same coding style.

Here are some improvement ideas:

* Get rid of the yellow banner ([should be solved in 1.1.0](https://bugs.launchpad.net/sikuli/+bug/1221062))
* Implement other sikuli functions
* It may be interesting to provide other `IPattern` implementation, e.g. embedded resources
* If possible, install Sikuli at runtime... not sure about this one though. Maybe a dedicated function such as `Sikuli.InstallSikuli();`

## License

Copyright (c) 2015 Christian Rondeau, [The MIT License](LICENSE.md)
