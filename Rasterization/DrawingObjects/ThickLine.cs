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
            }
            
        }

        protected override void PutPixel(int x, int y, byte[] RgbValues, BitmapData bmpData, double modifier = 1)
        {
            Brush.Position.Point = new Vector2(x, y);
            Brush.Draw(RgbValues, bmpData, CurrentlyDrawingAntialiesed);
        }
    }
}
