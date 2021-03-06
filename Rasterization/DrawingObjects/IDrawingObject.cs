using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;

namespace Rasterization.DrawingObjects
{
    /// <summary>
    /// Facilitates drawing algorithms
    /// </summary>
    interface IDrawingObject
    {
        /// Draws to the locked bitmap
        void Draw(byte[] RgbValues, BitmapData bmpData, bool Antialiesing = false);

        /// Returns the point belonging to the object closest to a given position
        DrawingPoint GetClosestPoint(Vector pos);

        /// Returns all points belonging to the object
        IEnumerable<DrawingPoint> GetTranslationPoints();

        /// Color of the object
        Color color { get; set; }

        bool useTexture { get; set; }

        TextureWrapper texture { get; set; }
    }

    interface IHasThickness : IDrawingObject
    {
        double Thickness { get; set; }
    }

    interface ILine : IDrawingObject
    {
        DrawingPoint Point1 { get; set; }
        DrawingPoint Point2 { get; set; }
    }

    interface IThickLine : ILine, IHasThickness { }
}
