using NUnit.Framework;

[TestFixture]
public class CalculatorTests
{
    [TestCase("2 + 3", ExpectedResult = 5)]
    [TestCase("10 - 5", ExpectedResult = 5)]
    [TestCase("3 * 4", ExpectedResult = 12)]
    [TestCase("20 / 5", ExpectedResult = 4)]
    [TestCase("7 % 3", ExpectedResult = 1)]
    [TestCase("2 ^ 3", ExpectedResult = 8)]
    [TestCase("5 + 3 * 2", ExpectedResult = 11)]
    [TestCase("(5 + 3) * 2", ExpectedResult = 16)]
    [TestCase("-4 + 6", ExpectedResult = 2)]
    [TestCase("3 - -2", ExpectedResult = 5)]
    [TestCase("4 * -2", ExpectedResult = -8)]
    [TestCase("10 / -5", ExpectedResult = -2)]
    [TestCase("-7 % 3", ExpectedResult = -1)]
    public double CalculateExpression_ValidInput_ReturnsExpectedResult(string expression)
    {
        return Calculator.EvaluateExpression(expression);
    }

    [TestCase("")]
    [TestCase("   ")]
    [TestCase("abc")]
    [TestCase("2 +")]
    [TestCase("5 3")]
    [TestCase("(2 + 3")]
    [TestCase("2 + 3)")]
    [TestCase("2 3 +")]
    [TestCase("2 / 0")]
    public void CalculateExpression_InvalidInput_ThrowsException(string expression)
    {
        Assert.Throws<ArgumentException>(() => Calculator.EvaluateExpression(expression));
    }
}