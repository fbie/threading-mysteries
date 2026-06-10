# Threading Mystery 07 #

Do not look at the code yet!

Instead, compile and run the program:

```
> dotnet build
> dotnet run <variant> <url> ...
```

where `<variant>` is one of

- `A`;
- `B`; or
- `C`.

Try different combinations:

- What does the program do?
- Can you see the variants behave differently?
- If yes, what do you think causes the differences?

If you cannot see any differences, install [ILSpy]() and inspect the CIL byte code (not the decompiled source code!):

```
$ dotnet tool install -g ilspy
$ dotnet ilspy -il bin/bin/Release/net10.0/07.dll
```

Focus on the `Download()` methods. What seems to be the main difference between the implementations? Does anything look unexpected? Take a look at the source code after you have inspected the byte code. Can you correlate the C# syntax with the byte code constructs?
