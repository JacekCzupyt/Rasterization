using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Rasterization.DrawingObjects
{
    [Serializable]
    class DrawingPoint : INotifyPropertyChanged
    {
        public DrawingPoint(Vector2 p) { Point = p; }
        public static explicit operator DrawingPoint(Vector2 p) { return new DrawingPoint(p); }

        private Vector2 point;
        public Vector2 Point
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

        public float X { get { return Point.X; }}
        public float Y { get { return Point.Y; }}

        public float dist(Vector2 pos) { return (pos - Point).Length(); }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
