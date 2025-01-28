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
var factors = new ConcurrentDictionary<int, IReadOnlyList<int>>();
IFactorizer factorizer = args[0] switch
{
    "A" => new ThreadFactorizer(inputs, factors),
    "B" => new TaskFactorizer(inputs, factors),
    "C" => new ParallelForFactorizer(inputs, factors),
    _ => throw new ArgumentException("Choose one of A, B or C")
};
var sw = new System.Diagnostics.Stopwatch();
sw.Start();
factorizer.Run();
Console.WriteLine($"Factorized {factors.Count} integers in {sw.ElapsedMilliseconds}ms");

interface IFactorizer
{
    void Run();
}

class ParallelForFactorizer : IFactorizer
{
    private readonly ConcurrentBag<int> _ns;
    private readonly ConcurrentDictionary<int, IReadOnlyList<int>> _fs;

    public ParallelForFactorizer(ConcurrentBag<int> ns, ConcurrentDictionary<int, IReadOnlyList<int>> fs)
    {
        _ns = ns;
        _fs = fs;
    }

    public void Run()
    {
        Parallel.ForEach(_ns, n => {
            _fs[n] = Util.Factorize(n);
        });
    }
}

class ThreadFactorizer : IFactorizer
{
    private readonly ConcurrentBag<int> _ns;
    private readonly ConcurrentDictionary<int, IReadOnlyList<int>> _fs;

    public ThreadFactorizer(ConcurrentBag<int> ns, ConcurrentDictionary<int, IReadOnlyList<int>> fs)
    {
        _ns = ns;
        _fs = fs;
    }

    public void Run()
    {
        var ts = new List<Thread>(_ns.Count);
        while (_ns.TryTake(out var n))
        {
            var t = new Thread(() => { _fs[n] = Util.Factorize(n); });
            t.Start();
            ts.Add(t);
        }
        foreach (var t in ts)
        {
            t.Join();
        }
    }
}

class TaskFactorizer : IFactorizer
{
    private readonly ConcurrentBag<int> _ns;
    private readonly ConcurrentDictionary<int, IReadOnlyList<int>> _fs;

    public TaskFactorizer(ConcurrentBag<int> ns, ConcurrentDictionary<int, IReadOnlyList<int>> fs)
    {
        _ns = ns;
        _fs = fs;
    }

    public void Run()
    {
        var ts = new List<Task>(_ns.Count);
        while (_ns.TryTake(out var n))
        {
            ts.Add(Task.Run(() => { _fs[n] = Util.Factorize(n); }));
        }
        foreach (var t in ts)
        {
            t.Wait();
        }
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
