using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace AdvancedXML
{
    public sealed class XmlTransformer
    {
        public void Transform(string xsltPath, Stream input, Stream output) =>
            Transform(xsltPath, input, output, new Dictionary<string, object>());

        public void Transform(string xsltPath, Stream input, Stream output, Dictionary<string, object> parameters)
        {
            if (string.IsNullOrWhiteSpace(xsltPath))
                throw new ArgumentException(nameof(xsltPath));
            if (input == null)
                throw new ArgumentException(nameof(input));
            if (output == null)
                throw new ArgumentException(nameof(output));
            if (parameters == null)
                throw new ArgumentException(nameof(parameters));

            var xsltSettings = new XsltSettings() { EnableScript = true };

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
            xslt.Transform(document, GetArgumentList(parameters), writer);
        }

        private static XsltArgumentList GetArgumentList(Dictionary<string, object> parameters)
        {
            var argumentList = new XsltArgumentList();
            foreach (var parameter in parameters)
            {
                argumentList.AddParam(parameter.Key, "", parameter.Value);
            }

            return argumentList;
        }
    }
}
