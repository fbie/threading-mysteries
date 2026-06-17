namespace test06;
using System.Threading;

[TestClass]
public sealed class Test
{
    [Timeout(1000)]
    [DataRow(1)]
    [TestMethod]
    public void TestWaiter(int n)
    {
        var barrier = new Barrier(n + 1);
        var waiter = IWaiter.MakeA(); // MakeB...
        for (var i = 0; i < n; i++) {
            waiter.Wait(barrier);
        }
        barrier.SignalAndWait();
    }
}
