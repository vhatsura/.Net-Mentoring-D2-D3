using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;

namespace ExpressionsAndIQueryable.Tests
{
    [TestFixture]
    public class CustomExpressionTransformerTests
    {
        #region VisitBinary Tests

        public static IEnumerable<TestCaseData> VisitBinaryTestData
        {
            get
            {
                yield return new TestCaseData(
                    (Expression<Func<int, int>>)(a => a + 1), Expression.Lambda(Expression.Increment(Expression.Parameter(typeof(int), "a")), Expression.Parameter(typeof(int), "a")));

                yield return new TestCaseData(
                    (Expression<Func<int, int>>)(a => 1 + a), Expression.Lambda(Expression.Increment(Expression.Parameter(typeof(int), "a")), Expression.Parameter(typeof(int), "a")));

                yield return new TestCaseData(
                    (Expression<Func<int, int>>)(a => a - 1), Expression.Lambda(Expression.Decrement(Expression.Parameter(typeof(int), "a")), Expression.Parameter(typeof(int), "a")));

                yield return new TestCaseData(
                    (Expression<Func<int, int>>)(a => 1 - a), (Expression<Func<int, int>>)(a => 1 - a));

                yield return new TestCaseData(
                    (Expression<Func<int, int>>)(a => a + 2), (Expression<Func<int, int>>)(a => a + 2));

                yield return new TestCaseData(
                    (Expression<Func<int, int>>)(a => 2 + a), (Expression<Func<int, int>>)(a => 2 + a));

                yield return new TestCaseData(
                    (Expression<Func<int, int>>)(a => a - 2), (Expression<Func<int, int>>)(a => a - 2));

                yield return new TestCaseData(
                    (Expression<Func<int, double>>)(a => a + 1.0), (Expression<Func<int, double>>)(a => a + 1.0));

                yield return new TestCaseData(
                    (Expression<Func<int, double>>)(a => a - 1.0), (Expression<Func<int, double>>)(a => a - 1.0));

            }
        }

        [Test, TestCaseSource(nameof(VisitBinaryTestData))]
        public void VisitBinaryTest(Expression sourceExpression, Expression expectedExpression)
        {
            // Arrange
            var transformer = new CustomExpressionTransformer();

            // Act
            var actualExpression = transformer.Visit(sourceExpression);

            // Assert
            Assert.IsTrue(ExpressionsComparer.ExpressionEqual(actualExpression, expectedExpression), $"Expected: {expectedExpression}\r\nActual: {actualExpression}");
        }
        
        #endregion
    }
}
