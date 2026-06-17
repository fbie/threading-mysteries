using System.Collections.Concurrent;

if (args.Length != 2)
{
    Console.WriteLine("Usage: 05.exe [A|B|C] <number-of-factors>");
    return;
}
var n = int.Parse(args[1]);
var inputs = new ConcurrentBag<int>();
for (var i = 100; i < n + 100; i++) {
    inputs.Add(i);
}
IFactorizer factorizer = args[0] switch
{
    "A" => new FactorizerA(),
    "B" => new FactorizerB(),
    "C" => new FactorizerC(),
    _ => throw new ArgumentException("Choose one of A, B or C")
};
var sw = new System.Diagnostics.Stopwatch();
sw.Start();
var factors = factorizer.Run(inputs);
Console.WriteLine($"Factorized {factors.Count} integers in {sw.ElapsedMilliseconds}ms");

public interface IFactorizer
{
    IReadOnlyDictionary<int, IReadOnlyList<int>> Run(ConcurrentBag<int> ns);
}

public class FactorizerA() : IFactorizer
{
    public IReadOnlyDictionary<int, IReadOnlyList<int>> Run(ConcurrentBag<int> ns)
    {
        ConcurrentDictionary<int, IReadOnlyList<int>> fs = new();
        var ts = new List<Thread>(ns.Count);
        while (ns.TryTake(out var n))
        {
            var t = new Thread(() => { fs[n] = Util.Factorize(n); });
            t.Start();
            ts.Add(t);
        }
        foreach (var t in ts)
        {
            t.Join();
        }
        return fs;
    }
}

public class FactorizerB() : IFactorizer
{
    public IReadOnlyDictionary<int, IReadOnlyList<int>> Run(ConcurrentBag<int> ns)
    {
        ConcurrentDictionary<int, IReadOnlyList<int>> fs = new();
        var ts = new List<Task>(ns.Count);
        while (ns.TryTake(out var n))
        {
            ts.Add(Task.Run(() => { fs[n] = Util.Factorize(n); }));
        }
        Task.WaitAll(ts);
        return fs;
    }
}

public class FactorizerC() : IFactorizer
{
    public IReadOnlyDictionary<int, IReadOnlyList<int>> Run(ConcurrentBag<int> ns)
    {
        ConcurrentDictionary<int, IReadOnlyList<int>> fs = new();
        Parallel.ForEach(ns, n => {
            fs[n] = Util.Factorize(n);
        });
        return fs;
    }
}

static class Util
{
    public static IReadOnlyList<int> Factorize(int n)
    {
        var factors = new List<int>();
        var f = 2;
        while (f * f <= n)
        {
            if (n % f == 0)
            {
                factors.Add(f);
                n /= f;
            }
            else
            {
                f += 1;
            }
        }
        if (n != 1)
        {
            factors.Add(n);
        }
        return factors;
    }
}
