namespace test06;
using System.Threading;

[TestClass]
public sealed class Test
{

    [DynamicData(nameof(Waiters))]
    [Timeout(5000)]
    [TestMethod]
    public void TestWaiter(IWaiter waiter)
    {
        var n = 1;
        var barrier = new Barrier(n + 1);
        for (var i = 0; i < n; i++) {
            waiter.Wait(barrier);
        }
        barrier.SignalAndWait();
    }

    public static IEnumerable<IWaiter> Waiters = [new WaiterA(), new WaiterB()];
}
