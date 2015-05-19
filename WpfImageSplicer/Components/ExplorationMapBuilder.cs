using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfImageSplicer.Components
{
    public class ExplorationMapBuilder
    {
        private int _tolerance;

        public ExplorationMapBuilder(int tolerance)
        {
            _tolerance = tolerance;
        }

        public MapState[,] GetExplorationMap(PixelColor[,] source)
        {
            int width = source.GetLength(0);
            int height =source.GetLength(1);

            var cellState = new MapState[width, height];

            var openCellColor = Color.FromRgb(255, 255, 255);

            // TODO: Parallelise
            // TODO: Refactor into Strategy pattern.
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var p = source[x, y];
                    var newColor = Color.FromRgb(p.Red, p.Green, p.Blue);

                    var open = AreColorsSimilar(openCellColor, newColor, _tolerance);

                    cellState[x, y] = open ?
                        MapState.AsOpen() :
                        MapState.AsClosed();
                }
            }

            return cellState;
        }


        private bool AreColorsSimilar(Color c1, Color c2, int tolerance)
        {
            return Math.Abs(c1.R - c2.R) < tolerance &&
                   Math.Abs(c1.G - c2.G) < tolerance &&
                   Math.Abs(c1.B - c2.B) < tolerance;
        }
    }
}
