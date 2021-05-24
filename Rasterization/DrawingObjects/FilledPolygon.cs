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
    class FilledPolygon : ClippedPolygon
    {
        public FilledPolygon(Color color, double thick, IEnumerable<DrawingRectangle> Clips, Vector p0, params Vector[] list) : base(color, thick, Clips, p0, list)
        {
            Fill = true;
        }
        
        public bool Fill { get; set; }

        public override void Draw(byte[] RgbValues, BitmapData bmpData, bool Antialiesing)
        {
            base.Draw(RgbValues, bmpData, Antialiesing);

            if (!Fill)
                return;

            ClippedLine.Clip clip;

            if (Clips == null || Clips.Count() == 0)
                clip = new ClippedLine.Clip() {
                    down = int.MinValue,
                    up = int.MaxValue,
                    left = int.MinValue,
                    right = int.MaxValue };
            else
                clip = Clips.Select(rec => new ClippedLine.Clip(rec)).Aggregate((a, b) => a * b);

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
            while (y <= Ymax && y<=clip.up)
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

                if (y >= clip.down)
                {
                    for (var i = aet.OrderBy(e => e.x).GetEnumerator(); i.MoveNext();)
                    {
                        var i1 = i.Current;
                        i.MoveNext();
                        var i2 = i.Current;
                        for (int x = Math.Max((int)Math.Ceiling(i1.x), (int)Math.Ceiling(clip.left));
                            x <= Math.Min((int)i2.x, (int)clip.right); 
                            x++)
                        {
                            PutPixel(x, y, RgbValues, bmpData);
                        }
                    }
                }

                foreach(var e in aet)
                {
                    e.x += e.mi;
                }

                y++;
            }
        }

        class aetElem
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
