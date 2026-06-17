namespace test05;
using System.Collections.Concurrent;

[TestClass]
public sealed class Test
{
    [DynamicData(nameof(Factorizers))]
    [TestMethod]
    public void TestFactorizer(IFactorizer factorizer)
    {
        var n = 100;
        var inputs = new ConcurrentBag<int>();
        for (var i = 100; i < n + 100; i++) {
            inputs.Add(i);
        }
        var factors = factorizer.Run(inputs);
        Assert.AreNotEqual(0, factors.Count); // What else can be observed instead of results?
    }

    public static IEnumerable<IFactorizer> Factorizers = [new FactorizerA(), new FactorizerB(), new FactorizerC()];
}
