using Logic.Stocks;

namespace TestStockEngine;

    [TestClass]
    public class TestCandleStock
    {
        [TestMethod]
        public void TestGetCandleValues_LowAboveHigh()
        {
            // Arrange
            const double mS = 20;
            const double startX = 2000;
            const double endX = 3000;
            const double interval = 5;

            // Act
            var values = CandleStock.CreateCandleValues(mS, startX, endX, interval);

            // Assert
            Assert.IsFalse(values.Any(v => v.Low > v.High));
        }

        [TestMethod]
        public void TestGetCandleValues_VolumeNegative()
        {
            // Arrange
            const double mS = 20;
            const double startX = 2000;
            const double endX = 3000;
            const double interval = 1;

            // Act
            var values = CandleStock.CreateCandleValues(mS, startX, endX, interval);

            // Assert
            Assert.IsTrue(values.Any(v => v.Volume >= 0));
        }
    }
