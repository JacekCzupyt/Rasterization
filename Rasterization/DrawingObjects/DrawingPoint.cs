using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Rasterization.DrawingObjects
{
    [Serializable]
    class DrawingPoint : INotifyPropertyChanged
    {
        public DrawingPoint(Vector p) { Point = p; }
        public static explicit operator DrawingPoint(Vector p) { return new DrawingPoint(p); }

        private Vector point;
        public Vector Point
        {
            get
            {
                return point;
            }
            set
            {
                point = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public double X { get { return Point.X; } set { Point = new Vector(value, Point.Y); } }
        public double Y { get { return Point.Y; } set { Point = new Vector(Point.X, value); } }

        public double dist(Vector pos) { return (pos - Point).Length; }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public override string ToString()
        {
            return Point.ToString();
        }
    }
}
