using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Rasterization.DrawingObjects
{
    class DrawingPoint
    {
        public DrawingPoint(Vector2 p) { Point = p; }
        public static implicit operator DrawingPoint(Vector2 p) { return new DrawingPoint(p); }

        public Vector2 Point;

        public float X { get { return Point.X; } set { Point.X = value; } }
        public float Y { get { return Point.Y; } set { Point.Y = value; } }

        public float dist(Vector2 pos) { return (pos - Point).Length(); }
    }

    public static class PointExt
    {
        public static Vector2 ToVector2(this System.Windows.Point point)
        {
            return new Vector2((float)point.X, (float)point.Y);
        }
    }
}
