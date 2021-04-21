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
        Vector2 p1, p2;

        public MidpointLine(Vector2 p1, Vector2 p2, Color color)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.color = color;
        }

        public override void Draw(byte[] RgbValues, int stride, int width, int height)
        {
            void swap(ref int a, ref int b) { int c = a; a = b; b = c; }
            void modPutPixel(int _x, int _y, bool mirror)
            {
                if (!mirror)
                    PutPixel(_x, _y, RgbValues, stride, width, height);
                else
                    PutPixel(_y, _x, RgbValues, stride, width, height);
            }

            int x1 = (int)Math.Round(p1.X);
            int x2 = (int)Math.Round(p2.X);
            int y1 = (int)Math.Round(p1.Y);
            int y2 = (int)Math.Round(p2.Y);

            bool mirrored = false;
            if (Math.Abs(x2-x1) > Math.Abs(y2-y1))
            {
                mirrored = true;
                swap(ref x1, ref y1);
                swap(ref x2, ref y2);
            }
            if (x2 < x1)
            {
                swap(ref x1, ref x2);
                swap(ref y1, ref y2);
            }

            int dx = x2 - x1;
            int dy = y2 - y1;

            int d = 2 * dy - dx;
            int dE = 2 * dy;
            int dNE = 2 * (dy - dx);

            int x = x1, y = y1;

            modPutPixel(x, y, mirrored);
            while (x < x2)
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
                modPutPixel(x, y, mirrored);
            }
        }

        public override IEnumerable<Vector2> GetAllPoints()
        {
            return new List<Vector2>() { p1, p2 };
        }

        public override Vector2 GetClosestPoint(Vector2 pos)
        {
            return (p1 - pos).Length() < (p2 - pos).Length() ? p1 : p2;
        }
    }
}
