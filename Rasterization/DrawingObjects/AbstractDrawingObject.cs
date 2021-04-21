using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Rasterization.DrawingObjects
{
    /// Mostly implements some helper functions that most drawing algorithms will use
    abstract class AbstractDrawingObject : IDrawingObject
    {
        public Color color { get; set; }
        public abstract void Draw(byte[] RgbValues, int stride, int width, int height);
        public abstract IEnumerable<Vector2> GetAllPoints();
        public abstract Vector2 GetClosestPoint(Vector2 pos);

        protected int Flatten((int, int) coords, int Width)
        {
            return coords.Item1 + coords.Item2 * Width;
        }

        protected (int, int) Unflatten(int ind, int Width)
        {
            return (ind % Width, ind / Width);
        }

        protected (int, int) RoundToPixels(Vector2 point)
        {
            return ((int)Math.Round(point.X), (int)Math.Round(point.Y));
        }

        protected bool IsInBounds((int, int) coords, int Width, int Height)
        {
            return coords.Item1 >= 0 && coords.Item2 >= 0 && coords.Item1 < Width && coords.Item2 < Height;
        }

        protected void PutPixel(int x, int y, byte[] RgbValues, int stride, int width, int height, double modifier = 1)
        {
            int i = Flatten((x, y), width) * stride / width;
            RgbValues[i] = (byte)(color.R * modifier);
            RgbValues[i + 1] = (byte)(color.G * modifier);
            RgbValues[i + 2] = (byte)(color.B * modifier);
        }
    }
}
