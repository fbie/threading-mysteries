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

public interface ISequenceBuilder {
    bool Next();

    void Print() {
        var ns = Sequence;
        for (var i = 0; i < ns.Count; i++) {
            Console.WriteLine($"{i} = {ns[i]}");
        }
    }

    List<int> Sequence { get; }
}

public class SequenceBuilderA(int _n) : ISequenceBuilder {
    private readonly List<int> _ns = new List<int>();
    private int _current = 0;

    public bool Next() {
        if (_current++ < _n) {
            _ns.Add(_current);
            return true;
        }
        return false;
    }

    public List<int> Sequence => _ns;
}

public class SequenceBuilderB(int _n) : ISequenceBuilder {
    private readonly List<int> _ns = new List<int>();
    private volatile int _current = 0;

    public bool Next() {
        if (_current++ < _n) {
            _ns.Add(_current);
            return true;
        }
        return false;
    }

    public List<int> Sequence => _ns;
}

public class SequenceBuilderC(int _n) : ISequenceBuilder {
    private readonly object _lock = new();
    private readonly List<int> _ns = new List<int>();
    private int _current = 0;

    public bool Next() {
        lock (_lock) {
            if (_current++ < _n) {
                _ns.Add(_current);
                return true;
            }
            return false;
        }
    }

    public List<int> Sequence => _ns;
}
