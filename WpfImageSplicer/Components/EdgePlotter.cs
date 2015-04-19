using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfImageSplicer.Components
{
    public class EdgePlotter
    {
        private Queue<Vector> _vectorQueue = new Queue<Vector>();
        private Queue<VectorPoint> _ExplorationQueue = new Queue<VectorPoint>();
        private ILogger _logger;
        private static bool _log = false;

        public EdgePlotter(ILogger logger)
        {
            _logger = logger;
            ConfigureVectorQueue();
        }

        private void ConfigureVectorQueue()
        {
            _vectorQueue.Clear();


            _vectorQueue.Enqueue(new Vector(1, 1));
            _vectorQueue.Enqueue(new Vector(1, 0));
            _vectorQueue.Enqueue(new Vector(1, -1));
            _vectorQueue.Enqueue(new Vector(0, -1));
            _vectorQueue.Enqueue(new Vector(-1, -1));
            _vectorQueue.Enqueue(new Vector(-1, 0));
            _vectorQueue.Enqueue(new Vector(-1, 1));
            _vectorQueue.Enqueue(new Vector(0, 1));
        }


        public List<Point> CalculateEdge(bool[,] shapeMap)
        {
            // Find First Cell
            // Explore Until you find a cell that is not an edge
            // Start from the first clockwise vector

            var path = new List<Point>();
            var startPoint = GetStartPoint(shapeMap);
            var startVector = new Vector(0, 1);

            if (_log) _logger.Trace("==== Calculating New Edge ====");

            _ExplorationQueue.Clear();
            _ExplorationQueue.Enqueue(new VectorPoint(startPoint, startVector));

            while (_ExplorationQueue.Count > 0)
            {
                //if (path.Count > 20000)
                //    Debugger.Break();

                FollowEdge(shapeMap, path);
                //System.Diagnostics.Trace.WriteLine("Move to: " + path[path.Count-1]);
            }

            return path;
        }

        private Point GetStartPoint(bool[,] shapeMap)
        {
            int width = shapeMap.GetLength(0);
            int height = shapeMap.GetLength(1);

            for (int x = 0; x < width; x++ )
            {
                for(int y=0; y<height; y++)
                {
                    if (shapeMap[x, y])
                        return new Point(x, y);
                }
            }
            
            throw new ApplicationException("No start point found.");
        }

        private void FollowEdge(bool[,] shapeMap, List<Point> path)
        {
            int width = shapeMap.GetLength(0);
            int height = shapeMap.GetLength(1);
            var start = _ExplorationQueue.Dequeue();
            
            var startPoint = start.StartPoint;
            var startVector = start.StartVector;

            if (path.Count > 0 && path[0] == startPoint)
                return;

            path.Add(startPoint);
            if (_log) _logger.Trace("Edge Found: " + startPoint);

            var vectors = GetCheckVectors(startVector);
            foreach(var targetVector in vectors)
            {
                var target = new Point(startPoint.X + targetVector.X, startPoint.Y + targetVector.Y);
                if (_log) _logger.Trace(string.Format("Exploring Vector: {0}, Target: {1}", targetVector, target));      
                
                // Bounds Check, skip if outside.
                if (target.X < 0 || target.X >= width ||
                    target.Y < 0 || target.Y >= height)
                {
                    if (_log) _logger.Trace("Out of Bounds");
                    continue;
                }

                if (shapeMap[target.X, target.Y])
                {
                    if (_log) _logger.Trace("Open");
                    _ExplorationQueue.Enqueue(new VectorPoint(target, targetVector));
                    return;
                }
                else
                    if (_log) _logger.Trace("Closed");
            }
        }

        private Vector[] GetCheckVectors(Vector vector)
        {
            var current = _vectorQueue.Peek();

            if (_log) _logger.Trace("Incoming vector: " + vector);

            // Iterate the vector list to the current vector
            while (current != vector)
            {
                current = _vectorQueue.Dequeue();
                _vectorQueue.Enqueue(current);
            }

            // Cycle by 6 so we can start from the outer angle.
            for (int i = 0; i < 6; i++)
            {
                current = _vectorQueue.Dequeue();
                _vectorQueue.Enqueue(current);
            }

            if (_log) _logger.Trace("Cycling to vector: " + _vectorQueue.Peek());

            return _vectorQueue.ToArray();
        }
    }

    public struct VectorPoint
    {
        public Point StartPoint;
        public Vector StartVector;

        public VectorPoint(Point startPoint, Vector startVector)
        {
            // TODO: Complete member initialization
            this.StartPoint = startPoint;
            this.StartVector = startVector;
        }
       
    }
}
