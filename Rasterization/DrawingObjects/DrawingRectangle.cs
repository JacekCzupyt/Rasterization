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
    class DrawingRectangle : AbstractDrawingObject, IHasThickness
    {
        public double Thickness { get { return Edges[0].Thickness; } set { Edges.ForEach(e => e.Thickness = value); } }

        public List<DrawingPoint> Points = new List<DrawingPoint>();
        List<ThickLine> Edges = new List<ThickLine>();

        public DrawingRectangle(Color color, double thick, Vector p0, Vector p1)
        {
            this.color = color;

            Points.Add(new DrawingPoint(p0));
            Points.Add(new DrawingPoint(new Vector(p0.X, p1.Y)));
            Points.Add(new DrawingPoint(p1));
            Points.Add(new DrawingPoint(new Vector(p1.X, p0.Y)));

            foreach (var p in Points)
                p.PropertyChanged += Position_PropertyChanged;

            InitializeEdges(thick);
        }

        public DrawingRectangle(Color color, double thick, DrawingPoint p0, DrawingPoint p1)
        {
            this.color = color;

            Points.Add(p0);
            Points.Add(new DrawingPoint(new Vector(p0.X, p1.Y)));
            Points.Add(p1);
            Points.Add(new DrawingPoint(new Vector(p1.X, p0.Y)));

            foreach (var p in Points)
                p.PropertyChanged += Position_PropertyChanged;

            InitializeEdges(thick);
        }

        private void Position_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!(sender is DrawingPoint))
                throw new ArgumentException("Sender should be drawing point");
            
            DrawingPoint p = sender as DrawingPoint;
            int index = Points.IndexOf(p);
            int d = index % 2 == 0 ? 1 : -1;
            if (Points[(index + d + Points.Count) % Points.Count].X != p.X)
                Points[(index + d + Points.Count) % Points.Count].X = p.X;
            if (Points[(index - d + Points.Count) % Points.Count].Y != p.Y)
                Points[(index - d + Points.Count) % Points.Count].Y = p.Y;
        }

        private void InitializeEdges(double thick)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                Edges.Add(new ThickLine(Points[i], Points[(i + 1) % Points.Count], thick, color));
            }
        }

        public override void Draw(byte[] RgbValues, BitmapData bmpData, bool Antialiesing)
        {
            foreach (var e in Edges)
                e.Draw(RgbValues, bmpData, Antialiesing);
        }

        public override IEnumerable<DrawingPoint> GetTranslationPoints()
        {
            return new List<DrawingPoint>() { Points[0], Points[2] };
        }

        public override DrawingPoint GetClosestPoint(Vector pos)
        {
            return Points.Aggregate((min, p) => min.dist(pos) < p.dist(pos) ? min : p);
        }

        public override Color color
        {
            get => base.color; set
            {
                base.color = value;
                foreach (var e in Edges)
                    e.color = value;
            }
        }
    }
}
