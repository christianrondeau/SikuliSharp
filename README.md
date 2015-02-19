# SikuliSharp

Yet another implementation of a [Sikuli](http://www.sikulix.com/) wrapper for using it in .NET.

## Status

This is an incomplete **ALPHA**, in progress.

## Why Another Sikuli Wrapper?

There are already existing wrappers, [sikuli-integrator](https://code.google.com/p/sikuli-integrator/) and [sikuli4net](http://sourceforge.net/projects/sikuli4net/), but in both cases they did not work easily, they did not have lots of documentation and used a lot of static classes, making it difficult to inject or replace parts of the code at runtime. I then decided to try building an implementation myself.

## How to Use

Here is a simple example using [NUnit](http://www.nunit.org/). 

```c#
var sikuli = new Sikuli();
var pattern = Patterns.FromFile(@"C:\Patterns\MyPattern.png"); 
Assert.That(sikuli.Exists(pattern), Is.True);
```

## To Do

There are still basic things to be done before being usable even for myself:

* Verify JAVA installation
* Verify Sikuli installation (or download/package it as part of the install)
* Get rid of the yellow banner ([should be solved in 1.1.0](https://bugs.launchpad.net/sikuli/+bug/1221062))
* Use interactive mode to reduce execution time
* Implement all (or at least most) of the available functions (`wait`, `exists`, `click`)
* Write unit tests for all components

## License

Copyright (c) 2015 Christian Rondeau, [The MIT License](LICENSE.md)
