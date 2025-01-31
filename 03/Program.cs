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
var n = int.Parse(args[1]);
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
    var t = new Thread(() => {
        var lines = File.ReadAllLines(arg);
        barrier.SignalAndWait();
        foreach (var line in lines) {
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

abstract class Set {
    public int Id { get; }
    public int Size { get; set; }
    public Set Parent { get; set; }

    public Set(int id) {
        Id = id;
        Size = 1;
        Parent = this;
    }

    protected static Set Assign(Set x, Set y) {
        x = x.Find();
        y = y.Find();
        if (x == y) {
            return x;
        }
        if (y.Id < x.Id) {
            (x, y) = (y, x);
        }
        y.Parent = x;
        var size = x.Size + y.Size;
        x.Size = size;
        y.Size = size;
        return x;
    }

    public Set Find() {
        if (Parent != this) {
            Parent = Parent.Find();
        }
        return Parent;
    }

    public abstract Set Union(Set other);
}

class SetA : Set {
    public SetA(int id) : base(id) {}

    public override Set Union(Set other) {
        var x = this.Find();
        var y = other.Find();
        // Ensure that locking happens always in the same order for
        // all threads to avoid deadlock if another thread locks
        // first y and then x.
        if (y.Id < x.Id) {
            (x, y) = (y, x);
        }
        lock (x)
        lock (y) {
            return Assign(x, y);
        }
    }
}

class SetB : Set {
    public SetB(int id) : base(id) {}

    // No synchronization whatsoever - this may cause stack overflows
    // on spurious synchronizations.
    public override Set Union(Set other) => Assign(this, other);
}

class SetC : Set {
    public SetC(int id) : base(id) {}

    public override Set Union(Set other) {
        var x = this.Find();
        var y = other.Find();
        // No swapping, this can deadlock.
        lock (x)
        lock (y) {
            return Assign(x, y);
        }
    }
}
