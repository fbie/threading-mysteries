if (args.Length != 2)
{
    Console.WriteLine("Usage: 06.exe [A|B] <number-of-threads>");
    return;
}
var n = int.Parse(args[1]);
var barrier = new Barrier(n + 1);
IWaiter waiter = args[0] switch
{
    "A" => new WaiterA(),
    "B" => new WaiterB(),
    _ => throw new ArgumentException("Choose one of A or B")
};
for (var i = 0; i < n; i++) {
    waiter.Wait(barrier);
}
barrier.SignalAndWait();

public interface IWaiter
{
    void Wait(Barrier barrier);
}

public class WaiterA : IWaiter {
    public void Wait(Barrier barrier) => new Thread(() => barrier.SignalAndWait()).Start();
}

public class WaiterB : IWaiter {
    public void Wait(Barrier barrier) => Task.Run(() => barrier.SignalAndWait());
}
