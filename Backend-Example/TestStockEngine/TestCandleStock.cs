using Backend_Example.Logic.Classes;
using Backend_Example.Logic.Stocks;

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
            double interval = 1;

            // Act
            CandleItem[] values = CandleStock.GetCandleValues(mS, startX, endX, interval);

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
            CandleItem[] values = CandleStock.GetCandleValues(mS, startX, endX, interval);

            // Assert
            Assert.IsTrue(values.Any(v => v.Volume >= 0));
        }
    }
}
