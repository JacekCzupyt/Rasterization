using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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

        public double X { get { return Point.X; }}
        public double Y { get { return Point.Y; }}

        public double dist(Vector pos) { return (pos - Point).Length; }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
