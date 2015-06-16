using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Xml;
using WpfImageSplicer.Collections;
using WpfImageSplicer.Utilities;

namespace WpfImageSplicer.Components
{
    public interface IXamlGenerator
    {
        Task<string> GenerateXaml(double width, double height, IEnumerable<PointCollection> shapes);
    }

    public class DefaultXamlGenerator : IXamlGenerator
    {
        public Task<string> GenerateXaml(double width, double height, IEnumerable<PointCollection> shapes)
        {
            // Need to create the task on the STA thread, otherwise it fails
            // to create the WPF controls.
            return StaTask.Start<string>(() => InternalGenerateXaml(width, height, shapes));;
        }

        private string InternalGenerateXaml(double width, double height, IEnumerable<PointCollection> shapes)
        {
            var converter = new WpfImageSplicer.Converters.PathConverter();

            // Setup Style
            var style = new System.Windows.Style();
            style.TargetType = typeof(System.Windows.Shapes.Path);

            style.Setters.Add(
                new System.Windows.Setter(System.Windows.Shapes.Path.StrokeThicknessProperty,
                    1.0));

            style.Setters.Add(
                new System.Windows.Setter(System.Windows.Shapes.Path.StrokeProperty,
                new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black)));

            style.Setters.Add(
                new System.Windows.Setter(System.Windows.Shapes.Path.FillProperty,
                new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent)));

            var root = new System.Windows.Controls.Canvas();
            root.Width = width;
            root.Height = height;

            // Style added so that all paths can point to this.
            root.Resources.Add("PathStyle", style);

            foreach (var shape in shapes)
            {
                var path = new System.Windows.Shapes.Path();
                path.Data = (System.Windows.Media.PathGeometry)converter.Convert(shape, null, null, null);
               
                // Create a null binding so we have something to replace
                var binding = new System.Windows.Data.Binding("{null}");
                path.SetBinding(System.Windows.Shapes.Path.StyleProperty, binding);

                root.Children.Add(path);
            }

            return GenerateXamlFromRoot(root);
        }

        private string GenerateXamlFromRoot(System.Windows.Controls.Canvas root)
        {
            // For proper XML formatting
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "    ";
            settings.OmitXmlDeclaration = true;
            
            var builder = new StringBuilder();
            using (var writer = XmlWriter.Create(builder, settings))
            {
                System.Windows.Markup.XamlWriter.Save(root, writer);

                // Post process paths to use style binding (Hacky...)
                builder.Replace("Style=\"{x:Null}\"", "Style=\"{StaticResource PathStyle}\"");

                return builder.ToString();
            }
        }
    }
}
