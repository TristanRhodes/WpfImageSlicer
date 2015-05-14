using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfImageSplicer.Collections;

namespace WpfImageSplicer.Converters
{
    public class PathConverter : IValueConverter
    {
        /// <summary>
        /// Converts WpfImageSplicer.Collections.PointCollection into PathGeometry
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var edge = value as WpfImageSplicer.Collections.PointCollection;
            if (edge == null)
                return value;

            var start = new System.Windows.Point(edge[0].X, edge[0].Y); ;

            var segments = new List<LineSegment>();
            for (var i = 1; i < edge.Count; i++)
            {
                var edgePoint = edge[i];
                var wpfPoint = new System.Windows.Point(edgePoint.X, edgePoint.Y);
                segments.Add(new LineSegment(wpfPoint, true));
            }

            var figure = new PathFigure(start, segments, true); //true if closed
            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            return geometry;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
