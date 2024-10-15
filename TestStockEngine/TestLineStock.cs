using Backend_Example.Logic.Stocks;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace TestStockEngine
{
    [TestClass]
    public class TestLineStock
    {
        [TestMethod]
        public void TestLineValues()
        {
            // Arrange
            LineStock linestock = new LineStock();
            double mS = 20;
            double startX = 2000;
            double endX = 3000;
            double interval = 1;

            // Act
            double[] values = linestock.GetValues(mS, startX, endX, interval);

            // Assert
            Console.WriteLine(values.Length);
            Assert.IsTrue(values.Length == ((endX - startX) / interval) + 1);
        }
    }
}
