---
title: '.NET Threading Mysteries'
author: 'Florian Biermann'
email: 'florian.biermann@protonmail.com'
monofont: 'Fira Code'
monofont-size: 16
---

# What #

- .NET uses "threads" to structure concurrent computations.
- A thread is an independent unit of execution.
- Lots of benefits: responsive GUI, higher throughput, more modular code.
- Lots of problems: race conditions, deadlocks, starvation, and so on.
- Problems are real and hard to detect.

---

# Concurrent Programming #

Writing correct threading code is:

- "easy": if you follow the basic patterns; but
- "hard": if you want to have really high performance, throughput, etc.

Analyzing threading bugs is:

- "harder": occur spuriously, "Heisenbugs", debugger interferes.

---

# Parallelism #

![](slides/vending-machines-parallelism.svg)

---

# Concurrency #

![](slides/vending-machines-concurrency.svg)

---

# Crash Course in .NET Threading 1/2 #

```csharp
var t = new Thread(() => ...);
t.Start();                   // Start thread t.
t.Join();                    // Wait for t to finish.
var s = Task.Run(() => ...); // Submit task to thread pool.
s.Wait();                    // Wait for task s to finish.
```

---

# Crash Course in .NET Threading 2/2 #

```csharp
class C {
    readonly object o = new(); // Why readonly?
    public void F() {
        // Lock on o from calling thread.
        lock (o) { ... }
    }}
```

---

# This Session #

There are six "threading mysteries". Each mystery:

- takes some kind of input (numbers, files, ...);
- does *something* with .NET threads; and
- comes in up to three variants (A, B, C.)


**Goals:**

1. experience unexpected(?) behavior;
2. build a hypothesis about causes; and
3. connect the two.

---

# Your Task for Today #

In small groups (2-3), your task is to:

1. compile the code;
2. **DO NOT** look at the sources; instead
3. run each variant, use different arguments.

Observe and interpret!

---

# Do Now! #

```
$ git clone https://codeberg.org/fbie/threading-mysteries.git
$ cd threading-mysteries
$ dotnet build threading-mysteries.slnx -c Release
$ dotnet test threading-mysteries-tests.slnx
```

---

# Solving the Mysteries #

In the code we display:

- modifiers like `readonly` are omitted, unless important; and
- constructors are omitted.

---

# Mystery 1 #

```csharp
interface ISequenceBuilder {
    bool Next();
}
ISqeuenceBuilder b = ...;
var t = new Thread(() => { while (b.Next()); });
t.Start();
while (b.Next());
t.Join();
```

---

# Mystery 1: A #

```csharp
class SequenceBuilderA(int _n) : ISequenceBuilder {
    List<int> _ns = new List<int>();
    int _current = 0;

    public bool Next() {
        if (_current++ < _n) {
            _ns.Add(_current);
            return true;
        }
        return false;
    }}
```
---

# Mystery 1: B #

```csharp
class SequenceBuilderB(int _n) : ISequenceBuilder {
    List<int> _ns = new List<int>();
    volatile int _current = 0;

    public bool Next() {
        if (_current++ < _n) {
            _ns.Add(_current);
            return true;
        }
        return false;
    }}
```
---

# Mystery 1: C #

```csharp
class SequenceBuilderC(int _n) : ISequenceBuilder {
    List<int> _ns = new List<int>();
    int _current = 0;

    public bool Next() {
        lock (_ns) {
            if (_current++ < _n) {
                _ns.Add(_current);
                return true;
            }
            return false;
        }}}
```

---

# Mystery 2 #

```csharp
interface IHistogram {
    void Add(char c);
    void Print();
}
IHistogram h = ...;
for (var i = 1; i < args.Length; i++) {
    var arg = args[i];
    new Thread(() => {
        var content = File.ReadAllText(arg);
        foreach (var c in content)
            if (char.IsLetterOrDigit(c))
                h.Add(c);
        }).Start();
}
```
---

# Mystery 2: A #

