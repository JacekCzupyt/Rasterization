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
    /* I originally thought that "ignore like thickness" was a method for drawing antialiased thick lines,
     * not that we can just draw thin lines instead. By the time I read your answer, I alrady implemented this
     * weird algorithm, which mostly works, so I may as well keep it here.
     * 
     * I'm not sure how Xiaolin Wu antialiesing is supposed to work on a brush type thick line
     * The task stated that the "thickness should be ignored", but I don't think that makes sense
     * I tried several methods that utilize the Xiaolin Wu in some way, the one that worked best,
     * implemented below, is drawing using a antialiased circular brush, and then drawing 2 antialiased
     * thin lines on the edges of the main thick line
     */

    class ThickLine : MidpointLine
    {
        private FilledCircle brush;
        public float Thickness { get { return brush.Radius; } set { brush.Radius = value < 0 ? 0 : value; } }

        public override Color color { get => base.color; set { if (brush != null) { brush.color = value; } base.color = value; }  }

        public ThickLine(Vector2 p1, Vector2 p2, float thick, Color color) : base(p1, p2, color)
        {
            brush = new FilledCircle(new Vector2(0, 0), thick, color);
        }

        public ThickLine(DrawingPoint p1, DrawingPoint p2, float thick, Color color) : base(p1, p2, color)
        {
            brush = new FilledCircle(new Vector2(0, 0), thick, color);
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
            brush.Position.Point = new Vector2(x, y);
            brush.Draw(RgbValues, bmpData, CurrentlyDrawingAntialiesed);
        }
    }
}
