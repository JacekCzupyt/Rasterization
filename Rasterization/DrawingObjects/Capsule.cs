using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Rasterization.DrawingObjects
{
    [Serializable]
    class Capsule : AbstractDrawingObject
    {
        public DrawingPoint Point1, Point2;
        private Vector Dir { get { Vector dir = Point2.Point - Point1.Point; dir.Normalize(); return double.IsNaN(dir.Length) ? new Vector(1, 0) : dir; } }
        private double rad;

        public double Radius { get { updateRadius(); return rad; } set { rad = value; radiusUtilityPoint.Point = Point2.Point + Dir * rad; } }

        public DrawingPoint radiusUtilityPoint;

        protected void updateRadius()
        {
            if (Dir * (radiusUtilityPoint.Point - Point2.Point) >= 0)
                rad = (Point2.Point - radiusUtilityPoint.Point).Length;
            else if (-Dir * (radiusUtilityPoint.Point - Point1.Point) >= 0)
                rad = (Point1.Point - radiusUtilityPoint.Point).Length;
            else
                rad = Math.Abs((radiusUtilityPoint.Point - Point1.Point) * new Vector(-Dir.Y, Dir.X));

        }

        private void Position_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Radius = rad;
        }

        public Capsule(Vector p1, Vector p2, double radi, Color color)
        {
            this.Point1 = new DrawingPoint(p1);
            this.Point2 = new DrawingPoint(p2);
            radiusUtilityPoint = new DrawingPoint(new Vector());
            this.Radius = radi;
            this.color = color;

            this.Point1.PropertyChanged += Position_PropertyChanged;
            this.Point2.PropertyChanged += Position_PropertyChanged;
        }

        public Capsule(DrawingPoint p1, DrawingPoint p2, double radi, Color color)
        {
            this.Point1 = p1;
            this.Point2 = p2;
            radiusUtilityPoint = new DrawingPoint(new Vector());
            this.Radius = radi;
            this.color = color;

            this.Point1.PropertyChanged += Position_PropertyChanged;
            this.Point2.PropertyChanged += Position_PropertyChanged;
        }

        public Capsule(Vector p1, Vector p2, Vector rad, Color color)
        {
            this.Point1 = new DrawingPoint(p1);
            this.Point2 = new DrawingPoint(p2);
            radiusUtilityPoint = new DrawingPoint(rad);
            updateRadius();
            this.color = color;

            this.Point1.PropertyChanged += Position_PropertyChanged;
            this.Point2.PropertyChanged += Position_PropertyChanged;
        }

        public override void Draw(byte[] RgbValues, BitmapData bmpData, bool Antialiesing)
        {
            Vector dir = (Point2.Point - Point1.Point);
            dir.Normalize();
            Vector orthogonal = new Vector(-dir.Y, dir.X);
            new MidpointLine(Point1.Point + orthogonal * Radius, Point2.Point + orthogonal * Radius, color).Draw(RgbValues, bmpData, Antialiesing);
            new MidpointLine(Point1.Point - orthogonal * Radius, Point2.Point - orthogonal * Radius, color).Draw(RgbValues, bmpData, Antialiesing);
            if (Antialiesing)
            {
                DrawAntialiesedSemicircle(Point1.Point, -dir, RgbValues, bmpData);
                DrawAntialiesedSemicircle(Point2.Point, dir, RgbValues, bmpData);
            }
            else
            {
                DrawSimpleSemicircle(Point1.Point, -dir, RgbValues, bmpData);
                DrawSimpleSemicircle(Point2.Point, dir, RgbValues, bmpData);
            }
        }

        protected void PutSemiCirclePixel(int _x, int _y, int x0, int y0, Vector dir, byte[] RgbValues, BitmapData bmpData, double mod = 1)
        {
            for (int c = 0; c < 8; c++)
            {
                int x = _x, y = _y;
                if (c % 2 >= 1)
                    swap(ref x, ref y);
                if (c % 4 >= 2)
                    x = -x;
                if (c % 8 >= 4)
                    y = -y;
                if(new Vector(x, y) * dir >= 0)
                    PutPixel(x0 + x, y0 + y, RgbValues, bmpData, mod);
            }
        }

        private void DrawSimpleSemicircle(Vector center, Vector direction, byte[] RgbValues, BitmapData bmpData)
        {
            int x1 = (int)Math.Round(center.X), y1 = (int)Math.Round(center.Y);
            int r = (int)Math.Round(Radius);

            int d = 1 - r;
            int x = 0;
            int y = r;
            PutSemiCirclePixel(x, y, x1, y1, direction, RgbValues, bmpData);
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
                PutSemiCirclePixel(x, y, x1, y1, direction, RgbValues, bmpData);
            }
        }

        private void DrawAntialiesedSemicircle(Vector center, Vector direction, byte[] RgbValues, BitmapData bmpData)
        {
            int x1 = (int)Math.Round(center.X), y1 = (int)Math.Round(center.Y);
            double r = Radius;

            double y = r;
            for (int x = 0; x < y; x++)
            {
                y = (double)Math.Sqrt(r * r - x * x);
                PutSemiCirclePixel(x, (int)y, x1, y1, direction, RgbValues, bmpData, 1 - y % 1);
                PutSemiCirclePixel(x, (int)y + 1, x1, y1, direction, RgbValues, bmpData, y % 1);
            }
        }

        public override IEnumerable<DrawingPoint> GetTranslationPoints()
        {
            return new List<DrawingPoint>() { Point1, Point2 };
        }

        public override DrawingPoint GetClosestPoint(Vector pos)
        {
            updateRadius();
            if (Point1.dist(pos) < Radius / 2 && Point1.dist(pos) < Point2.dist(pos))
            {
                return Point1;
            }
            else if(Point2.dist(pos) < Radius / 2)
            {
                return Point2;
            }
            else
            {
                Vector v2 = (pos - Point2.Point), v1 = (pos - Point1.Point);
                v2.Normalize(); v1.Normalize();

                if (Dir * v2 >= 0)
                    radiusUtilityPoint.Point = Point2.Point + v2 * Radius;
                else if (-Dir * v1 >= 0)
                    radiusUtilityPoint.Point = Point1.Point + v1 * Radius;
                else
                {
                    Vector orth = new Vector(-Dir.Y, Dir.X) * ((pos - Point2.Point) * new Vector(-Dir.Y, Dir.X));
                    orth.Normalize();
                    radiusUtilityPoint.Point = Point1.Point + Dir * ((pos - Point1.Point) * Dir) + orth * Radius;
                }
                return radiusUtilityPoint;
            }
        }
    }
}
