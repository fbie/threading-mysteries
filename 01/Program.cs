using System.Threading;

if (args.Length != 2) {
    Console.WriteLine("Usage: 01.exe [A|B|C] <count>");
    return;
}
var n = int.Parse(args[1]);
ISequenceBuilder b = args[0] switch {
    "A" => new SequenceBuilderA(n),
    "B" => new SequenceBuilderB(n),
    "C" => new SequenceBuilderC(n),
    _ => throw new ArgumentException("Choose one of A, B or C")
};
void Run() {
    while (b.Next()) {}
}

var t0 = new Thread(Run);
var t1 = new Thread(Run);
t0.Start();
t1.Start();
t0.Join();
t1.Join();

b.Print();

// --- The mysterious implementation. ---

interface ISequenceBuilder {
    bool Next();
    void Print();
}

class SequenceBuilderA : ISequenceBuilder {
    private int _n;
    private List<int> _ns = new List<int>();
    private int _current = 0;

    public SequenceBuilderA(int n) {
        _n = n;
    }

    public bool Next() {
        if (_current++ < _n) {
            _ns.Add(_current);
            return true;
        }
        return false;
    }

    public void Print() {
        for (var i = 0; i < _ns.Count; i++) {
            Console.WriteLine($"{i} = {_ns[i]}");
        }
    }
}

class SequenceBuilderB : ISequenceBuilder {
    private int _n;
    private List<int> _ns = new List<int>();
    private volatile int _current = 0;

    public SequenceBuilderB(int n) {
        _n = n;
    }

    public bool Next() {
        if (_current++ < _n) {
            _ns.Add(_current);
            return true;
        }
        return false;
    }

    public void Print() {
        for (var i = 0; i < _ns.Count; i++) {
            Console.WriteLine($"{i} = {_ns[i]}");
        }
    }
}

class SequenceBuilderC : ISequenceBuilder {
    private int _n;
    private List<int> _ns = new List<int>();
    private int _current = 0;

    public SequenceBuilderC(int n) {
        _n = n;
    }

    public bool Next() {
        lock (_ns) {
            if (_current++ < _n) {
                _ns.Add(_current);
                return true;
            }
            return false;
        }
    }

    public void Print() {
        for (var i = 0; i < _ns.Count; i++) {
            Console.WriteLine($"{i} = {_ns[i]}");
        }
    }
}
