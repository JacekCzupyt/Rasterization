using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Input;
using Rasterization.DrawingObjects;
using System.Linq;

namespace Rasterization
{
    public partial class MainWindow : Window
    {
        List<IDrawingObject> DrawingObjects = new List<IDrawingObject>();
        IEnumerable<DrawingRectangle> ClipRectangles { get
            {
                return DrawingObjects.ConvertAll<DrawingRectangle>(o => o as DrawingRectangle).Where(o => o != null);
            } }

        IDrawingObject currentlyDrawnObject;
        DrawingPoint currentlyDrawnPoint;

        Dictionary<DrawingPoint, FilledCircle> selectedPoints = new Dictionary<DrawingPoint, FilledCircle>();

        Color CurrentColor = Color.Black;

        private void BeginDrawingObject(MouseButtonEventArgs e)
        {
            Vector mousePos = (Vector)e.GetPosition(MainImageContainer);
            switch (currentlyPressedButton.Name)
            {
                case "DrawLineButon":
                    ILine line = new ClippedLine((Vector)e.GetPosition(MainImageContainer), (Vector)e.GetPosition(MainImageContainer), 2, CurrentColor, ClipRectangles);
                    currentlyDrawnObject = line;
                    DrawingObjects.Add(currentlyDrawnObject);
                    currentlyDrawnPoint = line.Point2;
                    break;
                case "DrawCircleButton":
                    MidpointCircle circle = new MidpointCircle((Vector)e.GetPosition(MainImageContainer), (Vector)e.GetPosition(MainImageContainer), CurrentColor);
                    currentlyDrawnObject = circle;
                    DrawingObjects.Add(currentlyDrawnObject);
                    currentlyDrawnPoint = circle.radiusUtilityPoint;
                    break;
                case "DrawPolygonButton":
                    Polygon poly = new FilledPolygon(CurrentColor, 2, ClipRectangles, (Vector)e.GetPosition(MainImageContainer), (Vector)e.GetPosition(MainImageContainer));
                    currentlyDrawnObject = poly;
                    DrawingObjects.Add(currentlyDrawnObject);
                    currentlyDrawnPoint = poly.Points[1];
                    break;
                case "DrawCapsuleButton":
                    Capsule cap = new Capsule(mousePos, mousePos, 0, CurrentColor);
                    currentlyDrawnObject = cap;
                    DrawingObjects.Add(currentlyDrawnObject);
                    currentlyDrawnPoint = cap.Point2;
                    break;
                case "DrawRectangleButton":
                    DrawingRectangle rec = new DrawingRectangle(CurrentColor, 2, mousePos, mousePos);
                    currentlyDrawnObject = rec;
                    DrawingObjects.Add(currentlyDrawnObject);
                    currentlyDrawnPoint = rec.Points[2];
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void ContinueDrawingObject(MouseButtonEventArgs e)
        {
            Vector mousePos = (Vector)e.GetPosition(MainImageContainer);
            switch (currentlyPressedButton.Name)
            {
                case "DrawCircleButton": //line and circle are exactly the same
                case "DrawLineButon":
                    currentlyDrawnObject = null;
                    currentlyDrawnPoint.Point = mousePos;
                    currentlyDrawnPoint = null;
                    currentState = UIState.PreparingToDraw;
                    UpdateMainImage();
                    break;
                case "DrawPolygonButton":
                    Polygon poly = currentlyDrawnObject as Polygon;
                    if (poly.Points.Count > 2 && poly.Points[0].dist(mousePos) < MaximumSelectDistance)
                    {
                        currentlyDrawnObject = null;
                        currentlyDrawnPoint.Point = mousePos;
                        currentlyDrawnPoint = null;
                        currentState = UIState.PreparingToDraw;
                    }
                    else
                    {
                        currentlyDrawnPoint.Point = mousePos;
                        currentlyDrawnPoint = poly.AddPoint(mousePos);
                    }
                    UpdateMainImage();
                    break;
                case "DrawCapsuleButton":
                    Capsule cap = currentlyDrawnObject as Capsule;
                    
                    if (currentlyDrawnPoint == cap.Point2)
                    {
                        currentlyDrawnPoint.Point = mousePos;
                        currentlyDrawnPoint = cap.radiusUtilityPoint;
                    }
                    else
                    {
                        currentlyDrawnObject = null;
                        currentlyDrawnPoint.Point = mousePos;
                        currentlyDrawnPoint = null;
                        currentState = UIState.PreparingToDraw;
                        UpdateMainImage();
                    }
                    break;
                case "DrawRectangleButton":
                    currentlyDrawnObject = null;
                    currentlyDrawnPoint.Point = mousePos;
                    currentlyDrawnPoint = null;
                    currentState = UIState.PreparingToDraw;
                    UpdateMainImage();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void CancelDrawingObject()
        {
            if(currentlyDrawnObject != null)
            {
                if (currentlyDrawnObject is Polygon && (currentlyDrawnObject as Polygon).Points.Count > 2)
                {
                    currentlyDrawnPoint = null;
                    (currentlyDrawnObject as Polygon).RemoveLastPoint();
                    currentlyDrawnObject = null;
                }
                else
                {
                    DrawingObjects.Remove(currentlyDrawnObject);
                    currentlyDrawnObject = null;
                    currentlyDrawnPoint = null;
                }
                
                UpdateMainImage();
            }
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
                drawingObject.Draw(RgbValues, bmpData, Antialiesing);
            }

            foreach(FilledCircle uiCircle in selectedPoints.Values)
            {
                uiCircle.Draw(RgbValues, bmpData, true);
            }

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(RgbValues, 0, ptr, bytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            return bmp;
        }
    }
}
