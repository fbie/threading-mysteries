# .NET Threading Mysteries #
#### Florian Biermann ####
##### `flbm@simcorp.com` #####

---

# What #

- .NET uses "threads" to structure concurrent computations.
- A thread is an independent unit of execution.
- Lots of benefits: responsive GUI, higher throughput, more modular code.
- Lots of problems: race conditions, deadlocks, starvation, and so on.
- Problems are real: we find threading bugs in the IMS code base.

---

# Concurrent Programming #

Writing correct threading code is:

- "easy": if you follow the basic patterns; but
- "hard": if you want to have really high performance, throughput, etc.

Analyzing threading bugs is:

- "harder": occur spuriously, "Heisenbugs", debugger interferes.

---

# This Session #

This session is structured through "threading mysteries". Each mystery:

- does *something* with .NET threads;
- takes some kind of input (numbers, files, ...); and
- comes in up to three variants, `A`, `B` and `C`.

Goals:

1. experience some unexpected behavior;
2. try to understand the causes; and
3. connect the two.

---

# Your task #

In small groups (2-3), your task is to:

1. compile the code;
2. **DO NOT** look at the sources; instead
3. run each variant, use different arguments.

Observe and interpret!

<center>https://simhub.simcorp.com/FLBM/threading-mysteries</center>

---

# Solving the Mysteries #

In the code we display:

- are modifiers like `readonly` omitted unless important; and
- are constructors omitted.

---

# Mystery 1 #

```csharp
interface ISequenceBuilder {
    bool Next();
}
ISqeuenceBuilder b = ...;
var t0 = new Thread(() => { while (b.Next()); });
var t1 = new Thread(() => { while (b.Next()); });
t0.Start();
t1.Start();
t0.Join();
t1.Join();
```

---

# Mystery 1: A #

```csharp
class SequenceBuilderA : ISequenceBuilder {
    int _n;
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
class SequenceBuilderB : ISequenceBuilder {
    int _n;
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
class SequenceBuilderC : ISequenceBuilder {
    int _n;
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
    ConcurrentDictionary<char, int> _chars
        = new();

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
class LockingCounter : ICounter {
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
class InterlockedCounter : ICounter {
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
class PaddedCounter : ICounter {
    int S = 31;
    int P = 64 * 8 / sizeof(long);
    long[] _vs = new long[S * P];

    public long Value { get {
        var s = 0L;
        for (var i = 0; i < _vs.Length; i += P)
            s += Interlocked.Read(ref _vs[i]);
        return s;
    }}

    public void Add(long value) {
        var h = CurrentThread.GetHashCode();
        var i = h % S * P;
        Interlocked.Add(ref _vs[i], value);
    }
```
---


# Mystery 5 #

```csharp
interface IFactorizer {
    void Run();
}
IFactorizer f = ...;
f.Run();
```

---

# Mystery 5: A #

```csharp
class ThreadFactorizer : IFactorizer {
    ConcurrentBag<int> _ns;
    ConcurrentDictionary<int, IList<int>> _fs;

    public void Run() {
        var ts = new List<Thread>(_ns.Count);
        while (_ns.TryTake(out var n)) {
            var t = new Thread(() => {
                _fs[n] = Util.Factorize(n);
                });
            t.Start();
            ts.Add(t);
        }
        foreach (var t in ts)
        t.Join();
        }}
```

---

# Mystery 5: B #

```csharp
class TaskFactorizer : IFactorizer {
    ConcurrentBag<int> _ns;
    ConcurrentDictionary<int, IList<int>> _fs;

    public void Run() {
        var ts = new List<Task>(_ns.Count);
        while (_ns.TryTake(out var n))
            ts.Add(Task.Run(() => {
                _fs[n] = Util.Factorize(n);
                }));
        foreach (var t in ts)
            t.Wait();
    }}
```

---

# Mystery 5: C #

```csharp
class ParallelForFactorizer : IFactorizer {
    ConcurrentBag<int> _ns;
    ConcurrentDictionary<int, IList<int>> _fs;

    public void Run() {
        Parallel.ForEach(_ns, n => {
            _fs[n] = Util.Factorize(n);
        });
    }
}
```

---

# Mystery 6 #

```csharp
interface IRunner {
    void Run(Barrier barrier);
}
IRunner runner = ...;
var barrier = new Barrier(n + 1);
for (var i = 0; i < n; i++)
    runner.Run(barrier);
barrier.SignalAndWait();

```
---

# Mystery 6: A #

```csharp
class ThreadRunner : IRunner {
    public void Run(Barrier barrier) {
        new Thread(() => {
            barrier.SignalAndWait();
        })
        .Start();
    }}
```

---

# Mystery 6: B #

```csharp
class TaskRunner : IRunner {
    public void Run(Barrier barrier) {
        Task.Run(() => {
            barrier.SignalAndWait();
        });
    }}
```

---

# Take-Aways #

<p style="text-align: center;">Shared mutable state is the root of all evil.</p>

- Purely functional program guards against this; but
- there is still state _somewhere_, so beware!
- Different models of concurrency: message passing, actors etc.


# Recommended Reading #

- "Java Concurrency in Practice", Brian Goetz (with Tim Peierls et al., 2006)
- "C# Precisely", Peter Sestoft (2nd edition, 2012)
- "The Art of Multiprocessor Programming", Herlihy & Shavit, 2012
