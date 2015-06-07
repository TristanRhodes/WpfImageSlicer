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
            return StartSTATask<string>(() => InternalExecute(width, height, shapes));;
        }

        /// <summary>
        /// This method starts a task on the STA tread.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        private static Task<T> StartSTATask<T>(Func<T> func)
        {
            var tcs = new TaskCompletionSource<T>();
            Thread thread = new Thread(() =>
            {
                try
                {
                    tcs.SetResult(func());
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            return tcs.Task;
        }

        private string InternalExecute(double width, double height, IEnumerable<PointCollection> shapes)
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

            return GenerateXaml(root);
        }

        private string GenerateXaml(System.Windows.Controls.Canvas root)
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
