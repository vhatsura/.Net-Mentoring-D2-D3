using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;

namespace ExpressionsAndIQueryable.Tests
{
    public class Foo
    {
        public int i = 5;
        float f = 5.0f;
        public string s = "str";
    }

    public class Bar
    {
        public int i = 3;
        public string s = "s";
        public double f = 3.0;

        public override string ToString() => $"{{i = {i}, s = {s}, f = {f}}}";
    }

    [TestFixture]
    public class MapperTests
    {
        public static IEnumerable<TestCaseData> MapperTestData
        {
            get
            {
                yield return new TestCaseData(
                    new Foo(), new Bar(), (Func<Bar, bool>)(bar => bar.i == 5 && bar.s == "str" && bar.f == 3.0)
                    );

                yield return new TestCaseData(
                    new { f = 8.2 }, new Bar(), (Func<Bar, bool>)(bar => bar.f == 8.2)
                    );
            }
        }

        [Test, TestCaseSource(nameof(MapperTestData))]
        public void MapperTest<TSource, TDestination>(TSource source, TDestination destination, Func<TDestination, bool> checkAction)
        {
            // Arrange
            var mapGenerator = new MappingGenerator();
            var mapper = mapGenerator.Generate<TSource, TDestination>();

            // Act
            var result = mapper.Map(source);

            // Assert
            Assert.IsTrue(checkAction(result), $"Source: {source}, Result: {result}");
        }
    }
}
