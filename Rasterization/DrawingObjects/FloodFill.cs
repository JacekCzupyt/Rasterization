using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;

namespace Rasterization.DrawingObjects
{
    class FloodFill : AbstractDrawingObject
    {
        DrawingPoint point;

        public FloodFill(DrawingPoint p)
        {
            point = p;
        }

        public FloodFill(Vector p)
        {
            point = new DrawingPoint(p);
        }

        public override void Draw(byte[] RgbValues, BitmapData bmpData, bool Antialiesing)
        {
            (int, int)[] dirArr = new (int, int)[] { (1, 0), (-1, 0), (0, 1), (0, -1) };
            (int, int) origin = ((int)point.X, (int)point.Y);
            Queue<(int, int)> q = new Queue<(int, int)>();
            q.Enqueue(origin);
            Color prevColor = GetPixel(origin.Item1, origin.Item2, RgbValues, bmpData);
            if (prevColor == color)
                return;
            PutPixel(origin.Item1, origin.Item2, RgbValues, bmpData);
            for ((int, int) p = q.Dequeue(); q.Count > 0; p = q.Dequeue())
            {
                foreach(var dir in dirArr)
                {
                    int x = p.Item1 + dir.Item1, y = p.Item2 + dir.Item2;
                    if (GetPixel(x, y, RgbValues, bmpData) == prevColor)
                    {
                        PutPixel(x, y, RgbValues, bmpData);
                        q.Enqueue((x, y));
                    }
                }
            }
        }

        public override DrawingPoint GetClosestPoint(Vector pos)
        {
            return point;
        }

        public override IEnumerable<DrawingPoint> GetTranslationPoints()
        {
            return new List<DrawingPoint>() { point };
        }
    }
}
