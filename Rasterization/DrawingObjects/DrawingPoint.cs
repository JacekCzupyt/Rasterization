﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Rasterization.DrawingObjects
{
    class DrawingPoint
    {
        DrawingPoint(Vector2 p) { Point = p; }
        public Vector2 Point;

        public float X { get { return Point.X; } set { Point.X = value; } }
        public float Y { get { return Point.Y; } set { Point.Y = value; } }

        public float dist(Vector2 pos) { return (pos - Point).Length(); }
    }
}
