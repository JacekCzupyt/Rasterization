using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;

namespace Rasterization.DrawingObjects
{
    [Serializable]
    class FilledPolygon : Polygon
    {
        public FilledPolygon(Color color, double thick, Vector p0, params Vector[] list) : base(color, thick, p0, list) { }

        public override void Draw(byte[] RgbValues, BitmapData bmpData, bool Antialiesing)
        {
            base.Draw(RgbValues, bmpData, Antialiesing);

            int Ymax = (int)Math.Round(Points.Select(p => p.Y).Max());
            int Ymin = (int)Math.Round(Points.Select(p => p.Y).Min());

            List<LinkedList<aetElem>> et = new List<LinkedList<aetElem>>(new LinkedList<aetElem>[Ymax - Ymin + 1]);
            for (int i = 0; i < et.Count; i++)
                et[i] = new LinkedList<aetElem>();

            for (int i = 0;i<Points.Count;i++)
            {
                DrawingPoint p1 = Points[i], p2 = Points[(i + 1) % Points.Count];
                aetElem elem = new aetElem(p1, p2);
                et[(int)Math.Round(Math.Min(p1.Y, p2.Y))-Ymin].AddLast(elem);
            }

            LinkedList<aetElem> aet = new LinkedList<aetElem>();

            int y = Ymin;
            while (y <= Ymax)
            {
                
                foreach (var e in et[y - Ymin])
                    aet.AddLast(e);

                for (var i = aet.First; i != null;)
                {
                    bool rm = i.Value.Ymax == y;
                    var k = i;
                    i = i.Next;
                    if (rm)
                        aet.Remove(k);
                }

                aet.OrderBy(e => e.x);

                for(var i = aet.First; i != null; i=i.Next.Next)
                {
                    for(int x = (int)Math.Ceiling(i.Value.x); x < (int)i.Next.Value.x; x++)
                    {
                        PutPixel(x, y, RgbValues, bmpData);
                    }
                    aetElem iv = i.Value;
                    iv.x += iv.mi;
                    i.Value = iv;

                    aetElem ivn = i.Next.Value;
                    ivn.x += ivn.mi;
                    i.Next.Value = ivn;
                }

                y++;
            }
        }

        struct aetElem
        {
            public aetElem(DrawingPoint p1, DrawingPoint p2)
            {
                Ymax = (int)Math.Round(Math.Max(p1.Y, p2.Y));
                x = p1.Y < p2.Y ? p1.X : p2.X;
                mi = (p2.X - p1.X) / (p2.Y - p1.Y);
            }

            public int Ymax;
            public double x;
            public double mi;
        }
    }
}
