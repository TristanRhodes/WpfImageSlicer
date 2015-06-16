using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using WpfImageSplicer.Components;
using NUnit.Framework;
using System.Windows.Media;

namespace WpfImageSplicer.Tests.Components
{
    public class BaseTests
    {
        public static PixelColor[,] LoadMap(string imagePath)
        {
            var imageSource = GetSource(imagePath);
            var pixelMapBuilder = new PixelMapBuilder();
            var pixels = pixelMapBuilder.GetPixels(imageSource);
            return pixels;
        }

        public static MapState[,] ExploreMap(PixelColor[,] pixels)
        {
            var comparer = new TolerancePixelComparer(20, Color.FromRgb(255, 255, 255));
            var mapBuilder = new ExplorationMapBuilder(comparer);
            var map = mapBuilder.GetExplorationMap(pixels);
            return map;
        }

        public static List<bool[,]> GenerateShapes(MapState[,] map)
        {
            var shapes = new List<bool[,]>();
            var shapeDetector = new ShapeDetector(map);

            var shapeMap = shapeDetector.CreateShapeMap();
            while (shapeDetector.GenerateShape(shapeMap))
            {
                shapes.Add(shapeMap);
                shapeMap = shapeDetector.CreateShapeMap();
            }
            return shapes;
        }

        public static List<List<Point>> GenerateEdges(List<bool[,]> shapes)
        {
            var edgePlotter = new EdgePlotter(new TraceLogger());

            var edges = new List<List<Point>>();
            foreach (var shape in shapes)
            {
                var edge = edgePlotter.CalculateEdge(shape);
                edges.Add(edge);
            }
            return edges;
        }


        private static BitmapSource GetSource(string path)
        {
            var url = path;
            var uri = new Uri(url, UriKind.Relative);

            return new BitmapImage(uri);
        }

    }
}
