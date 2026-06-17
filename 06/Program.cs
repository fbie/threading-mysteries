if (args.Length != 2)
{
    Console.WriteLine("Usage: 05.exe [A|B] <number-of-threads>");
    return;
}
var n = int.Parse(args[1]);
var barrier = new Barrier(n + 1);
IWaiter waiter = args[0] switch
{
    "A" => new ThreadWaiter(),
    "B" => new TaskWaiter(),
    _ => throw new ArgumentException("Choose one of A or B")
};
for (var i = 0; i < n; i++) {
    runner.wait(barrier);
}
barrier.SignalAndWait();

interface IWaiter
{
    void Wait(Barrier barrier);
}

class ThreadWaiter : IWaiter {
    public void Wait(Barrier barrier) => new Thread(() => barrier.SignalAndWait()).Start();
}

class TaskWaiter : IWaiter {
    public void Wait(Barrier barrier) => Task.Run(() => barrier.SignalAndWait());
}
