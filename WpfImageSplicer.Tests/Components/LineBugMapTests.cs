using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace WpfImageSplicer.Tests.Components
{
    [TestFixture]
    public class LineBugMapTests : BaseTests
    {
        private string _imagePath = "Resources\\LineBugTestMap.png";

        [Test]
        public void LoadTestMap()
        {
            var pixels = LoadMap(_imagePath);

            Validate(pixels);
        }

        [Test]
        public void ExploreTestMap()
        {
            var pixels = LoadMap(_imagePath);
            var map = ExploreMap(pixels);

            Validate(map);
        }

        [Test]
        public void DetectMapShapes()
        {
            // Should Detect 1 seperate shapes.
            var pixels = LoadMap(_imagePath);
            var map = ExploreMap(pixels);
            var shapes = GenerateShapes(map);

            Assert.AreEqual(1, shapes.Count);
            ValidateShape(shapes[0], 0, 12, 0, 11, 121, 11);
        }

        [Test]
        public void DetectMapEdges()
        {
            // Should Detect 4 seperate shapes.
            var pixels = LoadMap(_imagePath);
            var map = ExploreMap(pixels);
            var shapes = GenerateShapes(map);
            var edges = GenerateEdges(shapes);

            Assert.AreEqual(1, edges.Count);

            Assert.AreEqual(10, edges[0].Count);
        }


        private void Validate(WpfImageSplicer.Components.PixelColor[,] pixels)
        {
            int width = pixels.GetLength(0);
            int height = pixels.GetLength(1);

            int counter = 0;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Check the image is loaded correctly, but note that
                    // what constitutes a closed pixel will change.
                    var pixel = pixels[x, y];

                    var r = pixel.Red == 0;
                    var g = pixel.Green == 0;
                    var b = pixel.Blue == 0;

                    if (r && g && b)
                        counter++;
                }
            }

            Assert.AreEqual(121, counter, "Expected closed pixel count does not match.");
        }

        private void Validate(WpfImageSplicer.Components.MapState[,] map)
        {
        }


        private static void ValidateShape(bool[,] map, int lowerX, int upperX, int lowerY, int upperY, int expectedClosedCells, int expectedOpenCells)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            int closedCellCounter = 0;
            int openCellCounter = 0;

            for (int x = lowerX; x < upperX; x++)
            {
                for (int y = lowerY; y < upperY; y++)
                {
                    var cell = map[x, y];
                    var inbounds = lowerX <= x && x < upperX && lowerY <= y && y < upperY;

                    if (!cell)
                        closedCellCounter++;
                    else
                        openCellCounter++;

                    Assert.IsTrue(inbounds, string.Format("Not in bounds: X: {0}, Y: {1}", x, y));
                }
            }

            Assert.AreEqual(expectedClosedCells, closedCellCounter, "Closed Cell Missmatch");
            Assert.AreEqual(expectedOpenCells, openCellCounter, "Open Cell Missmatch");
        }

    }
}
