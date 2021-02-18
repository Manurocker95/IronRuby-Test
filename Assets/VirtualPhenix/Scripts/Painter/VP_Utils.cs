#if USE_CUSTOM_PAINTER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Painter;

namespace VirtualPhenix
{
    public static partial class VP_Utils
    {
        public static class Painter
        {
            public static bool PaintOnAlpha(Color color)
            {
                if (color.a > 0 || VP_PaintSetup.CAN_PAINT_ON_ALPHA)
                    return true;

                return false;
            }

            public static float GaussFalloff(float distance, float inRadius)
            {
                return Mathf.Clamp01(Mathf.Pow(360.0f, -Mathf.Pow(distance / inRadius, 2.5f) - 0.01f));
            }

            public static Vector3 NearestPointStrict(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
            {
                Vector3 fullDirection = lineEnd - lineStart;
                Vector3 lineDirection = Vector3.Normalize(fullDirection);
                float closestPoint = Vector3.Dot((point - lineStart), lineDirection) / Vector3.Dot(lineDirection, lineDirection);
                return lineStart + (Mathf.Clamp(closestPoint, 0.0f, Vector3.Magnitude(fullDirection)) * lineDirection);
            }
        }
    }
}
#endif