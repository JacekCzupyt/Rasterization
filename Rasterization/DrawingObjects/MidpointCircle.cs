using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Rasterization.DrawingObjects
{
    class MidpointCircle : AbstractDrawingObject
    {
        Vector2 Position;
        float radius;

        MidpointCircle(Vector2 pos, float rad, Color color)
        {
            this.color = color;
            this.Position = pos;
            this.radius = rad;
        }

        MidpointCircle(Vector2 pos, Vector2 point2, Color color)
        {
            this.color = color;
            this.Position = pos;
            this.radius = (pos-point2).Length();
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
                PutPixel(x0 + _x, y0 + _y, RgbValues, stride, width, height);
            }
            void PutCirclePixel(int _x, int _y, int x0, int y0)
            {
                for (int c = 0; c < 8; c++)
                    modPutPixel(_x, _y, x0, y0, c);
            }


            int x1 = (int)Math.Round(Position.X), y1 = (int)Math.Round(Position.Y);
            int r = (int)Math.Round(radius);

            int d = 1 - r;
            int x = 0;
            int y = r;
            PutCirclePixel(x, y, x1, y1);
            while (y > x)
            {
                if (d < 0)
                    d += 2 * x + 3;
                else
                {
                    d += 2 * x - 2 * y + 5;
                    y--;
                }
                x++;
                PutCirclePixel(x, y, x1, y1);
            }
        }

        public override IEnumerable<Vector2> GetAllPoints()
        {
            return new List<Vector2>() { Position };
        }

        public override Vector2 GetClosestPoint(Vector2 pos)
        {
            return (pos - Position).Length() < radius / 2 ? Position : Position + (pos - Position) * radius / (pos - Position).Length();
        }
    }
}
