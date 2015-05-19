using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfImageSplicer.Components
{
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
