namespace test04;

[TestClass]
public sealed class Test
{
    [DynamicData(nameof(Counters))]
    [TestMethod]
    public void TestCounter(ICounter c)
    {
        var n = 1;
        var t = new Thread(() =>
        {
            for (var i = 0; i < n; i++) {
                c.Increment();
            }
        });
        t.Start();
        while (c.Value < n) ; // Spin.
        t.Join();
        Assert.AreEqual(n, c.Value); // What else can be observed instead of results?
    }

    public static IEnumerable<ICounter> Counters = [new CounterA(), new CounterB(), new CounterC()];
}
