using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using WpfImageSplicer.Components;
using WpfImageSplicer.Tests.Components;
using NUnit.Framework;

namespace WpfImageSplicer.Tests
{
    [TestFixture]
    public class BasicTestMapTests : BaseTests
    {
        private string _imagePath = "Resources\\BasicTestMap.png";

        [Test]
        public void LoadBasicTestMap()
        {
            var pixels = LoadMap(_imagePath);

            Validate(pixels);
        }

        [Test]
        public void ExploreBasicTestMap()
        {
            var pixels = LoadMap(_imagePath);
            var map = ExploreMap(pixels);

            Validate(map);
        }

        [Test]
        public void DetectBasicTestMapShapes()
        {
            // Should Detect 4 seperate shapes.
            var pixels = LoadMap(_imagePath);
            var map = ExploreMap(pixels);
            var shapes = GenerateShapes(map);

            Assert.AreEqual(4, shapes.Count);
            ValidateShape(shapes[0], 0, 4, 0, 4);
            ValidateShape(shapes[1], 0, 4, 6, 10);
            ValidateShape(shapes[2], 6, 10, 0, 4);
            ValidateShape(shapes[3], 6, 10, 6, 10);
        }

        [Test]
        public void DetectBasicTestMapEdges()
        {
            // Should Detect 4 seperate shapes.
            var pixels = LoadMap(_imagePath);
            var map = ExploreMap(pixels);
            var shapes = GenerateShapes(map);
            var edges = GenerateEdges(shapes);

            Assert.AreEqual(4, edges.Count);

            Assert.AreEqual(12, edges[0].Count);
            Assert.AreEqual(12, edges[1].Count);
            Assert.AreEqual(12, edges[2].Count);
            Assert.AreEqual(12, edges[3].Count);
        }


        private static void ValidateShape(bool[,] map, int lowerX, int upperX, int lowerY, int upperY)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var cell = map[x, y];
                    var inbounds = lowerX <= x && x < upperX && lowerY <= y && y < upperY;

                    Assert.AreEqual(inbounds, cell, string.Format("Invalid Bounds - X:{0}, Y:{1}", x, y));
                }
            }
        }

        private static void Validate(PixelColor[,] pixels)
        {
            int width = pixels.GetLength(0);
            int height = pixels.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Check the image is loaded correctly, but note that
                    // what constitutes a closed pixel will change.
                    var closedPixel = x == 4 || x == 5 || y == 4 || y == 5;
                    var pixel = pixels[x, y];

                    bool r;
                    bool g;
                    bool b;

                    if (closedPixel)
                    {
                        r = pixel.Red == 0;
                        g = pixel.Green == 0;
                        b = pixel.Blue == 0;
                    }
                    else
                    {
                        r = pixel.Red == 255;
                        g = pixel.Green == 255;
                        b = pixel.Blue == 255;
                    }

                    Assert.IsTrue(r && g && b, "Pixel Invalid");
                }
            }
        }

        private static void Validate(MapState[,] map)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Check the image is loaded correctly, but note that
                    // what constitutes a closed pixel will change.
                    var closedPixel = x == 4 || x == 5 || y == 4 || y == 5;

                    Assert.AreEqual(!closedPixel, map[x, y].Open, string.Format("X: {0}, Y: {1}", x, y));
                    Assert.AreEqual(false, map[x, y].Explored, string.Format("X: {0}, Y: {1}", x, y));
                }
            }
        }
    }
}
