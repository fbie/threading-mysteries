namespace test05;
using System.Collections.Concurrent;

[TestClass]
public sealed class Test
{
    [DataRow(100)]
    [TestMethod]
    public void TestFactorizer(int n)
    {
        var inputs = new ConcurrentBag<int>();
        for (var i = 100; i < n + 100; i++) {
            inputs.Add(i);
        }
        var factors = new ConcurrentDictionary<int, IReadOnlyList<int>>();
        IFactorizer.MakeA(inputs, factors).Run(); // MakeB, MakeC...
        Assert.AreNotEqual(0, factors.Count); // What else can be observed instead of results?
    }
}
