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
    class ClippedLine : IThickLine
    {
        public bool useTexture { get => mainLine.useTexture; set => mainLine.useTexture = value; }
        public TextureWrapper texture { get => mainLine.texture; set => mainLine.texture=value; }

        DrawingPoint point1, point2;
        public DrawingPoint Point1 { get { return point1; } set { point1 = value; if(l1 != null) l1.Point1 = value; } }
        public DrawingPoint Point2 { get { return point2; } set { point2 = value; if (l2 != null) l2.Point1 = value; } }

        public Color color { get => mainLine.color; set => mainLine.color = value; }

        List<DrawingRectangle> Clips;
        ThickLine mainLine, l1, l2;
        DrawingPoint cp1, cp2;

        public double Thickness { get { return mainLine.Thickness; } set { mainLine.Thickness = value; l1.Thickness = Thickness; l2.Thickness = Thickness; } }

        public ClippedLine(Vector p1, Vector p2, double thick, Color color, List<DrawingRectangle> ClipRectangles)
        {
            Clips = ClipRectangles;
            l1 = new ThickLine(p1, p1, thick, Color.Pink);
            l2 = new ThickLine(p2, p2, thick, Color.Pink);
            Point1 = l1.Point1;
            Point2 = l2.Point1;
            cp1 = l1.Point2;
            cp2 = l2.Point2;
            mainLine = new ThickLine(cp1, cp2, thick, color);
        }

        public ClippedLine(DrawingPoint p1, DrawingPoint p2, double thick, Color color, List<DrawingRectangle> ClipRectangles)
        {
            Clips = ClipRectangles;
            Point1 = p1;
            Point2 = p2;
            cp1 = new DrawingPoint(Point1.Point);
            cp2 = new DrawingPoint(Point2.Point);
            l1 = new ThickLine(Point1, cp1, thick, Color.Pink);
            l2 = new ThickLine(Point2, cp2, thick, Color.Pink);
            mainLine = new ThickLine(cp1, cp2, thick, color);
        }

        public void Draw(byte[] RgbValues, BitmapData bmpData, bool Antialiesing)
        {
            cp1.Point = Point1.Point;
            cp2.Point = Point2.Point;
            foreach(var rec in Clips)
            {
                if (!ClipToRectangle(cp1, cp2, new Clip(rec)))
                    break;
            }
            if (cp1.Point != Point1.Point)
                l1.Draw(RgbValues, bmpData, Antialiesing);
            if (cp2.Point != Point2.Point)
                l2.Draw(RgbValues, bmpData, Antialiesing);
            if (cp1.Point != cp2.Point)
                mainLine.Draw(RgbValues, bmpData, Antialiesing);
        }

        public struct Clip
        {
            public double right, left, up, down;
            public Clip(DrawingRectangle rec)
            {
                right = rec.Points.Aggregate((p1, p2) => p1.X > p2.X ? p1 : p2).X;
                left = rec.Points.Aggregate((p1, p2) => p1.X < p2.X ? p1 : p2).X;
                up = rec.Points.Aggregate((p1, p2) => p1.Y > p2.Y ? p1 : p2).Y;
                down = rec.Points.Aggregate((p1, p2) => p1.Y < p2.Y ? p1 : p2).Y;
            }
            public static Clip operator *(Clip a, Clip b)
            {
                Clip c = new Clip();
                c.up = Math.Min(a.up, b.up);
                c.down = Math.Max(a.down, b.down);
                c.right = Math.Min(a.right, b.right);
                c.left = Math.Max(a.left, b.left);
                return c;
            }
                
        }

        bool ClipToRectangle(DrawingPoint p1, DrawingPoint p2, Clip clip)
        {
            bool accept = false, done = false;
            byte outcode1 = ComputeOutcode(p1, clip);
            byte outcode2 = ComputeOutcode(p2, clip);
            do
            {
                if ((outcode1 | outcode2) == 0)
                { //trivially accepted
                    accept = true;
                    done = true;
                }
                else if ((outcode1 & outcode2) != 0)
                { //trivially rejected
                    p2.Point = p1.Point;
                    accept = false;
                    done = true;
                }
                else
                {
                    byte outcodeOut = (outcode1 != 0) ? outcode1 : outcode2;
                    Vector p = new Vector();

                    if ((outcodeOut & 1) != 0)//right
                    {
                        p.Y = p1.Y + (p2.Y - p1.Y) * (clip.right - p1.X) / (p2.X - p1.X);
                        p.X = clip.right;
                    }
                    else if ((outcodeOut & 2) != 0)//left
                    {
                        p.Y = p1.Y + (p2.Y - p1.Y) * (clip.left - p1.X) / (p2.X - p1.X);
                        p.X = clip.left;
                    }
                    else if ((outcodeOut & 4) != 0)//top
                    {
                        p.X = p1.X + (p2.X - p1.X) * (clip.up - p1.Y) / (p2.Y - p1.Y);
                        p.Y = clip.up;
                    }
                    else if ((outcodeOut & 8) != 0)//bottom
                    {
                        p.X = p1.X + (p2.X - p1.X) * (clip.down - p1.Y) / (p2.Y - p1.Y);
                        p.Y = clip.down;
                    }

                    if (outcodeOut == outcode1)
                    {
                        p1.Point = p;
                        outcode1 = ComputeOutcode(p1, clip);
                    }
                    else
                    {
                        p2.Point = p;
                        outcode2 = ComputeOutcode(p2, clip);
                    }

                }
            } while (!done);

            return accept;
        }

        byte ComputeOutcode(DrawingPoint p, Clip clip)
        {
            byte outcode = 0;
            outcode |= (p.X > clip.right) ? 1 : 0;
            outcode |= (p.X < clip.left) ? 2 : 0;
            outcode |= (p.Y > clip.up) ? 4 : 0;
            outcode |= (p.Y < clip.down) ? 8 : 0;
            return outcode;
        }

        public IEnumerable<DrawingPoint> GetTranslationPoints()
        {
            return new List<DrawingPoint>() { Point1, Point2 };
        }

        public DrawingPoint GetClosestPoint(Vector pos)
        {
            return Point1.dist(pos) < Point2.dist(pos) ? Point1 : Point2;
        }
    }
}
