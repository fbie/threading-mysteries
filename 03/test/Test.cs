namespace test02;

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

    [DataRow(10)]
    [TestMethod]
    public void TestSet(int n)
    {
        var s = MakeSets(n, i => new SetA(1)); // SetB, SetC...

        var t = new Thread(() =>
        {
            for (var i = 1; i < n; i++) {
                s[0].Union(s[i]);
            }
        });
        t.Start();
        t.Join();

        foreach (var e in s) {
            Assert.AreEqual(s[0].Parent, e.Parent);
        }

    }
}
