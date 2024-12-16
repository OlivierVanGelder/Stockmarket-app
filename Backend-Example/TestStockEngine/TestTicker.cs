using Logic.Models;

namespace TestStockEngine;

[TestClass]
public class TestTicker
{
    [TestMethod]
    [DataRow("PBKS", 44)]
    [DataRow("ABAB", 2)]
    [DataRow("DV", 24)]
    [DataRow("", 0)]
    [DataRow("ZZZZ", 100)]
    public void TestConvertWordToNumber(string tickerName, int expected)
    {
        // Act
        var result = Converter.ConvertWordToNumber(tickerName);

        // Assert
        Assert.AreEqual(expected, result);
    }
}
