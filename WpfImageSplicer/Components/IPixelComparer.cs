using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfImageSplicer.Components
{
    public interface IPixelComparer
    {
        bool IsOpen(Color color);
    }

    public class TolerancePixelComparer : IPixelComparer
    {
        private int _tolerance;
        private Color _openCellColor;

        public TolerancePixelComparer(int tolerance, Color openCellColor)
        {
            _tolerance = tolerance;
            _openCellColor = openCellColor;
        }

        public bool IsOpen(Color color)
        {
            return Math.Abs(_openCellColor.R - color.R) < _tolerance &&
                   Math.Abs(_openCellColor.G - color.G) < _tolerance &&
                   Math.Abs(_openCellColor.B - color.B) < _tolerance;
        }
    }
}
