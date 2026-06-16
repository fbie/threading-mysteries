# .NET Threading Mystery Classes #

Some C# classes that do mysterious threading things.

**Do not browse the source code in `src` folders if you want to solve the mysteries!**


## How to build and run ##

Please install the .NET SDK version 10:

- either by visiting the [download page](https://dotnet.microsoft.com/en-us/download/dotnet/10.0); or
- by executing the script `dotnet-install.sh` in this repository (this is the official Microsoft .NET installation script.)

Make sure that the `dotnet` executable is in your environment variables.

Run the following command from the root folder of this repository:

```
$ dotnet build threading-mysteries -c Release
```

## Solve the Mysteries ##

Enter a mystery directory and run the compiled code using

```
$ dotnet run --project src --no-build -c Release -- [ A | B | C ] <more options> ...
```

Alternatively, you can write and run tests:
```
$ dotnet test test
```

There are (up to) three variants (`A`, `B` and `C`) for each mystery.

- What do you observe when running the different variants?
- Do they behave the same?
- What happens if you change the input arguments?
- Are they predictable?
- Can you form a theory about each underlying implementation?

### Inspecting Mysterious Behavior ###

If a mystery behaves strangely, can you observe anything interesting using `dotnet dump` or `dotnet trace`? Install them via:

```
$ dotnet tool install --global dotnet-dump dotnet-trace
```


## Acknowledgements ##

Threading mysteries are strongly inspired by:

- Shriram Krishnamurthi's [Mystery Languages](https://github.com/shriram/mystery-languages); and
- the paper [Teaching Programming Languages by Experimental and Adversarial Thinking.](https://cs.brown.edu/~sk/Publications/Papers/Published/pkf-teach-pl-exp-adv-think/)

Threading variants are based on:

- Brian Goetz' [Java Concurrency in Practice](https://www.oreilly.com/library/view/java-concurrency-in/0321349601/); and
- Peter Sestoft's course [Practical Parallel and Concurrent Programming](https://www.itu.dk/people/sestoft/itu/PCPP/E2016/) (note that there exist more recent versions of the course).

The vending machine analogy is due to Bent Thomsen.

Threading mysteries were developed at [SimCorp A/S](https://simcorp.com) for the OTC Core Knowledge Sharing series.
