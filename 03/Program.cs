#pragma warning disable CS0728

Set[] MakeSets(int size, Func<int, Set> make) {
    var sets = new Set[size];
    for (var i = 0; i < size; i++) {
        sets[i] = make(i);
    }
    return sets;
}

if (args.Length < 2) {
    Console.WriteLine("Usage: 02 [A|B|C] <number-of-sets> <path to file> <path to file>...");
}
var n = int.Parse(args[1]);
Set[] sets = args[0] switch {
    "A" => MakeSets(n, i => new SetA(i)),
    "B" => MakeSets(n, i => new SetB(i)),
    "C" => MakeSets(n, i => new SetC(i)),
    _ => throw new ArgumentException("Choose one of A, B or C")
};

var threads = new List<Thread>();
for (var i = 2; i < args.Length; i++) {
    var arg = args[i];
    var t = new Thread(() => {
        foreach (var line in File.ReadLines(arg)) {
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
foreach (var t in threads) {
    t.Join();
}
foreach (var set in sets) {
    Console.WriteLine($"{set.Id} -> {set.Parent.Id}");
}

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
        if (x == y) {
            return x;
        }
        if (x.Size < y.Size) {
            (x, y) = (y, x);
        }
        y.Parent = x;
        x.Size += y.Size;
        return x;
    }

    public abstract Set Find();

    public abstract Set Union(Set other);
}

class SetB : Set {
    public SetB(int id) : base(id) {}

    public override Set Find() {
        if (Parent != this) {
            Parent = Parent.Find();
        }
        return Parent;
    }

    public override Set Union(Set other) {
        var x = this.Find();
        var y = other.Find();
        return Assign(x, y);
    }
}

class SetA : Set {
    public SetA(int id) : base(id) {}

    public override Set Find() {
        if (Parent != this) {
            Parent = Parent.Find();
        }
        return Parent;
    }

    public override Set Union(Set other) {
        Set x = this;
        var y = other;
        // Ensure that locking happens always in the same order for
        // all threads.
        if (y.Id < x.Id) {
            (x, y) = (y, x);
        }
        lock (x)
        lock (y) {
            x = x.Find();
            y = y.Find();
            return Assign(x, y);
        }
    }
}

class SetC : Set {
    public SetC(int id) : base(id) {}

    public override Set Find() {
        if (Parent != this) {
            Parent = Parent.Find();
        }
        return Parent;
    }

    public override Set Union(Set other) {
        Set x = this;
        var y = other;
        // No swapping, this can deadlock.
        lock (x)
        lock (y) {
            x = x.Find();
            y = y.Find();
            return Assign(x, y);
        }
    }
}
