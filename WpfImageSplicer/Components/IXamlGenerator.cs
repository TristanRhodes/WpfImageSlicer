using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfImageSplicer.Collections;

namespace WpfImageSplicer.Components
{
    public interface IXamlGenerator
    {
        string GenerateXaml(double width, double height, IEnumerable<PointCollection> shapes);
    }

    public class DefaultXamlGenerator : IXamlGenerator
    {
        public string GenerateXaml(double width, double height, IEnumerable<PointCollection> shapes)
        {
            //NOTE: This is a work in progress. :)


            //HACK: Quick export implementation. To refactor out!!!!
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

            //TODO: Find a way to export the style as a resource, rather than embedded in the XAML.
            //NOTE: Might requre an XML transform as a post process step.

            //root.Resources.Add("PathStyle", style);

            foreach (var shape in shapes)
            {
                var path = new System.Windows.Shapes.Path();
                path.Data = (System.Windows.Media.PathGeometry)converter.Convert(shape, null, null, null);
                path.Style = style;
                //TODO: Use a binding to a StaticResource. Can't find a current answer.
                //var binding = new System.Windows.Data.Binding("PathStyle");
                //path.SetBinding(System.Windows.Shapes.Path.StyleProperty, binding);
                root.Children.Add(path);
            }

            // TODO: Implement Proper XML Formatting.
            return System.Windows.Markup.XamlWriter.Save(root);
        }
    }
}
