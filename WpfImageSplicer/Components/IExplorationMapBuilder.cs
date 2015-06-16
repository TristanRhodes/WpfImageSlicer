using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfImageSplicer.Components
{
    public interface IExplorationMapBuilder
    {
        MapState[,] GetExplorationMap(PixelColor[,] source);
    }

    public class ExplorationMapBuilder : IExplorationMapBuilder
    {
        private IPixelComparer _pixelComparer;
        

        public ExplorationMapBuilder(IPixelComparer pixelComparer)
        {
            _pixelComparer = pixelComparer;
        }


        public MapState[,] GetExplorationMap(PixelColor[,] source)
        {
            int width = source.GetLength(0);
            int height = source.GetLength(1);

            var cellState = new MapState[width, height];

            // TODO: Parallelise
            // TODO: Refactor into Strategy pattern.
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var newColor = source[x, y]
                                    .ToColor();

                    var open = _pixelComparer
                                    .IsOpen(newColor);

                    cellState[x, y] = open ?
                        MapState.AsOpen() :
                        MapState.AsClosed();
                }
            }

            return cellState;
        }
    }
}
