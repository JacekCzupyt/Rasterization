using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;

namespace Rasterization.DrawingObjects
{
    [Serializable]
    class ClippedPolygon : Polygon
    {
        IEnumerable<DrawingRectangle> Clips;

        public ClippedPolygon(Color color, double thick, Vector p0, IEnumerable<DrawingRectangle> Clips) :
            base(color, thick, p0)
        {
            this.Clips = Clips;
        }

        public ClippedPolygon(Color color, double thick, DrawingPoint p0, IEnumerable<DrawingRectangle> Clips) :
            base(color, thick, p0)
        {
            this.Clips = Clips;
        }

        protected override void InitializeEdges(double thick)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                Edges.Add(new ClippedLine(Points[i], Points[(i + 1) % Points.Count], thick, color, Clips));
            }
        }

        public override DrawingPoint AddPoint(DrawingPoint p)
        {
            Points.Add(p);
            Edges.Last().Point2 = p;
            Edges.Add(new ClippedLine(p, Points[0], Thickness, color, Clips));
            return p;
        }

    }
}
