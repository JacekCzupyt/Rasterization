using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Rasterization.DrawingObjects
{
    [Serializable]
    class FilledCircle : MidpointCircle
    {
        public FilledCircle(Vector pos, double rad, Color color) : base(pos, rad, color) { }

        public FilledCircle(DrawingPoint pos, double rad, Color color) : base(pos, rad, color) { }

        public FilledCircle(Vector pos, Vector point2, Color color) : base(pos, point2, color) {}

        void PutFillCirclePixel(int _x, int _y, int x0, int y0, byte[] RgbValues, BitmapData bmpData, double mod = 1)
        {
            for (int i = 0; i <= _y; i++)
                for (int c = 0; c < 8; c++)
                    modPutPixel(_x, i, x0, y0, c, RgbValues, bmpData, mod);
        }

        public override void Draw(byte[] RgbValues, BitmapData bmpData, bool Antialiesing)
        {
            if (!Antialiesing)
                DrawSimple(RgbValues, bmpData);
            else
                DrawAntialiesed(RgbValues, bmpData);
        }

        private void DrawSimple(byte[] RgbValues, BitmapData bmpData)
        {
            updateRadius();
            
            int x1 = (int)Math.Round(Position.X), y1 = (int)Math.Round(Position.Y);
            int r = (int)Math.Round(Radius);

            int d = 1 - r;
            int x = 0;
            int y = r;
            PutFillCirclePixel(x, y, x1, y1, RgbValues, bmpData);
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
                PutFillCirclePixel(x, y, x1, y1, RgbValues, bmpData);
            }
        }

        private void DrawAntialiesed(byte[] RgbValues, BitmapData bmpData)
        {
            updateRadius();


            int x1 = (int)Math.Round(Position.X), y1 = (int)Math.Round(Position.Y);
            double r = Radius;

            double y = r;
            for (int x = 0; x < y; x++)
            {
                y = (double)Math.Sqrt(r * r - x * x);
                PutFillCirclePixel(x, (int)y, x1, y1, RgbValues, bmpData);
                PutCirclePixel(x, (int)y + 1, x1, y1, RgbValues, bmpData, y % 1);
            }
        }
    }
}
