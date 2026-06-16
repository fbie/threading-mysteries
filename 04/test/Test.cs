namespace test04;

[TestClass]
public sealed class Test
{
    [DataRow(10000)]
    [TestMethod]
    public void TestCounter(int n)
    {
        var c = ICounter.MakeA(); // MakeB, MakeC...
        var t = new Thread(() =>
        {
            for (var i = 0; i < n; i++) {
                c.Increment();
            }
        });
        t.Start();
        var s = new Thread(() =>
        {
            while (c.Value < n);
        });
        s.Start();
        t.Join();
        s.Join();
        Assert.AreEqual(n, c.Value); // What else can be observed instead of results?
    }
}
