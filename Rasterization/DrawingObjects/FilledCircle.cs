using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Rasterization.DrawingObjects
{
    class FilledCircle : MidpointCircle
    {
        public FilledCircle(Vector2 pos, float rad, Color color) : base(pos, rad, color) { }

        public FilledCircle(DrawingPoint pos, float rad, Color color) : base(pos, rad, color) { }

        public FilledCircle(Vector2 pos, Vector2 point2, Color color) : base(pos, point2, color) {}

        public override void Draw(byte[] RgbValues, int stride, int width, int height, bool Antialiesing)
        {
            if (!Antialiesing)
                DrawSimple(RgbValues, stride, width, height);
            else
                DrawAntialiesed(RgbValues, stride, width, height);
        }

        private void DrawSimple(byte[] RgbValues, int stride, int width, int height)
        {
            updateRadius();
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
            void PutFillCirclePixel(int _x, int _y, int x0, int y0)
            {
                for (int i = 0; i <= _y; i++)
                    for (int c = 0; c < 8; c++)
                        modPutPixel(_x, i, x0, y0, c);
            }


            int x1 = (int)Math.Round(Position.X), y1 = (int)Math.Round(Position.Y);
            int r = (int)Math.Round(Radius);

            int d = 1 - r;
            int x = 0;
            int y = r;
            PutFillCirclePixel(x, y, x1, y1);
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
                PutFillCirclePixel(x, y, x1, y1);
            }
        }

        private void DrawAntialiesed(byte[] RgbValues, int stride, int width, int height)
        {
            updateRadius();
            void swap(ref int a, ref int b) { int tmp = a; a = b; b = tmp; }
            void modPutPixel(int _x, int _y, int x0, int y0, int c0, float mod)
            {
                if (c0 % 2 >= 1)
                    swap(ref _x, ref _y);
                if (c0 % 4 >= 2)
                    _x = -_x;
                if (c0 % 8 >= 4)
                    _y = -_y;
                PutPixel(x0 + _x, y0 + _y, RgbValues, stride, width, height, mod);
            }
            void PutCirclePixel(int _x, int _y, int x0, int y0, float mod)
            {
                for (int c = 0; c < 8; c++)
                    modPutPixel(_x, _y, x0, y0, c, mod);
            }
            void PutFillCirclePixel(int _x, int _y, int x0, int y0)
            {
                for (int i = 0; i <= _y; i++)
                    for (int c = 0; c < 8; c++)
                        modPutPixel(_x, i, x0, y0, c, 1);
            }


            int x1 = (int)Math.Round(Position.X), y1 = (int)Math.Round(Position.Y);
            int r = (int)Math.Round(Radius);

            float y = r;
            for (int x = 0; x < y; x++)
            {
                y = (float)Math.Sqrt(r * r - x * x);
                PutFillCirclePixel(x, (int)y, x1, y1);
                PutCirclePixel(x, (int)y + 1, x1, y1, y % 1);
            }
        }
    }
}
