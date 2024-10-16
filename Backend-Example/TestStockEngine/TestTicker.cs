using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend_Example.Logic.Classes;

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
            // Arrange
            Ticker ticker = new Ticker();

            // Act
            int result = ticker.ConvertWordToNumber(tickername);

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
