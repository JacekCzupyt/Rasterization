using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Rasterization.DrawingObjects
{
    class MidpointLine : AbstractDrawingObject
    {
        public DrawingPoint Point1, Point2;

        public MidpointLine(Vector2 p1, Vector2 p2, Color color)
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

        public override void Draw(byte[] RgbValues, int stride, int width, int height, bool Antialiesing)
        {
            if (!Antialiesing)
                DrawSimple(RgbValues, stride, width, height);
            else
                DrawAntialiesed(RgbValues, stride, width, height);
        }

        private void DrawSimple(byte[] RgbValues, int stride, int width, int height)
        {
            void swap(ref int a, ref int b) { int tmp = a; a = b; b = tmp; }
            void modPutPixel(int _x, int _y, int x0, int y0, int c0)
            {
                if (c0 % 2 >= 1)
                    swap(ref _x, ref _y);
                if (c0 % 4 >= 2)
                    _x = -_x;
                if (c0 % 8 >= 4)
                    _y = -_y;
                PutPixel(x0+_x, y0+_y, RgbValues, stride, width, height);
            }

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

            modPutPixel(x, y, x1, y1, c);
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
                modPutPixel(x, y, x1, y1, c);
            }
        }

        private void DrawAntialiesed(byte[] RgbValues, int stride, int width, int height)
        {
            void swap(ref int a, ref int b) { int tmp = a; a = b; b = tmp; }
            void modPutPixel(int _x, int _y, int x0, int y0, int c0, float mod = 1)
            {
                if (c0 % 2 >= 1)
                    swap(ref _x, ref _y);
                if (c0 % 4 >= 2)
                    _x = -_x;
                if (c0 % 8 >= 4)
                    _y = -_y;
                PutPixel(x0 + _x, y0 + _y, RgbValues, stride, width, height, mod);
            }

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


            float y = 0;
            float m = (float)dy / dx;

            for (int x = 0; x < dx; x++)
            {
                modPutPixel(x, (int)y, x1, y1, c, 1-y%1);
                modPutPixel(x, (int)y+1, x1, y1, c, y % 1);
                y += m;
            }
        }

        public override IEnumerable<DrawingPoint> GetTranslationPoints()
        {
            return new List<DrawingPoint>() { Point1, Point2 };
        }

        public override DrawingPoint GetClosestPoint(Vector2 pos)
        {
            return Point1.dist(pos) < Point2.dist(pos) ? Point1 : Point2;
        }
    }
}
