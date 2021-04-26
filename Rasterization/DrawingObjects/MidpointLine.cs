using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;

namespace Rasterization.DrawingObjects
{
    [Serializable]
    class MidpointLine : AbstractDrawingObject
    {
        public DrawingPoint Point1, Point2;

        public MidpointLine(Vector p1, Vector p2, Color color)
        {
            this.Point1 = new DrawingPoint(p1);
            this.Point2 = new DrawingPoint(p2);
            this.color = color;
        }

        public MidpointLine(DrawingPoint p1, DrawingPoint p2, Color color)
        {
            this.Point1 = p1;
            this.Point2 = p2;
            this.color = color;
        }

        public override void Draw(byte[] RgbValues, BitmapData bmpData, bool Antialiesing)
        {
            if (!Antialiesing)
                DrawSimple(RgbValues, bmpData);
            else
                DrawAntialiesed(RgbValues, bmpData);
        }

        protected void DrawSimple(byte[] RgbValues, BitmapData bmpData)
        {
            int x1 = (int)Math.Round(Point1.X);
            int x2 = (int)Math.Round(Point2.X);
            int y1 = (int)Math.Round(Point1.Y);
            int y2 = (int)Math.Round(Point2.Y);

            int dx = x2 - x1;
            int dy = y2 - y1;

            int c = 0;
            if (dy<0)
            {
                dy = -dy;
                c += 4;
            }
            if (dx<0)
            {
                dx = -dx;
                c += 2;
            }
            if (Math.Abs(x2-x1) < Math.Abs(y2-y1))
            {
                c += 1;
                swap(ref dy, ref dx);
            }

            int d = 2 * dy - dx;
            int dE = 2 * dy;
            int dNE = 2 * (dy - dx);

            int x = 0, y = 0;

            modPutPixel(x, y, x1, y1, c, RgbValues, bmpData);
            while (x < dx)
            {
                if (d < 0)
                {
                    d += dE;
                    x++;
                }
                else
                {
                    d += dNE;
                    x++;
                    y++;
                }
                modPutPixel(x, y, x1, y1, c, RgbValues, bmpData);
            }
        }

        private void DrawAntialiesed(byte[] RgbValues, BitmapData bmpData)
        {

            int x1 = (int)Math.Round(Point1.X);
            int x2 = (int)Math.Round(Point2.X);
            int y1 = (int)Math.Round(Point1.Y);
            int y2 = (int)Math.Round(Point2.Y);

            int dx = x2 - x1;
            int dy = y2 - y1;

            int c = 0;
            if (dy < 0)
            {
                dy = -dy;
                c += 4;
            }
            if (dx < 0)
            {
                dx = -dx;
                c += 2;
            }
            if (Math.Abs(x2 - x1) < Math.Abs(y2 - y1))
            {
                c += 1;
                swap(ref dy, ref dx);
            }


            double y = 0;
            double m = (double)dy / dx;

            for (int x = 0; x < dx; x++)
            {
                modPutPixel(x, (int)y, x1, y1, c, RgbValues, bmpData, 1-y%1);
                modPutPixel(x, (int)y+1, x1, y1, c, RgbValues, bmpData, y % 1);
                y += m;
            }
        }

        public override IEnumerable<DrawingPoint> GetTranslationPoints()
        {
            return new List<DrawingPoint>() { Point1, Point2 };
        }

        public override DrawingPoint GetClosestPoint(Vector pos)
        {
            return Point1.dist(pos) < Point2.dist(pos) ? Point1 : Point2;
        }
    }
}
