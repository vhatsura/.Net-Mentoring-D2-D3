using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace AdvancedXML
{
    public sealed class XmlValidator
    {
        /// <summary>
        /// Validate xml file with xsd schema
        /// </summary>
        /// <param name="targetNamespace">The schema targetNamespace property, or null to use the targetNamespace specified in the schema.</param>
        /// <param name="schemaPath">The path to file that specifies the schema to load.</param>
        /// <param name="filePath">The path to file that specifies xml document to load</param>
        /// <param name="errors">The list of errors during validation of xml document</param>
        /// <returns>Validation result. If xml file has errors - false, otherwise - true</returns>
        public bool Validate(string targetNamespace, string schemaPath, string filePath, out IList<string> errors)
        {
            if (string.IsNullOrWhiteSpace(schemaPath))
                throw new ArgumentException(nameof(schemaPath));

            if(string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException(nameof(filePath));

            errors = new List<string>();

            var schemas = new XmlSchemaSet();
            schemas.Add(targetNamespace, XmlReader.Create(new FileStream(schemaPath, FileMode.Open, FileAccess.Read)));

            var document = XDocument.Load(new FileStream(filePath, FileMode.Open));

            var list = errors;
            document.Validate(schemas, (o, e) =>
            {
                list.Add(e.Message);
            });

            return !errors.Any();
        }
    }
}
