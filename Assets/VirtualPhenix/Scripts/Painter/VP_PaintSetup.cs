#if USE_CUSTOM_PAINTER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Painter
{
    public static class VP_PaintSetup
    {
        public struct Point
        {
            public short x;
            public short y;
            public Point(short aX, short aY) { x = aX; y = aY; }
            public Point(int aX, int aY) : this((short)aX, (short)aY) { }
        }

        public const float BLACK_WHITE_POINT = 0.5f;
        public const bool CAN_PAINT_ON_ALPHA = false;
        public const BRUSH_TYPE DEFAULT_BRUSH_TYPE = BRUSH_TYPE.BRUSH;
        public const int DEFAULT_BRUSH_SIZE = 14;
    }
}
#endif