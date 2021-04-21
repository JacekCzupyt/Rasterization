using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Rasterization.DrawingObjects;

namespace Rasterization
{
    public partial class MainWindow : Window
    {
        MidpointLine mainLine;
        MidpointCircle mainCircle;

        Bitmap DrawBitmap(Bitmap bmp)
        {
            using (Graphics gfx = Graphics.FromImage(bmp))
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 255, 255)))
            {
                gfx.FillRectangle(brush, 0, 0, bmp.Width, bmp.Height);
            }

            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] RgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, RgbValues, 0, bytes);


            // ---------- Temporary -------------
            if (mainCircle != null)
                mainCircle.Draw(RgbValues, bmpData.Stride, bmpData.Width, bmpData.Height);
            // ----------------------------------

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(RgbValues, 0, ptr, bytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            return bmp;
        }
    }
}
