﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Rasterization.DrawingObjects
{
    class Polygon : AbstractDrawingObject
    {
        public List<DrawingPoint> Points = new List<DrawingPoint>();
        List<MidpointLine> Edges = new List<MidpointLine>();

        public Polygon(Color color, Vector2 p0, params Vector2[] list)
        {
            this.color = color;

            Points.Add(new DrawingPoint(p0));
            Points.AddRange(list.Select(p => new DrawingPoint(p)));

            InitializeEdges();
        }

        public Polygon(Color color, DrawingPoint p0, params DrawingPoint[] list)
        {
            this.color = color;
            Points.Add(p0);
            Points.AddRange(list);

            InitializeEdges();
        }

        private void InitializeEdges()
        {
            for (int i = 0; i < Points.Count; i++)
            {
                Edges.Add(new MidpointLine(Points[i], Points[(i + 1) % Points.Count], color));
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
            Edges.Add(new MidpointLine(p, Points[0], color));
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

        public override void Draw(byte[] RgbValues, int stride, int width, int height, bool Antialiesing)
        {
            foreach (var e in Edges)
                e.Draw(RgbValues, stride, width, height, Antialiesing);
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