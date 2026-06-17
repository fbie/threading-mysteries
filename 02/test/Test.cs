namespace test02;

[TestClass]
public sealed class Test
{
    [DynamicData(nameof(Histograms))]
    [TestMethod]
    public void TestHistogram(IHistogram h)
    {
        var content = File.ReadAllText("../../../../words.txt"); // Or something else?
        var t = new Thread(() =>
        {
            foreach (var c in content)
            {
                if (char.IsLetterOrDigit(c))
                {
                    h.Add(c);
                }
            }
        });
        t.Start();
        t.Join();

        Assert.AreNotEqual(0, h.Chars.Count);
    }

    public static IEnumerable<IHistogram> Histograms = [new HistogramA(), new HistogramB(), new HistogramC()];
}
