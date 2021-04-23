using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Rasterization.DrawingObjects
{
    /* I'm not sure how Xiaolin Wu antialiesing is supposed to work on a brush type thick line
     * The task stated that the "thickness should be ignored", but I don't think that makes sense
     * I tried several methods that utilize the Xiaolin Wu in some way, the one that worked best,
     * implemented below, is drawing using a antialiased circular brush, and then drawing 2 antialiased
     * thin lines on the edges of the main thick line
     */

    class ThickLine : MidpointLine
    {
        private FilledCircle Brush;
        public float Thickness { get { return Brush.Radius; } set { Brush.Radius = value; } }

        public ThickLine(Vector2 p1, Vector2 p2, float thick, Color color) : base(p1, p2, color)
        {
            Brush = new FilledCircle(new Vector2(0, 0), thick, color);
        }

        public ThickLine(DrawingPoint p1, DrawingPoint p2, int thick, Color color) : base(p1, p2, color)
        {
            Brush = new FilledCircle(new Vector2(0, 0), thick, color);
        }

        bool CurrentlyDrawingAntialiesed;

        public override void Draw(byte[] RgbValues, BitmapData bmpData, bool Antialiesing)
        {
            if (Thickness == 0)
                new MidpointLine(Point1, Point2, color).Draw(RgbValues, bmpData, Antialiesing);
            else
            {
                CurrentlyDrawingAntialiesed = Antialiesing;
                DrawSimple(RgbValues, bmpData);
                if (Antialiesing)
                {
                    //this draws 2 antialiesed thin lines on the edges of the the main thick line
                    Vector2 dir = Point2.Point - Point1.Point;
                    dir /= dir.Length();
                    Vector2 orthogonal = new Vector2(-dir.Y, dir.X);
                    new MidpointLine(Point1.Point + orthogonal * Thickness, Point2.Point + orthogonal * Thickness, color).Draw(RgbValues, bmpData, Antialiesing);
                    new MidpointLine(Point1.Point - orthogonal * Thickness, Point2.Point - orthogonal * Thickness, color).Draw(RgbValues, bmpData, Antialiesing);
                }
            }
        }

        protected override void PutPixel(int x, int y, byte[] RgbValues, BitmapData bmpData, double modifier = 1)
        {
            Brush.Position.Point = new Vector2(x, y);
            Brush.Draw(RgbValues, bmpData, CurrentlyDrawingAntialiesed);
        }
    }
}
