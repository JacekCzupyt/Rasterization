using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows;

namespace Rasterization.DrawingObjects
{
    [Serializable]
    class Polygon : AbstractDrawingObject, IHasThickness
    {
        public double Thickness { get { return Edges[0].Thickness; } set { Edges.ForEach(e => e.Thickness = value); } }

        public List<DrawingPoint> Points = new List<DrawingPoint>();
        protected List<IThickLine> Edges = new List<IThickLine>();

        public Polygon(Color color, double thick, Vector p0, params Vector[] list)
        {
            this.color = color;

            Points.Add(new DrawingPoint(p0));
            Points.AddRange(list.Select(p => new DrawingPoint(p)));

            InitializeEdges(thick);
        }

        public Polygon(Color color, double thick, DrawingPoint p0, params DrawingPoint[] list)
        {
            this.color = color;
            Points.Add(p0);
            Points.AddRange(list);

            InitializeEdges(thick);
        }

        protected Polygon(Color color, Vector p0, params Vector[] list)
        {
            this.color = color;

            Points.Add(new DrawingPoint(p0));
            Points.AddRange(list.Select(p => new DrawingPoint(p)));
        }

        protected Polygon(Color color, DrawingPoint p0, params DrawingPoint[] list)
        {
            this.color = color;
            Points.Add(p0);
            Points.AddRange(list);
        }

        protected virtual void InitializeEdges(double thick)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                Edges.Add(new ThickLine(Points[i], Points[(i + 1) % Points.Count], thick, color));
            }
        }

        public DrawingPoint AddPoint(Vector v)
        {
            return AddPoint(new DrawingPoint(v));
        }

        public virtual DrawingPoint AddPoint(DrawingPoint p)
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

        public override DrawingPoint GetClosestPoint(Vector pos)
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
