using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Rasterization.DrawingObjects
{
    /// <summary>
    /// Facilitates drawing algorithms
    /// </summary>
    interface IDrawingObject
    {
        /// Draws to the locked bitmap
        void Draw(byte[] RgbValues, int stride, int width, int height);

        /// Returns the point belonging to the object closest to a given position
        DrawingPoint GetClosestPoint(Vector2 pos);
        
        /// Returns all points belonging to the object
        IEnumerable<DrawingPoint> GetTranslationPoints();

        /// Color of the object
        Color color { get; set; }
    }
}
