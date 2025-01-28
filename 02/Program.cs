using System.IO;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading;

if (args.Length < 2) {
    Console.WriteLine("Usage: 02.exe [A|B|C] <path to file> <path to file>...");
    return;
}
IHistogram h = args[0] switch {
    "A" => new HistogramA(),
    "B" => new HistogramB(),
    "C" => new HistogramC(),
    _ => throw new ArgumentException("Choose one of A, B or C")
};

var threads = new List<Thread>();
for (var i = 1; i < args.Length; i++) {
    var arg = args[i];
    var t = new Thread(() => {
        var content = File.ReadAllText(arg);
        foreach (var c in content) {
            if (char.IsLetterOrDigit(c)) {
                h.Add(c);
            }
        }
    });
    // Don't start threads just yet, we want
    // as much parallel execution as possible.
    threads.Add(t);
}
foreach (var t in threads) {
    t.Start();
}
foreach (var t in threads) {
    t.Join();
}
h.Print();

// --- The mysterious implementation. ---

interface IHistogram {
    void Add(char c);
    void Print();
}

class HistogramA : IHistogram {
    private readonly Dictionary<char, int> _chars = new();

    public void Add(char c) {
        // This fails when called by
        // more than one thread simultaneously.
        if (_chars.TryGetValue(c, out var i)) {
            _chars[c] = i + 1;
        } else {
            _chars[c] = 1;
        }
    }

    public void Print() {
        foreach (var (c, i) in _chars) {
            Console.WriteLine($"{c} : {i}");
        }
    }
}

class HistogramB : IHistogram {
    private readonly Dictionary<char, int> _chars = new();

    public void Add(char c) {
        // Lock on _chars; this provides exclusive
        // access to _chars, but also might be
        // slower on large workloads.
        lock (_chars) {
            if (_chars.TryGetValue(c, out var i)) {
                _chars[c] = i + 1;
            } else {
                _chars[c] = 1;
            }
        }
    }

    public void Print() {
        foreach (var (c, i) in _chars) {
            Console.WriteLine($"{c} : {i}");
        }
    }
}

class HistogramC : IHistogram {
    private readonly ConcurrentDictionary<char, int> _chars = new();

    public void Add(char c) {
        // This simply uses the thread-safe API
        // of ConcurrentDictionary. Note that
        // the closures are non-capturing, which
        // is important. Otherwise, we would
        // allocate one or two new closures per
        // call, which would greatly affect
        // performance.
        _chars.AddOrUpdate(c, _ => 1, (c, i) => i + 1);
    }

    public void Print() {
        foreach (var (c, i) in _chars) {
            Console.WriteLine($"{c} : {i}");
        }
    }
}
