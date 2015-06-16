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
        bool AreSimilar(Color c1, Color c2);
    }

    public class TolerancePixelComparer : IPixelComparer
    {
        private int _tolerance;

        public TolerancePixelComparer(int tolerance)
        {
            _tolerance = tolerance;
        }

        public bool AreSimilar(Color c1, Color c2)
        {
            return Math.Abs(c1.R - c2.R) < _tolerance &&
                   Math.Abs(c1.G - c2.G) < _tolerance &&
                   Math.Abs(c1.B - c2.B) < _tolerance;
        }
    }
}
