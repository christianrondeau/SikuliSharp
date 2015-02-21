# SikuliSharp

Yet another implementation of a [Sikuli](http://www.sikulix.com/) wrapper for using it in .NET.

## Why Another Sikuli Wrapper?

There are already existing wrappers, [sikuli-integrator](https://code.google.com/p/sikuli-integrator/) and [sikuli4net](http://sourceforge.net/projects/sikuli4net/), but I had trouble running them, they use an additional level of wrapping, they do not seem very active, and especially they used a lot of static classes, which makes it difficult to extend. I then decided to try building an implementation myself.

## How to Use

Check these steps first:

1. [Download Java](http://java.com/en/download/)
2. Create an environment variable `JAVA_HOME` that points to your Java folder (e.g. `C:\Program Files (x86)\Java\jre1.8.0_31`)
3. [Download Sikuli](https://launchpad.net/sikuli/+download), [install it](http://www.sikulix.com/quickstart.html) and make sure to select the option to run scripts from command line
4. Create an environment variable `SIKULI_HOME` that points to your Sikuli install folder

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
new Sikuli.RunProject(@"C:\MyProject.sikuli");
```

## How Does it Work

A `SikuliSession` launches an instance of the Sikuli interactive script engine using `java.exe -jar sikuli-script.jar -i`. All commands are sent to the interactive console, and the output is then parsed.

## Documentation

Remember that this library simply wraps Sikuli; the same limitations apply. You can't use your computer while tests are running since Sikuli takes control of the mouse, and patterns may require fine tuning (using `similarity`).

### Sikuli

The `Sikuli` object is the main entry point for all operations. It contains two methods:

* `CreateSession`, which returns an `ISikuliSession`, with which you can execute Sikuli commands
* `RunProject` which simply runs a `.sikuli` project and returns the console output

### SikuliSession

All actions must happen within a `ISikuliSession`.

```c#
using (var session = Sikuli.CreateSession())
{
  // Do stuff here
}
```

### Patterns

Creating a pattern from a file path

```c#
var pattern = Patterns.FromFile(@"C:\Patterns\MyPattern.png"); 
```

You can also specify a similarity (between `0f` an `1f`)

```c#
var pattern = Patterns.FromFile(@"C:\Patterns\MyPattern.png", 0.6f); 
```

### Commands

All commands run against a `SikuliSession` instance. Also, all commands take a second `timeoutSeconds` parameter, that if left empty, will wait "forever".

* `session.Exists(pattern)` checks if the pattern exists on the screen
* `session.Click(pattern)` tries to click on the pattern if it exists on the screen
* `session.Wait(pattern)` tries to click on the pattern if it exists on the screen

## Future

Here are some ideas of this to improve, if there is interest:

* Get rid of the yellow banner ([should be solved in 1.1.0](https://bugs.launchpad.net/sikuli/+bug/1221062))
* Implement other sikuli functions (`wait`, `offset`)
* It may be interesting to provide other `IPattern` implementation, e.g. embedded resources
* If possible, install Sikuli at runtime. Not sure about this one though.
* Allow providing a Sikuli project (`.sikuli`). That would be fairly easy, we just need to run the project using `-r` and gather the output. I'm thinking something along the lines of `SikuliSession.RunProject(@"C:\MyProject.sikuli")`, which returns the console output.

## Contributions

This project is open for contributions through pull requests or feedback. This project is too small to have a contribution guide yet, but usual rules apply: make sure all tests work and try to keep the same coding style.

## License

Copyright (c) 2015 Christian Rondeau, [The MIT License](LICENSE.md)
