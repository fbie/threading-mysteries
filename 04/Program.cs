if (args.Length != 3) {
    Console.WriteLine("Usage: 03 [A|B|C] <number-of-threads> <number-of-increments>");
    return;
}
ICounter counter = args[0] switch {
    "A" => new InterlockedCounter(),
    "B" => new PaddedCounter(),
    "C" => new LockingCounter(),
    _ => throw new ArgumentException("Choose one of A, B or C")
};
var threads = int.Parse(args[1]);
var increments = int.Parse(args[2]);
var barrier = new Barrier(threads + 1);
for (var i = 0; i < threads; i++) {
    Task.Run(() => {
        barrier.SignalAndWait();
        for (var j = 0; j < increments; j++) {
            counter.Increment();
        }
        barrier.SignalAndWait();
    });
}
var sw = new System.Diagnostics.Stopwatch();
sw.Start();
barrier.SignalAndWait();
barrier.SignalAndWait();
sw.Stop();
Console.WriteLine($"Counter value: {counter.Value}\nTook {sw.ElapsedMilliseconds}ms");

interface ICounter {
    long Value { get; }
    void Add(long value);
    void Increment();
}

class LockingCounter : ICounter {
    private readonly object _lock = new object();
    private long _value = 0L;
    public long Value { get { lock (_lock) { return _value; }}}
    public void Add(long value) { lock (_lock) { _value += value; }}
    public void Increment() => Add(1L);
}

class InterlockedCounter : ICounter {
    private long _value = 0L;
    public long Value => Interlocked.Read(ref _value);
    public void Add(long value) => Interlocked.Add(ref _value, value);
    public void Increment() => Add(1L);
}

class PaddedCounter : ICounter {
    private const int CACHE_LINE_SIZE = 64 * 8;
    private const int STRIPES = 31;
    private const int PADDING = CACHE_LINE_SIZE / sizeof(long);
    private readonly long[] _values = new long[STRIPES * PADDING];

    public long Value {
        get {
            var sum = 0L;
            for (var i = 0; i < _values.Length; i+= PADDING) {
                sum += Interlocked.Read(ref _values[i]);
            }
            return sum;
        }
    }

    public void Add(long value)
        => Interlocked.Add(ref _values[Thread.CurrentThread.GetHashCode() % STRIPES * PADDING], value);

    public void Increment() => Add(1L);
}
