using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfImageSplicer.Collections;

namespace WpfImageSplicer.Components
{
    public interface IImageProcessor
    {
        Task<List<PointCollection>> ProcessImage(PixelColor[,] pixels);
    }

    public class ImageProcessor : IImageProcessor
    {
        private ILogger _logger;
        

        public ImageProcessor(ILogger logger)
        {
            _logger = logger;
        }


        public Task<List<PointCollection>> ProcessImage(PixelColor[,] pixels)
        {
            return new Task<List<PointCollection>>(
                        () => InternalExecute(pixels));
        }

        private List<PointCollection> InternalExecute(PixelColor[,] pixels)
        {
            var map = ExploreMap(pixels);

            var shapeDetector = new ShapeDetector(map);
            var edgePlotter = new EdgePlotter(_logger);
            var shapeMap = shapeDetector.CreateShapeMap();

            var edgeList = new List<PointCollection>();
            while (shapeDetector.GenerateShape(shapeMap))
            {
                var edge = edgePlotter.CalculateEdge(shapeMap);

                if (edge.Count > 2)
                    edgeList.Add(edge);
            }

            return edgeList;
        }

        private MapState[,] ExploreMap(PixelColor[,] pixels)
        {
            var mapBuilder = new ExplorationMapBuilder(20);
            var map = mapBuilder.GetExplorationMap(pixels);
            return map;
        }
    }
}
