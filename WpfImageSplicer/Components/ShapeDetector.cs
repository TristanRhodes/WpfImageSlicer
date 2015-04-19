using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace WpfImageSplicer.Components
{
    public class ShapeDetector
    {
        private static readonly Point[] _vectorList = new Point[] 
            { 
                new Point(0, 1), 
                new Point(1, 0), 
                new Point(0, -1), 
                new Point(-1, 0) 
            };


        private Queue<Point> _explorationQueue;
        
        private MapState[,] _map;


        public ShapeDetector(MapState[,] map)
        {
            _map = map;
        
            _explorationQueue = new Queue<Point>();
        }


        public bool[,] CreateShapeMap()
        {
            int width = _map.GetLength(0);
            int height = _map.GetLength(1);

            var shapeMap = new bool[width, height];

            return shapeMap;
        }

        /// <summary>
        /// Generate a shape map from the supplied map state. Should return null
        /// if no shape can be found.
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public bool GenerateShape(bool[,] shapeMap)
        {
            if (shapeMap == null)
                throw new ArgumentNullException("shapeMap");

            if (shapeMap.GetLength(0) != _map.GetLength(0))
                throw new ArgumentException("Width Size Mismatch");

            if (shapeMap.GetLength(1) != _map.GetLength(1))
                throw new ArgumentException("Height Size Mismatch");

            return FindShape(shapeMap);
        }

        private bool FindShape(bool[,] shapeMap)
        {
            int width = _map.GetLength(0);
            int height = _map.GetLength(1);

            // Fill map with false
            for(int x=0; x<width; x++)
            {
                for(int y=0; y<height; y++)
                {
                    shapeMap[x, y] = false;
                }
            }
            // find open node
            var startingPoint = GetFirstNode();
            if (startingPoint == null)
                return false;


            // Start with white nodes, and find all other white nodes.
            _explorationQueue.Clear();
            _explorationQueue.Enqueue(startingPoint.Value);

            while (_explorationQueue.Count > 0)
            {
                // Add root point to explored queue. Toggle as explored.
                var point = _explorationQueue.Dequeue();

                shapeMap[point.X, point.Y] = true;
                _map[point.X, point.Y] = _map[point.X, point.Y].AsExplored();

                for (var i = 0; i < _vectorList.Length; i++)
                {
                    var vector = _vectorList[i];
                    var target = new Point(point.X + vector.X, point.Y + vector.Y);

                    // Bound Check
                    if (target.X >= width || target.X < 0 ||
                        target.Y >= height || target.Y < 0)
                        continue;

                    // Get cell
                    var cell = _map[target.X, target.Y];

                    // Skip if it's not open, or has already been explored
                    if (!cell.Open || cell.Explored)
                        continue;

                    // Set as explored and queue for further exploration
                    _map[target.X, target.Y] = _map[target.X, target.Y].AsExplored();
                    _explorationQueue.Enqueue(target);
                }
            }

            return true;
        }

        private Point? GetFirstNode()
        {
            int width = _map.GetLength(0);
            int height = _map.GetLength(1);

            for(int x=0; x < width; x++)
            {
                for(int y=0; y < height; y++)
                {
                    var pixel = _map[x, y];
                    if (pixel.Open && !pixel.Explored)
                        return new Point(x, y);
                }
            }

            return null;
        }
    }
}
