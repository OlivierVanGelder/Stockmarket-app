using Logic.Models;
using Logic.Stocks;

namespace TestStockEngine
{
    [TestClass]
    public class TestCandleStock
    {
        [TestMethod]
        public void TestGetCandleValues_LowAboveHigh()
        {
            // Arrange
            CandleStock candleStock = new CandleStock();
            double mS = 20;
            double startX = 2000;
            double endX = 3000;
            double interval = 5;

            // Act
            CandleItem[] values = CandleStock.CreateCandleValues(mS, startX, endX, interval);

            // Assert
            Assert.IsFalse(values.Any(v => v.Low > v.High));
        }

        [TestMethod]
        public void TestGetCandleValues_VolumeNegative()
        {
            // Arrange
            CandleStock candleStock = new CandleStock();
            double mS = 20;
            double startX = 2000;
            double endX = 3000;
            double interval = 1;

            // Act
            CandleItem[] values = CandleStock.CreateCandleValues(mS, startX, endX, interval);

            // Assert
            Assert.IsTrue(values.Any(v => v.Volume >= 0));
        }
    }
}
