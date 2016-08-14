using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace AdvancedXML
{
    public sealed class XmlTransformer
    {
        public void Transform(string xsltPath, Stream input, Stream output)
        {
            if (string.IsNullOrWhiteSpace(xsltPath))
                throw new ArgumentException(nameof(xsltPath));

            if (input == null)
                throw new ArgumentException(nameof(input));

            if (output == null)
                throw new ArgumentException(nameof(output));

            var xsltSettings = new XsltSettings()
            {
                EnableScript = true
            };

            var xslt = new XslCompiledTransform();
            xslt.Load(XmlReader.Create(xsltPath), xsltSettings, null);

            var document = new XPathDocument(input);
            var xmlWriterSettings = new XmlWriterSettings()
            {
                OmitXmlDeclaration = false,
                Indent = true,
                ConformanceLevel = ConformanceLevel.Fragment,
                CloseOutput = false
            };

            var writer = XmlWriter.Create(output, xmlWriterSettings);

            xslt.Transform(document, writer);
        }
    }
}
