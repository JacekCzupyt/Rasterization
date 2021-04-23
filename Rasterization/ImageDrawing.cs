﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Rasterization.DrawingObjects;

namespace Rasterization
{
    public partial class MainWindow : Window
    {
        List<IDrawingObject> DrawingObjects = new List<IDrawingObject>();

        IDrawingObject currentlyDrawnObject;
        DrawingPoint currentlyDrawnPoint;

        Dictionary<DrawingPoint, FilledCircle> selectedPoints = new Dictionary<DrawingPoint, FilledCircle>();

        private void BeginDrawingObject(MouseButtonEventArgs e)
        {
            switch (currentlyPressedButton.Name)
            {
                case "DrawLineButon":
                    MidpointLine line = new MidpointLine(e.GetPosition(MainImageContainer).ToVector2(), e.GetPosition(MainImageContainer).ToVector2(), Color.Black);
                    currentlyDrawnObject = line;
                    DrawingObjects.Add(currentlyDrawnObject);
                    currentlyDrawnPoint = line.Point2;
                    break;
                case "DrawCircleButton":
                    MidpointCircle circle = new MidpointCircle(e.GetPosition(MainImageContainer).ToVector2(), e.GetPosition(MainImageContainer).ToVector2(), Color.Black);
                    currentlyDrawnObject = circle;
                    DrawingObjects.Add(currentlyDrawnObject);
                    currentlyDrawnPoint = circle.radiusUtilityPoint;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void ContinueDrawingObject(MouseButtonEventArgs e)
        {
            switch (currentlyPressedButton.Name)
            {
                case "DrawCircleButton": //line and circle are exactly the same
                case "DrawLineButon":
                    currentlyDrawnObject = null;
                    currentlyDrawnPoint.Point = e.GetPosition(MainImageContainer).ToVector2();
                    currentlyDrawnPoint = null;
                    currentState = UIState.PreparingToDraw;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void CancelDrawingObject()
        {
            DrawingObjects.Remove(currentlyDrawnObject);
            currentlyDrawnObject = null;
            currentlyDrawnPoint = null;
            UpdateMainImage();
        }

        void UpdateMainImage()
        {
            mainBitmap = DrawBitmap(mainBitmap);

            MainImageContainer.Fill = new System.Windows.Media.ImageBrush(BitmapToImageSource(mainBitmap));
        }

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


            foreach(IDrawingObject drawingObject in DrawingObjects)
            {
                drawingObject.Draw(RgbValues, bmpData.Stride, bmpData.Width, bmpData.Height, true);
            }

            foreach(FilledCircle uiCircle in selectedPoints.Values)
            {
                uiCircle.Draw(RgbValues, bmpData.Stride, bmpData.Width, bmpData.Height, false);
            }

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(RgbValues, 0, ptr, bytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            return bmp;
        }
    }
}