```csharp
class HistogramA : IHistogram {
    Dictionary<char, int> _chars = new();

    public void Add(char c) {
        if (_chars.TryGetValue(c, out var i))
            _chars[c] = i + 1;
        else
            _chars[c] = 1;
        }}
```
---

# Mystery 2: B #

```csharp
class HistogramB : IHistogram {
    Dictionary<char, int> _chars = new();

    public void Add(char c) {
        lock (_chars) {
            if (_chars.TryGetValue(c, out var i))
                _chars[c] = i + 1;
            else
                _chars[c] = 1;
            }}}
```
---

# Mystery 2: C #

```csharp
class HistogramC : IHistogram {
    ConcurrentDictionary<char, int> _chars = new();

    public void Add(char c) {
        chars.AddOrUpdate(c,
            _ => 1,
            (c, i) => i + 1);
    }
```
---

# Mystery 3 #

```csharp
abstract class Set {
    public int Id { get; }
    public int Size { get; set; }
    public Set Parent { get; set; }

    public Set Find() {
        if (Parent != this) {
            Parent = Parent.Find();
        }
        return Parent;
    }

    public abstract Set Union(Set other);
```
---

# Mystery 3 (continued) #

```csharp
for (var i = 2; i < args.Length; i++) {
    var arg = args[i];
    new Thread(() => {
        var lines = File.ReadAllLines(arg);
        foreach (var line in lines) {
            var (x, y) = Split(line);
            sets[x].Union(sets[y]);
        }
    }).Start();
}
```
---

# Mystery 3: B #

```csharp
class SetB : Set {
    public override Set Union(Set other) {
        Assign(this, other);
    }
}
```

---

# Mystery 3: C #

```csharp
class SetC : Set {
    public override Set Union(Set other) {
        var x = this.Find();
        var y = other.Find();
        lock (x) { lock (y) {
            return Assign(x, y);
        }}}}
```
---

# Mystery 3: A #

```csharp
class SetA : Set {
    public override Set Union(Set other) {
        var x = this.Find();
        var y = other.Find();
        if (y.Id < x.Id) {
            (x, y) = (y, x);
        }
        lock (x) { lock (y) {
            return Assign(x, y);
        }}}}
```
---

# Mystery 4 #
```csharp
interface ICounter {
    long Value { get; }
    void Add(long value);
}
ICounter c = ...;
for (var i = 0; i < threads; i++)
    new Thread(() => {
        for (var j = 0; j < increments; j++)
            counter.Increment();
    }).Start();
}
```
---

# Mystery 4: C #

```csharp
class CounterC : ICounter {
    readonly object _lock = new object();
    long _value = 0L;

    public long Value { get {
        lock (_lock) { return _value; }
    }}

    public void Add(long value) {
        lock (_lock) { _value += value; }
    }
}

```
---

# Mystery 4: A #
```csharp
class CounterA : ICounter {
    long _value = 0L;

    public long Value
        => Interlocked.Read(ref _value);

    public void Add(long value) {
        Interlocked.Add(ref _value, value);
    }
}
```
---

# Mystery 4: B #
```csharp
class CounterB : ICounter
{
    [StructLayout(LayoutKind.Explicit, Size=64)]
    struct PaddedLong { [FieldOffset(0)] public long V; }

    private readonly PaddedLong[] _vs = new PaddedLong[32];

    public long Value { get {
        var s = 0L;
        for (var i = 0; i < _vs.Length; i++)
            s += Interlocked.Read(ref _vs[i].V);
        return s;
    }}

    public void Add(long n) {
        var i = Thread.CurrentThread.ManagedThreadId % 32;
        Interlocked.Add(ref _vs[i].V, n);
    }
```

---


# Mystery 5 #

```csharp
interface IFactorizer {
    void Run();
}
IFactorizer f = ...;
ConcurrentBag<int> input = ...;
var factors = f.Run(input);
```

---

# Mystery 5: A #

