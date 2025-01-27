let r = System.Random();
let maybe_get_arg i k =
    if Array.length fsi.CommandLineArgs >= i + 1 then
        int fsi.CommandLineArgs[i]
    else
        k
let n = maybe_get_arg 1 1000
let m = maybe_get_arg 2 5000
for i in 0 .. m do
    printfn $"{r.Next(0, n)} {r.Next(0, n)}"
