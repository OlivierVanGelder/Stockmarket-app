using Logic.Models;

namespace TestStockEngine
{
    [TestClass]
    public class TestTicker
    {
        [TestMethod]
        [DataRow("PBKS", 44)]
        [DataRow("ABAB", 2)]
        [DataRow("DV", 24)]
        [DataRow("", 0)]
        [DataRow("ZZZZ", 100)]
        public void TestConvertWordToNumber(string tickername, int expected)
        {
            // Act
            int result = Converter.ConvertWordToNumber(tickername);

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
