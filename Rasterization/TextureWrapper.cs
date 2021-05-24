using Rasterization.DrawingObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rasterization
{
    [Serializable]
    class TextureWrapper
    {
        static int MathMod(int a, int b)
        {
            return (Math.Abs(a * b) + a) % b;
        }

        Bitmap bmp;
        [NonSerialized]
        BitmapData bmpData;
        [NonSerialized]
        byte[] rgbValues;

        public TextureWrapper(Bitmap _bmp)
        {
            bmp = _bmp;
            extract();
        }

        private void extract()
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            bmpData =
                bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * bmpData.Height;
            rgbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
        }

        private long Flatten((int, int) coords, int Width)
        {
            return (long)coords.Item1 + (long)coords.Item2 * Width;
        }

        public Color GetPixel(int x, int y, DrawingPoint p = null)
        {
            if (bmpData == null)
                extract();
            if (p != null)
            {
                x -= (int)p.X;
                y -= (int)p.Y;
            }
            x = MathMod(x, bmpData.Width);
            y = MathMod(y, bmpData.Height);
            long i = Flatten((x, y), bmpData.Width) * bmpData.Stride / bmpData.Width;
            return Color.FromArgb(rgbValues[i + 2], rgbValues[i + 1], rgbValues[i]);
        }
    }
}
