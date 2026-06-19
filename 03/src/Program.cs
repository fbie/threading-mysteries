#pragma warning disable CS0728

Set[] MakeSets(int size, Func<int, Set> make) {
    var sets = new Set[size];
    for (var i = 0; i < size; i++) {
        sets[i] = make(i);
    }
    return sets;
}
if (args.Length < 2) {
    Console.WriteLine("Usage: 03.exe [A|B|C] <number-of-sets> <path to file> <path to file>...");
    return;
}
if (!int.TryParse(args[1], out var n) || n < 1) {
    throw new ArgumentException($"Expected a non-negative number, got {args[1]}");
}
Set[] sets = args[0] switch {
    "A" => MakeSets(n, i => new SetA(i)),
    "B" => MakeSets(n, i => new SetB(i)),
    "C" => MakeSets(n, i => new SetC(i)),
    _ => throw new ArgumentException("Choose one of A, B or C")
};

var threads = new List<Thread>();
var barrier = new Barrier(args.Length - 1); // Num threads + main thread.
for (var i = 2; i < args.Length; i++) {
    var arg = args[i];
    var lines = File.ReadAllLines(arg);
        if (!int.TryParse(lines.First(), out var m)) {
            throw new ArgumentException("Malformed input file: first line should define number of sets");
        }
        if (n < m) {
            throw new ArgumentException($"Input file requires {m} sets but only {n} sets initialized");
        }
    var t = new Thread(() => {
        barrier.SignalAndWait();
        foreach (var line in lines.Skip(1)) {
            var split = line.Split(" ");
            var x = int.Parse(split[0]);
            var y = int.Parse(split[1]);
            sets[x].Union(sets[y]);
        }
    });
    // Don't start threads just yet, we want
    // as much parallel execution as possible.
    threads.Add(t);
}
foreach (var t in threads) {
    t.Start();
}
barrier.SignalAndWait();
foreach (var t in threads) {
    t.Join();
}
Set largest = sets[0];
foreach (var set in sets) {
    if (largest.Size < set.Size) {
        largest = set;
    }
}
Console.WriteLine($"Largest set has {largest.Size} elements.");

// --- The mysterious implementation. ---

public abstract class Set {
    public int Id { get; }
    public int Size { get; set; }
    private Set _parent;

    public Set(int id) {
        Id = id;
        Size = 1;
        _parent = this;
    }

    protected static Set Join(Set x, Set y) {
        x = x.Find();
        y = y.Find();
        if (x == y) {
            return x;
        }
        if (x.Size < y.Size) {
            (x, y) = (y, x);
        }
        y._parent = x;
        var size = x.Size + y.Size;
        x.Size = size;
        y.Size = size;
        return x;
    }

    public Set Find() {
        // Normally, we would implement this iteratively.  Using
        // recursion, we can provoke a stack overflow, which is more
        // easily distinguishable from a deadlock than a livelock.
        //
        // Also, note that this does not synchronize, an attempt at
        // "optimistic" concurrency.
        if (_parent != this) {
            _parent = _parent.Find();
        }
        return _parent;
    }

    public abstract Set Union(Set other);
}

public class SetA(int id) : Set(id) {

    public override Set Union(Set other) {
        var x = this.Find();
        var y = other.Find();
        // Ensure that locking happens always in the same order for
        // all threads to avoid deadlock if another thread locks
        // first y and then x.
        if (y.Id < x.Id) {
            (x, y) = (y, x);
        }
        lock (x){
            lock (y) {
                return Join(x, y);
            }
        }
    }
}

public class SetB(int id) : Set(id) {
    // No synchronization whatsoever - this may cause stack overflows
    // on spurious synchronizations.
    public override Set Union(Set other) => Join(this, other);
}

public class SetC(int id) : Set(id) {

    public override Set Union(Set other) {
        var x = this.Find();
        var y = other.Find();
        // No swapping, this can deadlock.
        lock (x)
        {
            lock (y)
            {
                return Join(x, y);
            }
        }
    }
}
