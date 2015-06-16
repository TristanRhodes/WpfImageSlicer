﻿using System;
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
        private IExplorationMapBuilder _mapBuilder;
        

        public ImageProcessor(ILogger logger, IExplorationMapBuilder mapBuilder)
        {
            _logger = logger;
            _mapBuilder = mapBuilder;
        }


        public Task<List<PointCollection>> ProcessImage(PixelColor[,] pixels)
        {
            return Task.Run(() => InternalExecute(pixels));
        }

        private List<PointCollection> InternalExecute(PixelColor[,] pixels)
        {
            var map = _mapBuilder.GetExplorationMap(pixels);

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
    }
}
