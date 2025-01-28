if (args.Length != 2)
{
    Console.WriteLine("Usage: 05.exe [A|B] <number-of-threads>");
    return;
}
var n = int.Parse(args[1]);
var barrier = new Barrier(n + 1);
IRunner runner = args[0] switch
{
    "A" => new ThreadRunner(),
    "B" => new TaskRunner(),
    _ => throw new ArgumentException("Choose one of A or B")
};
for (var i = 0; i < n; i++) {
    runner.Run(barrier);
}
barrier.SignalAndWait();

interface IRunner
{
    void Run(Barrier barrier);
}

class ThreadRunner : IRunner {
    public void Run(Barrier barrier) => new Thread(() => barrier.SignalAndWait()).Start();
}

class TaskRunner : IRunner {
    public void Run(Barrier barrier) => Task.Run(() => barrier.SignalAndWait());
}
