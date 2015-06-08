using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;
using WpfImageSplicer.Collections;
using WpfImageSplicer.Components;

namespace WpfImageSplicer.Tests.Components
{
    [TestFixture]
    public class DefaultXamlGeneratorTests
    {
        private static readonly XNamespace nsRoot = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        private static readonly XNamespace nsx = "http://schemas.microsoft.com/winfx/2006/xaml";
        private static readonly XNamespace nss = "clr-namespace:System;assembly=mscorlib";

        private DefaultXamlGenerator _generator;
        private List<PointCollection> _shapes;

        [SetUp]
        public void Initialize()
        {
            _generator = new DefaultXamlGenerator();
            _shapes = new List<PointCollection>();
            _shapes.Add(new PointCollection()
                {
                    new Point(0, 0),
                    new Point(10, 0),
                    new Point(10, 10),
                    new Point(0, 10),
                });
        }

        /// <summary>
        /// This is a basic Xaml generation test to make sure that the code
        /// is actually executing and generating a document. It does not guarentee
        /// correctness.
        /// </summary>
        [Test]
        public void ShouldGenerateBasicXaml()
        {
            var result = _generator
                .GenerateXaml(5, 10, _shapes)
                .Result;

            Trace.Write(result);

            var doc = XDocument.Parse(result);

            var nCanvas = GetElement(doc, nsRoot, "Canvas");
            AssertAttributesMatch(nCanvas, "Width", "5");
            AssertAttributesMatch(nCanvas, "Height", "10");

            var nResources = GetElement(nCanvas, nsRoot, "Canvas.Resources");

            // Check we have a style (Basic)
            var nStyle = GetElement(nResources, nsRoot, "Style");
            AssertAttributesMatch(nStyle, "TargetType", "Path");
            AssertAttributesMatch(nStyle, nsx + "Key", "PathStyle");

            // Check we have a shape (Basic)
            var nShape = GetElement(nCanvas, nsRoot, "Path");
            AssertAttributesMatch(nShape, "Style", "{StaticResource PathStyle}");
        }


        private void AssertAttributesMatch(XElement node, XName name, string value)
        {
            var attribute = node.Attribute(name);

            Assert.IsNotNull(attribute, string.Format("Attribute {0} not found on element {1}.", name, node.Name));
            Assert.AreEqual(value, attribute.Value, string.Format("Attribute {0} on element {1} has incorrect value.", name, node.Name));
        }

        private static XElement GetElement(XElement root, XNamespace ns, string name)
        {
            var fullName = nsRoot + name;

            var node = root
                    .Elements(fullName)
                    .SingleOrDefault();

            Assert.IsNotNull(node, string.Format("Node {0} not found on parent {1}.", fullName, root.Name));

            return node;
        }

        private static XElement GetElement(XDocument root, XNamespace ns, string name)
        {
            var fullName = nsRoot + name;

            var node = root
                    .Elements(fullName)
                    .SingleOrDefault();

            Assert.IsNotNull(node, string.Format("Node {0} not found in document root.", fullName));

            return node;
        }
    }
}
