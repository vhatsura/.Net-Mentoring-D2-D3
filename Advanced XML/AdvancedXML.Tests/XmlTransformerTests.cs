using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using NUnit.Framework;

namespace AdvancedXML.Tests
{
    [TestFixture]
    public class XmlTransformerTests
    {
        [OneTimeSetUp]
        public void Initialize()
        {
            Directory.SetCurrentDirectory(@"D:\Projects\.Net-Mentoring-D2-D3\Advanced XML\AdvancedXML.Tests");
        }

        [Test]
        public void ValidateXmlToAtomTests()
        {
            // Arrange
            Console.WriteLine(Directory.GetCurrentDirectory());
            var validator = new XmlTransformer();
            var memoryStream = new MemoryStream();

            // Act
            validator.Transform(
                "../xmlToAtom.xslt",
                new FileStream("../books.xml", FileMode.Open, FileAccess.Read),
                memoryStream);

            memoryStream.Position = 0;

            // Assert
            var document = XDocument.Load(memoryStream);
        }

        [Test]
        public void ValidateXmlToHtmlTests()
        {
            // Arrange
            Console.WriteLine(Directory.GetCurrentDirectory());
            var validator = new XmlTransformer();
            var memoryStream = new MemoryStream();

            // Act
            validator.Transform(
                "../xmlToHTMLReport.xslt",
                new FileStream("../books.xml", FileMode.Open, FileAccess.Read),
                memoryStream,
                new Dictionary<string, object>() { { "Date", DateTime.Now.ToString("f") } }
            );

            memoryStream.Position = 0;

            // Assert
            var document = XDocument.Load(memoryStream);
        }
    }
}
