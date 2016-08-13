using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace AdvancedXML.Tests
{
    [TestFixture]
    public class XmlValidatorTests
    {
        public static IEnumerable<TestCaseData> ValidateCaseSource
        {
            get
            {
                yield return new TestCaseData("http://library.by/catalog", "../book.xsd", "./InvalidDate.xml", false);

                yield return new TestCaseData("http://library.by/catalog", "../book.xsd", "./InvalidGenre.xml", false);

                yield return new TestCaseData("http://library.by/catalog", "../book.xsd", "./InvalidId.xml", false);

                yield return new TestCaseData("http://library.by/catalog", "../book.xsd", "./InvalidISBN.xml", false);

                yield return new TestCaseData("http://library.by/catalog", "../book.xsd", "../books.xml", true);
            }
        }

        [OneTimeSetUp]
        public void Initialize()
        {
            Directory.SetCurrentDirectory(@"D:\Projects\.Net-Mentoring-D2-D3\Advanced XML\AdvancedXML.Tests");
        }

        [Test, TestCaseSource(nameof(ValidateCaseSource))]
        public void ValidateTests(string targetNamespace, string schemaPath, string filePath, bool expectedResult)
        {
            // Arrange
            Console.WriteLine(Directory.GetCurrentDirectory());
            var validator = new XmlValidator();
            IList<string> errors;

            // Act
            var result = validator.Validate(targetNamespace, schemaPath, filePath, out errors);

            // Assert
            Assert.AreEqual(expectedResult, result);
            Assert.AreEqual(result, !errors.Any());

            Console.WriteLine($"Errors: {string.Join(";", errors)}");
        }
    }
}
