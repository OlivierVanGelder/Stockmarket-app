﻿using Logic.Models;

namespace TestStockEngine;

[TestClass]
public class TestFormula
{
    [TestMethod]
    [DataRow(0, 0)]
    [DataRow(0, 50)]
    [DataRow(100000000, 0)]
    [DataRow(100000000, 100)]
    [DataRow(-50000, -100)]
    public void TestFormulaOutput(double x, int mS)
    {
        // Arrange

        // Act
        var result = Formula.CalculateFormula(x, mS);

        // Assert
        Assert.IsNotNull(result);
    }
}
