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
        private IPixelComparer _pixelComparer;
        

        public ExplorationMapBuilder(IPixelComparer pixelComparer)
        {
            _pixelComparer = pixelComparer;
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

                    var open = _pixelComparer.AreSimilar(openCellColor, newColor);

                    cellState[x, y] = open ?
                        MapState.AsOpen() :
                        MapState.AsClosed();
                }
            }

            return cellState;
        }
    }
}
