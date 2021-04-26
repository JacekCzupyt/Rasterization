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
    class MidpointCircle : AbstractDrawingObject
    {
        public DrawingPoint Position;
        private double rad;
        public double Radius { get { updateRadius(); return rad; } set { rad = value; radiusUtilityPoint.Point = Position.Point + new Vector(rad, 0); } }

        public DrawingPoint radiusUtilityPoint;

        private void Position_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            radiusUtilityPoint.Point = Position.Point + new Vector(rad, 0);
        }

        public MidpointCircle(Vector pos, double rad, Color color)
        {
            this.color = color;
            this.Position = new DrawingPoint(pos);
            radiusUtilityPoint = new DrawingPoint(new Vector());
            this.Radius = rad;

            this.Position.PropertyChanged += Position_PropertyChanged;
        }

        public MidpointCircle(DrawingPoint pos, double rad, Color color)
        {
            this.color = color;
            this.Position = pos;
            radiusUtilityPoint = new DrawingPoint(new Vector());
            this.Radius = rad;

            this.Position.PropertyChanged += Position_PropertyChanged;
        }

        public MidpointCircle(Vector pos, Vector point2, Color color)
        {
            this.color = color;
            this.Position = new DrawingPoint(pos);
            this.radiusUtilityPoint = new DrawingPoint(point2);

            this.Position.PropertyChanged += Position_PropertyChanged;
        }

        protected void updateRadius()
        {
             rad = Position.dist(radiusUtilityPoint.Point);
        }

        protected void PutCirclePixel(int _x, int _y, int x0, int y0, byte[] RgbValues, BitmapData bmpData, double mod = 1)
        {
            for (int c = 0; c < 8; c++)
                modPutPixel(_x, _y, x0, y0, c, RgbValues, bmpData, mod);
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
            PutCirclePixel(x, y, x1, y1, RgbValues, bmpData);
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
                PutCirclePixel(x, y, x1, y1, RgbValues, bmpData);
            }
        }

        

        private void DrawAntialiesed(byte[] RgbValues, BitmapData bmpData)
        {
            updateRadius();

            int x1 = (int)Math.Round(Position.X), y1 = (int)Math.Round(Position.Y);
            double r = Radius;

            double y = r;
            for(int x = 0;x<y;x++)
            {
                y = (double)Math.Sqrt(r * r - x * x);
                PutCirclePixel(x, (int)y, x1, y1, RgbValues, bmpData, 1 - y % 1);
                PutCirclePixel(x, (int)y+1, x1, y1, RgbValues, bmpData, y % 1);
            }
        }

        public override IEnumerable<DrawingPoint> GetTranslationPoints()
        {
            return new List<DrawingPoint>() { Position };
        }

        public override DrawingPoint GetClosestPoint(Vector pos)
        {
            updateRadius();
            if(Position.dist(pos) < Radius / 2)
            {
                return Position;
            }
            else
            {
                radiusUtilityPoint.Point = Position.Point + (pos - Position.Point) * Radius / (pos - Position.Point).Length;
                return radiusUtilityPoint;
            }
        }
    }
}
