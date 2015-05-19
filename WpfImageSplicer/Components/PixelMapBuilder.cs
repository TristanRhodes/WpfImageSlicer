using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfImageSplicer.Components
{
    public interface IPixelMapBuilder
    {
        PixelColor[,] GetPixels(BitmapSource source);
    }

    public class PixelMapBuilder : IPixelMapBuilder
    {
        public PixelColor[,] GetPixels(BitmapSource source)
        {
            if (source.Format != PixelFormats.Bgra32)
                source = new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);

            int width = source.PixelWidth;
            int height = source.PixelHeight;
            PixelColor[,] result = new PixelColor[width, height];

            CopyPixels(source, result, width * 4, 0);

            return result;
        }

        private void CopyPixels(BitmapSource source, PixelColor[,] pixels, int stride, int offset)
        {
            var height = source.PixelHeight;
            var width = source.PixelWidth;

            var pixelBytes = new byte[height * width * 4];
            source.CopyPixels(pixelBytes, stride, 0);

            int y0 = offset / width;
            int x0 = offset - width * y0;
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var b = pixelBytes[(y * width + x) * 4 + 0];
                    var g = pixelBytes[(y * width + x) * 4 + 1];
                    var r = pixelBytes[(y * width + x) * 4 + 2];
                    var a = pixelBytes[(y * width + x) * 4 + 3];

                    pixels[x + x0, y + y0] = new PixelColor(r, g, b, a);
                }
            }
        }
    }

    public struct PixelColor
    {
        public byte Blue;
        public byte Green;
        public byte Red;
        public byte Alpha;


        public PixelColor(byte r, byte g, byte b, byte a)
        {
            Red = r;
            Green = g;
            Blue = b;
            Alpha = a;
        }


        public override string ToString()
        {
            return string.Format("#{0:X}{1:X}{2:X}{3:X}", Red, Green, Blue, Alpha);
        }
    }
}
