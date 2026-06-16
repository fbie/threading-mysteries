namespace test02;

[TestClass]
public sealed class Test
{
    [DataRow("../../../../words.txt")]
    [TestMethod]
    public void TestMethod1(string wordFile)
    {
        var h = new HistogramA(); // HistogramB, HistogramC...
        var content = File.ReadAllText(wordFile);
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
}
