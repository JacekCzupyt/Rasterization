using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Rasterization
{
    public static class ExtensionMethods
    {
        public static Vector2 ToVector2(this System.Windows.Point point)
        {
            return new Vector2((float)point.X, (float)point.Y);
        }
    }
}
