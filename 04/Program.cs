using System.Runtime.InteropServices;

#pragma warning disable CS4014
if (args.Length != 3)
{
    Console.WriteLine("Usage: 04.exe [A|B|C] <number-of-threads> <number-of-increments>");
    return;
}
ICounter counter = args[0] switch
{
    "A" => new InterlockedCounter(),
    "B" => new PaddedCounter(),
    "C" => new LockingCounter(),
    _ => throw new ArgumentException("Choose one of A, B or C")
};
var threads = int.Parse(args[1]);
var increments = int.Parse(args[2]);
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
        barrier.SignalAndWait();
    }).Start();
}
var sw = new System.Diagnostics.Stopwatch();
sw.Start();
barrier.SignalAndWait();
barrier.SignalAndWait();
sw.Stop();
Console.WriteLine($"Counter value: {counter.Value}\nTook {sw.ElapsedMilliseconds}ms");

interface ICounter
{
    long Value { get; }
    void Add(long value);
    void Increment();
}

class LockingCounter : ICounter
{
    private readonly object _lock = new object();
    private long _value = 0L;
    public long Value { get { lock (_lock) { return _value; } } }
    public void Add(long value) { lock (_lock) { _value += value; } }
    public void Increment() => Add(1L);
}

class InterlockedCounter : ICounter
{
    private long _value = 0L;
    public long Value => Interlocked.Read(ref _value);
    public void Add(long value) => Interlocked.Add(ref _value, value);
    public void Increment() => Add(1L);
}

class PaddedCounter : ICounter
{
    [StructLayout(LayoutKind.Sequential, Size=64)]
    struct PaddedLong {
        public long Value; // 8 bytes payload +
        private byte        // 56 bytes padding = 64 bytes.
            _p01, _p02, _p03, _p04, _p05, _p06, _p07, _p08,
            _p09, _p10, _p11, _p12, _p13, _p14, _p15, _p16,
            _p17, _p18, _p19, _p20, _p21, _p22, _p23, _p24,
            _p25, _p26, _p27, _p28, _p29, _p30, _p31, _p32,
            _p33, _p34, _p35, _p36, _p37, _p38, _p39, _p40,
            _p41, _p42, _p43, _p44, _p45, _p46, _p47, _p48,
            _p49, _p50, _p51, _p52, _p53, _p54, _p55, _p56;
    }

    private const int STRIPES = 32;
    private readonly PaddedLong[] _values = new PaddedLong[STRIPES];

    public long Value
    {
        get
        {
            var sum = 0L;
            for (var i = 0; i < _values.Length; i++)
            {
                sum += Interlocked.Read(ref _values[i].Value);
            }
            return sum;
        }
    }

    public void Add(long value)
        => Interlocked.Add(ref _values[Thread.CurrentThread.ManagedThreadId % STRIPES].Value, value);

    public void Increment() => Add(1L);
}
