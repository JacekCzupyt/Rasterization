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
    [Serializable]
    class Polygon : AbstractDrawingObject
    {
        public float Thickness { get { return Edges[0].Thickness; } set { Edges.ForEach(e => e.Thickness = value); } }

        public List<DrawingPoint> Points = new List<DrawingPoint>();
        List<ThickLine> Edges = new List<ThickLine>();

        public Polygon(Color color, float thick, Vector2 p0, params Vector2[] list)
        {
            this.color = color;

            Points.Add(new DrawingPoint(p0));
            Points.AddRange(list.Select(p => new DrawingPoint(p)));

            InitializeEdges(thick);
        }

        public Polygon(Color color, float thick, DrawingPoint p0, params DrawingPoint[] list)
        {
            this.color = color;
            Points.Add(p0);
            Points.AddRange(list);

            InitializeEdges(thick);
        }

        private void InitializeEdges(float thick)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                Edges.Add(new ThickLine(Points[i], Points[(i + 1) % Points.Count], thick, color));
            }
        }

        public DrawingPoint AddPoint(Vector2 v)
        {
            return AddPoint(new DrawingPoint(v));
        }

        public DrawingPoint AddPoint(DrawingPoint p)
        {
            Points.Add(p);
            Edges.Last().Point2 = p;
            Edges.Add(new ThickLine(p, Points[0], Thickness, color));
            return p;
        }

        public void RemoveLastPoint()
        {
            if (Points.Count <= 2)
                throw new InvalidOperationException("Can't remove points from polygon if there's only 2 left");
            Points.RemoveAt(Points.Count - 1);
            Edges.RemoveAt(Edges.Count - 1);
            Edges.Last().Point2 = Points[0];
        }

        public override void Draw(byte[] RgbValues, BitmapData bmpData, bool Antialiesing)
        {
            foreach (var e in Edges)
                e.Draw(RgbValues, bmpData, Antialiesing);
        }

        public override DrawingPoint GetClosestPoint(Vector2 pos)
        {
            return Points.Aggregate((min, p) => min.dist(pos) < p.dist(pos) ? min : p);
        }

        public override IEnumerable<DrawingPoint> GetTranslationPoints()
        {
            return Points;
        }

        public override Color color { get => base.color; set
            {
                base.color = value;
                foreach (var e in Edges)
                    e.color = value;
            } }
    }
}
