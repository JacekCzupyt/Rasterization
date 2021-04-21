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

            int x1 = (int)Math.Round(Position.X), y1 = (int)Math.Round(Position.Y);
            
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