```csharp
class FactorizerA : IFactorizer {
    public IReadOnlyDictionary<int, IReadOnlyList<int>> Run(ConcurrentBag<int> ns) {
        ConcurrentDictionary<int, IReadOnlyList<int>> fs = new();
        var ts = new List<Thread>(ns.Count);
        while (_ns.TryTake(out var n)) {
            var t = new Thread(() => {
                fs[n] = Util.Factorize(n);
                });
            t.Start();
            ts.Add(t);
        }
        foreach (var t in ts)
        t.Join();
        return fs;
        }}
```

---

# Mystery 5: B #

```csharp
class FactorizerB : IFactorizer {
    public IReadOnlyDictionary<int, IReadOnlyList<int>> Run(ConcurrentBag<int> ns) {
        ConcurrentDictionary<int, IReadOnlyList<int>> fs = new();
        var ts = new List<Task>(_ns.Count);
        while (_ns.TryTake(out var n))
            ts.Add(Task.Run(() => {
                fs[n] = Util.Factorize(n);
                }));
        foreach (var t in ts)
            t.Wait();
        return fs;
    }}
```

---

# Mystery 5: C #

```csharp
class FactorizerC : IFactorizer {
    public IReadOnlyDictionary<int, IReadOnlyList<int>> Run(ConcurrentBag<int> ns) {
        ConcurrentDictionary<int, IReadOnlyList<int>> fs = new();
        Parallel.ForEach(_ns, n => {
            fs[n] = Util.Factorize(n);
        });
        return fs;
    }
}
```

---

# Mystery 6 #

```csharp
interface IWaiter {
    void Wait(Barrier barrier);
}
IWaiter waiter = ...;
var barrier = new Barrier(n + 1);
for (var i = 0; i < n; i++)
    runner.Wait(barrier);
barrier.SignalAndWait();

```
---

# Mystery 6: A #

```csharp
class WaiterA : IRunner {
    public void Wait(Barrier barrier) {
        new Thread(() => {
            barrier.SignalAndWait();
        })
        .Start();
    }}
```

---

# Mystery 6: B #

```csharp
class WaiterB : IRunner {
    public void Wait(Barrier barrier) {
        Task.Run(() => {
            barrier.SignalAndWait();
        });
    }}
```

---

# Mystery 7 #

```csharp
interface IDownloader {
    public Task<int> Download(string url);
}
IDownloader downloader = ...;
var bytes = Task.WhenAll(args[1..].Select(url => downloader.Download(Fixup(url)))).Result.Sum();
```

---

# Mystery 7: A #

```csharp
public Task<int> Download(string url) {
    int bytes = 0;
    var t = new Thread(() => {
        bytes = client.GetByteArrayAsync(url).Result.Length;
    });
    t.Start();
    t.Join();
    return Task.FromResult(bytes);
}
```

---

# Mystery 7: B #

```csharp
public Task<int> Download(string url)
    => Task.Run(() => client
        .GetByteArrayAsync(url)
        .ContinueWith(t => t.Result.Length));
}
```

---

# Mystery 7: C #

```csharp
public async Task<int> Download(string url)
{
    var bytes = await client.GetByteArrayAsync(url);
    return bytes.Length;
}
```

---

# Mystery 7: C behind the scenes #

The C# compiler generates an **abstract state-machine** that handles task resumption and exceptions more robustly than `Task.ContinueWith()`!

```
$ ilspycmd -lv CSharp4 -t AsyncDownloader bin/Release/net10.0/07.dll
```

---

# Take-Aways #

<p style="text-align: center;">Shared mutable state is the root of all evil.</p>

- Purely functional program guards against this; but
- there is still state _somewhere_, so beware!
- Different models of concurrency: message passing, actors etc.


# Recommended Reading #

- "Java Concurrency in Practice", Brian Goetz with Tim Peierls et al., 2006
- "C# Precisely", Peter Sestoft, 2nd edition, 2012
- "The Art of Multiprocessor Programming", Herlihy & Shavit, 2012
- "Teaching Programming languages by experimental and adversarial thinking.", Pombrio, J., Krishnamurthi, S. & Fisler, K., SNAPL 2017
- https://github.com/shriram/mystery-languages
