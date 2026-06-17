namespace test01;

[TestClass]
public sealed class Test
{

    public void TestSequenceBuilder(Func<int, ISequenceBuilder> f)
    {
        var n = 10;
        var seq = new int[n];
        for (var i = 0; i < n; i++) {
            seq[i] = i + 1;
        }

        var b = f(n);
        var t = new Thread(() => { while (b.Next()) { } });
        t.Start();
        t.Join();
        CollectionAssert.AreEqual(seq, b.Sequence);
    }

    [TestMethod]
    public void TestSequenceBuilderA() => TestSequenceBuilder(n => new SequenceBuilderA(n));

    [TestMethod]
    public void TestSequenceBuilderB() => TestSequenceBuilder(n => new SequenceBuilderB(n));

    [TestMethod]
    public void TestSequenceBuilderC() => TestSequenceBuilder(n => new SequenceBuilderC(n));
}
