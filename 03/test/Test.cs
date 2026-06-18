namespace test03;

[TestClass]
public sealed class Test
{
    static Set[] MakeSets(int size, Func<int, Set> make) {
        var sets = new Set[size];
        for (var i = 0; i < size; i++) {
            sets[i] = make(i);
        }
        return sets;
    }

    public void TestSet(Func<int, Set> f)
    {
        var n = 10;
        var s = MakeSets(n, f);
        var t = new Thread(() =>
        {
            for (var i = 1; i < n; i++) {
                s[0].Union(s[i]);
            }
        });
        t.Start();
        t.Join();
        foreach (var e in s) {
            Assert.AreEqual(s[0].Find(), e.Find());
        }
    }

    [TestMethod]
    public void TestSetA() => TestSet(i => new SetA(i));

    [TestMethod]
    public void TestSetB() => TestSet(i => new SetB(i));

    [TestMethod]
    public void TestSetC() => TestSet(i => new SetC(i));
}
