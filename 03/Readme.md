# Threading Mystery 03 #

Do not look at the code yet!

Instead, compile and run the program:

```
> dotnet build
> dotnet run -- <variant> <number of sets> <path fo file> <path to another file> ...
```

where `<variant>` is one of

- `A`;
- `B`; or
- `C`.

You can pass zero or more paths to text files.

Make sure that the `<number of sets>` argument matches the input files' number of sets, e.g. `input-1000-200-a.txt` has been generated with `<number of sets> = 1000`.

Try different combinations:

- What does the program do?
- Can you see the variants behave differently?
- If yes, what do you think causes the differences?


### Generating New Input ###

In case you do not get any interesting results with the already generated input text files, run `gen-input.fsx` as follows:

```
> dotnet fsi gen-input.fsx <number of sets> <number of operations> input.txt
```
