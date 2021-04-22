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
        DrawingPoint p1, p2;

        public MidpointLine(Vector2 p1, Vector2 p2, Color color)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.color = color;
        }

        public override void Draw(byte[] RgbValues, int stride, int width, int height)
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

            int x1 = (int)Math.Round(p1.X);
            int x2 = (int)Math.Round(p2.X);
            int y1 = (int)Math.Round(p1.Y);
            int y2 = (int)Math.Round(p2.Y);

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

        public override IEnumerable<DrawingPoint> GetTranslationPoints()
        {
            return new List<DrawingPoint>() { p1, p2 };
        }

        public override DrawingPoint GetClosestPoint(Vector2 pos)
        {
            return p1.dist(pos) < p2.dist(pos) ? p1 : p2;
        }
    }
}
