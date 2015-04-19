using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfImageSplicer
{
    public struct Vector
     {
            public int X;
            public int Y;

            public Vector(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public override string ToString()
            {
                return string.Format("X: {0}, Y: {1}", X, Y);
            }

            
            public static bool operator ==(Vector p1, Vector p2)
            {
                return (p1.X == p2.X && p1.Y == p2.Y);
            }

            public static bool operator !=(Vector p1, Vector p2)
            {
                return !(p1 == p2);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is Vector))
                    return false;

                var target = (Vector)obj;

                return X == target.X && Y == target.Y;
            }

            public override int GetHashCode()
            {
                return (X + Y).GetHashCode();
            }
        }
    }
