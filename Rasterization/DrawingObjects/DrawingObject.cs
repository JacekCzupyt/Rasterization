using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Rasterization.DrawingObjects
{
    interface IDrawingObject
    {
        byte[] Draw(byte[] RgbValues, int stride, int width, int height);

        Vector2 GetClosestPoint(Vector2 pos);
        IEnumerable<Vector2> GetAllPoints();]
        Color color { get; set; }
    }
}
