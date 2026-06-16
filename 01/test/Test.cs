namespace test01;

[TestClass]
public sealed class Test
{
    [DataRow(10)]
    [TestMethod]
    public void TestSequence(int n)
    {
        var seq = new int[n];
        for (var i = 0; i < n; i++) {
            seq[i] = i + 1;
        }

        var b = new SequenceBuilderA(n); // SequenceBuilderB, SequenceBuilderC ...
        var t = new Thread(() => { while (b.Next()) { } });
        t.Start();
        t.Join();
        CollectionAssert.AreEqual(seq, b.Sequence);
    }
}
