using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;

namespace Rasterization.DrawingObjects
{
    /// Mostly implements some helper functions that most drawing algorithms will use
    [Serializable]
    abstract class AbstractDrawingObject : IDrawingObject
    {
        public bool useTexture { get; set; } = false;
        public TextureWrapper texture { get; set; } = null;
        protected virtual DrawingPoint textureOrigin { get => null;}
        public virtual Color color { get; set; }
        public abstract void Draw(byte[] RgbValues, BitmapData bmpData, bool Antialiesing);
        public abstract IEnumerable<DrawingPoint> GetTranslationPoints();
        public abstract DrawingPoint GetClosestPoint(Vector pos);

        protected long Flatten((int, int) coords, int Width)
        {
            return (long)coords.Item1 + (long)coords.Item2 * Width;
        }

        protected (int, int) Unflatten(long ind, int Width)
        {
            return ((int)(ind % Width), (int)(ind / Width));
        }

        protected (int, int) RoundToPixels(Vector point)
        {
            return ((int)Math.Round(point.X), (int)Math.Round(point.Y));
        }

        protected bool IsInBounds((int, int) coords, int Width, int Height)
        {
            return coords.Item1 >= 0 && coords.Item2 >= 0 && coords.Item1 < Width && coords.Item2 < Height;
        }

        protected void swap(ref int a, ref int b) { int tmp = a; a = b; b = tmp; }

        protected virtual void PutPixel(int x, int y, byte[] RgbValues, BitmapData bmpData, double modifier = 1)
        {
            Color c;
            if(useTexture && texture != null)
                c = texture.GetPixel(x, y, textureOrigin);
            else
                c = color;
            if(IsInBounds((x, y), bmpData.Width, bmpData.Height))
            {
                long i = Flatten((x, y), bmpData.Width) * bmpData.Stride / bmpData.Width;
                RgbValues[i] = (byte)(c.B * modifier + RgbValues[i] * (1 - modifier));
                RgbValues[i + 1] = (byte)(c.G * modifier + RgbValues[i+1] * (1 - modifier));
                RgbValues[i + 2] = (byte)(c.R * modifier + RgbValues[i+2] * (1 - modifier));
            }
        }

        protected Color GetPixel(int x, int y, byte[] RgbValues, BitmapData bmpData)
        {
            if (IsInBounds((x, y), bmpData.Width, bmpData.Height))
            {
                long i = Flatten((x, y), bmpData.Width) * bmpData.Stride / bmpData.Width;
                return Color.FromArgb(RgbValues[i + 2], RgbValues[i + 1], RgbValues[i]);
            }
            else
                throw new InvalidOperationException();
        }

        protected void modPutPixel(int _x, int _y, int x0, int y0, int c0, byte[] RgbValues, BitmapData bmpData, double mod = 1)
        {
            if (c0 % 2 >= 1)
                swap(ref _x, ref _y);
            if (c0 % 4 >= 2)
                _x = -_x;
            if (c0 % 8 >= 4)
                _y = -_y;
            PutPixel(x0 + _x, y0 + _y, RgbValues, bmpData, mod);
        }
    }
}
