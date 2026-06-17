using System.Runtime.InteropServices;

#pragma warning disable CS4014
if (args.Length != 3)
{
    Console.WriteLine("Usage: 04.exe [A|B|C] <number-of-threads> <number-of-increments>");
    return;
}
ICounter counter = args[0] switch
{
    "A" => new CounterA(),
    "B" => new CounterB(),
    "C" => new CounterC(),
    _ => throw new ArgumentException("Choose one of A, B or C")
};
var threads = int.Parse(args[1]);
var increments = long.Parse(args[2]);
var barrier = new Barrier(threads + 1);
for (var i = 0; i < threads; i++)
{
    new Thread(() =>
    {
        barrier.SignalAndWait();
        for (var j = 0; j < increments; j++)
        {
            counter.Increment();
        }
    }).Start();
}
// var sw = new System.Diagnostics.Stopwatch();
// sw.Start();
barrier.SignalAndWait();
while (counter.Value < threads * increments) { }
// sw.Stop();
Console.WriteLine($"Counter value: {counter.Value}");
// Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms");

public interface ICounter
{
    long Value { get; }
    void Add(long value);
    void Increment();
}

public class CounterA : ICounter
{
    private long _value = 0L;
    public long Value => Interlocked.Read(ref _value);
    public void Add(long value) => Interlocked.Add(ref _value, value);
    public void Increment() => Add(1L);
}

public class CounterC : ICounter
{
    private readonly object _lock = new object();
    private long _value = 0L;
    public long Value { get { lock (_lock) { return _value; } } }
    public void Add(long value) { lock (_lock) { _value += value; } }
    public void Increment() => Add(1L);
}

public class CounterB : ICounter
{
    [StructLayout(LayoutKind.Explicit, Size=64)]
    struct PaddedLong {
        [FieldOffset(0)] public long Value;
    }

    private const int STRIPES = 32;
    private readonly PaddedLong[] _values = new PaddedLong[STRIPES];

    public long Value
        => _values.Sum(p => Interlocked.Read(ref p.Value));

    public void Add(long value)
        => Interlocked.Add(ref _values[Thread.CurrentThread.ManagedThreadId % STRIPES].Value, value);

    public void Increment() => Add(1L);
}
