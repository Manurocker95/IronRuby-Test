#if USE_CUSTOM_PAINTER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Painter;

using Point = VirtualPhenix.Painter.VP_PaintSetup.Point;

namespace VirtualPhenix
{
    public static partial class VP_Extensions 
    {

        public static void FloodFillArea(this Texture2D aTex, ref Color32[] colors, Color32[] initColors, int aX, int aY, Color32 aFillColor)
        {
            int w = aTex.width;
            int h = aTex.height;
            Color refCol = colors[aX + aY * w];
          
            Queue<Point> nodes = new Queue<Point>();
            nodes.Enqueue(new Point(aX, aY));
            Color C;
            Color initColor;
            var BLACK_WHITE_POINT = VP_PaintSetup.BLACK_WHITE_POINT;

            while (nodes.Count > 0)
            {
                Point current = nodes.Dequeue();
                for (int i = current.x; i < w; i++)
                {
                    C = colors[i + current.y * w];
                    initColor = initColors[i + current.y * w];

  
                    //if ((C != refCol && initColor.r < 0.5f) || C == aFillColor)
                    if ((initColor.r < BLACK_WHITE_POINT && initColor.g < BLACK_WHITE_POINT && initColor.b < BLACK_WHITE_POINT) || C == aFillColor || (C != refCol && (refCol != Color.white || C.r != C.g)))
                        break;
                    colors[i + current.y * w] = aFillColor;

                    if (current.y + 1 < h)
                    {
                        C = colors[i + current.y * w + w];
                        initColor = initColors[i + current.y * w + w];

                        //if ((C == refCol || initColor.r > 0.5f) && C != aFillColor)
                        if ((C == refCol || (initColor.r > BLACK_WHITE_POINT && initColor.r == C.r)) && C != aFillColor)
                            nodes.Enqueue(new Point(i, current.y + 1));


                    }
                    if (current.y - 1 >= 0)
                    {
                        C = colors[i + current.y * w - w];
                        initColor = initColors[i + current.y * w - w];

                        if ((C == refCol || (initColor.r > BLACK_WHITE_POINT && initColor.r == C.r)) && C != aFillColor)
                            nodes.Enqueue(new Point(i, current.y - 1));
                    }
                }
                for (int i = current.x - 1; i >= 0; i--)
                {
                    C = colors[i + current.y * w];
                    initColor = initColors[i + current.y * w];
                    if (initColor.r < BLACK_WHITE_POINT || C == aFillColor || (C != refCol && (refCol != Color.white || C.r != C.g)))
                        break;
                    colors[i + current.y * w] = aFillColor;

                    if (current.y + 1 < h)
                    {
                        C = colors[i + current.y * w + w];
                        initColor = initColors[i + current.y * w + w];
                        if ((C == refCol || (initColor.r > BLACK_WHITE_POINT && initColor.r == C.r)) && C != aFillColor)
                            nodes.Enqueue(new Point(i, current.y + 1));
                    }
                    if (current.y - 1 >= 0)
                    {
                        C = colors[i + current.y * w - w];
                        initColor = initColors[i + current.y * w - w];
                        if ((C == refCol || (initColor.r > BLACK_WHITE_POINT && initColor.r == C.r)) && C != aFillColor)
                            nodes.Enqueue(new Point(i, current.y - 1));
                    }

                }
            }
            aTex.SetPixels32(colors);
            aTex.Apply();
        }


        public static void FloodFillBorder(this Texture2D aTex, ref Color32[] colors, int aX, int aY, Color32 aFillColor, Color32 aBorderColor)
        {
            int w = aTex.width;
            int h = aTex.height;
            byte[] checkedPixels = new byte[colors.Length];
            Color refCol = aBorderColor;
            Queue<Point> nodes = new Queue<Point>();
            nodes.Enqueue(new Point(aX, aY));
            while (nodes.Count > 0)
            {
                Point current = nodes.Dequeue();

                for (int i = current.x; i < w; i++)
                {
                    if (checkedPixels[i + current.y * w] > 0 || colors[i + current.y * w] == refCol)
                        break;
                    colors[i + current.y * w] = aFillColor;
                    checkedPixels[i + current.y * w] = 1;
                    if (current.y + 1 < h)
                    {
                        if (checkedPixels[i + current.y * w + w] == 0 && colors[i + current.y * w + w] != refCol)
                            nodes.Enqueue(new Point(i, current.y + 1));
                    }
                    if (current.y - 1 >= 0)
                    {
                        if (checkedPixels[i + current.y * w - w] == 0 && colors[i + current.y * w - w] != refCol)
                            nodes.Enqueue(new Point(i, current.y - 1));
                    }
                }
                for (int i = current.x - 1; i >= 0; i--)
                {
                    if (checkedPixels[i + current.y * w] > 0 || colors[i + current.y * w] == refCol)
                        break;
                    colors[i + current.y * w] = aFillColor;
                    checkedPixels[i + current.y * w] = 1;
                    if (current.y + 1 < h)
                    {
                        if (checkedPixels[i + current.y * w + w] == 0 && colors[i + current.y * w + w] != refCol)
                            nodes.Enqueue(new Point(i, current.y + 1));
                    }
                    if (current.y - 1 >= 0)
                    {
                        if (checkedPixels[i + current.y * w - w] == 0 && colors[i + current.y * w - w] != refCol)
                            nodes.Enqueue(new Point(i, current.y - 1));
                    }
                }
            }
            aTex.SetPixels32(colors);
        }

        public static void Drawing(this Texture2D texture, ref Color[] pixels, Vector2 from, Vector2 to, int brushSize, Color color)
        {
            if (from == Vector2.zero)
            {
                from = to;
            }

            float extent = (float)brushSize;
            float stY = Mathf.Clamp(Mathf.Min(from.y, to.y) - extent, 0, texture.height);
            float stX = Mathf.Clamp(Mathf.Min(from.x, to.x) - extent, 0, texture.width);
            float endY = Mathf.Clamp(Mathf.Max(from.y, to.y) + extent, 0, texture.height);
            float endX = Mathf.Clamp(Mathf.Max(from.x, to.x) + extent, 0, texture.width);

            Point point = new Point((int)stX, (int)stY);

            float lengthX = endX - stX;
            float lengthY = endY - stY;

            Color c;

            float sqrRad = extent * extent;
            float sqrRad2 = (extent + 1) * (extent + 1);
            Vector2 start = new Vector2(stX, stY);
            //Debug.Log (widthX + "   "+ widthY + "   "+ widthX*widthY);
            for (float y = 0; y < lengthY; y++)
            {
                for (float x = 0; x < lengthX; x++)
                {
                    Vector2 xandy = new Vector2(x, y);
                    Vector2 p = xandy + start;
                    Vector2 center = p + new Vector2(0.5f, 0.5f);
                    Vector2 nearest = center - (Vector2)VP_Utils.Painter.NearestPointStrict(from, to, center);
                    float dist = (center - nearest).sqrMagnitude;
                    if (dist > sqrRad2)
                    {
                        continue;
                    }
                    dist = VP_Utils.Painter.GaussFalloff(Mathf.Sqrt(dist), extent); //* hardness;
                                                                   //dist = (samples[i]-pos).sqrMagnitude;
                    if (dist > 0)
                    {
                        c = Color.Lerp(pixels[(int)y * (int)lengthX + (int)x], color, dist);
                    }
                    else
                    {
                        c = Color.Lerp(pixels[(int)y * (int)lengthX + (int)x], color, dist);
                    }

                    pixels[(int)y * (int)lengthX + (int)x] = c;

                }
            }

            texture.SetPixels((int)start.x, (int)start.y, (int)lengthX, (int)lengthY, pixels, 0);

            texture.Apply();
        }


        public static void Draw(this Texture2D aTex, ref Color32[] colors, Color32[] initColors, int aX, int aY, int brushSize, Color32 brushColor, BRUSH_TYPE brush, bool pencilMode = false, Vector2 previousPoint = default(Vector2), bool blockColor = false, Color32? c = null)
        {
            float t = Time.realtimeSinceStartup;

            Point point = new Point(aX, aY);

            if (previousPoint != Vector2.zero)
            {
                int dy = (int)(aY - previousPoint.y);
                int dx = (int)(aX - previousPoint.x);
                int stepx, stepy;

                int step = 1;//brushSize / 10 + 1;

                if (dy < 0)
                {
                    dy = -dy;
                    stepy = -step;
                }
                else
                    stepy = step;

                if (dx < 0)
                {
                    dx = -dx;
                    stepx = -step;
                }
                else
                    stepx = step;

                dy <<= 1;
                dx <<= 1;

                float fraction = 0;

                int x = (int)previousPoint.x;
                int y = (int)previousPoint.y;

                switch (brush)
                {
                    case BRUSH_TYPE.BRUSH:
                    case BRUSH_TYPE.PENCIL:
                        aTex.DrawPoint(ref colors, initColors, new Point(x, y), brushSize, brushColor, pencilMode, blockColor, c);
                        break;
                    case BRUSH_TYPE.PURPURIN:
                        aTex.DrawPurpurinaPoint(ref colors, initColors, new Point(x, y), brushSize, brushColor);
                        break;

                }

                if (dx > dy)
                {
                    fraction = dy - (dx >> 1);
                    while (Mathf.Abs(x - aX) > step)
                    {
                        if (fraction >= 0)
                        {
                            y += stepy;
                            fraction -= dx;
                        }
                        x += stepx;
                        fraction += dy;

                        switch (brush)
                        {
                            case BRUSH_TYPE.BRUSH:
                            case BRUSH_TYPE.PENCIL:
                                aTex.DrawPoint(ref colors, initColors, new Point(x, y), brushSize, brushColor, pencilMode, blockColor, c);
                                break;

                            case BRUSH_TYPE.PURPURIN:
                                aTex.DrawPurpurinaPoint(ref colors, initColors, new Point(x, y), brushSize, brushColor);
                                break;

                        }
                    }
                }
                else
                {
                    fraction = dx - (dy >> 1);
                    while (Mathf.Abs(y - aY) > step)
                    {
                        if (fraction >= 0)
                        {
                            x += stepx;
                            fraction -= dy;
                        }
                        y += stepy;
                        fraction += dx;

                        switch (brush)
                        {
                            case BRUSH_TYPE.BRUSH:
                            case BRUSH_TYPE.PENCIL:
                                aTex.DrawPoint(ref colors, initColors, new Point(x, y), brushSize, brushColor, pencilMode, blockColor, c);
                                break;
                            case BRUSH_TYPE.PURPURIN:
                                aTex.DrawPurpurinaPoint(ref colors, initColors, new Point(x, y), brushSize, brushColor);
                                break;
                        }
                    }
                }

            }
            else
            {
                switch (brush)
                {
                    case BRUSH_TYPE.BRUSH:
                    case BRUSH_TYPE.PENCIL:
                        aTex.DrawPoint(ref colors, initColors, point, brushSize, brushColor, pencilMode, blockColor, c);
                        break;
                    case BRUSH_TYPE.PURPURIN:
                        aTex.DrawPurpurinaPoint(ref colors, initColors, point, brushSize, brushColor);
                        break;
                }

            }

            aTex.SetPixels32(colors);
            aTex.Apply();

        }

        public static void DrawPoint(this Texture2D aTex, ref Color32[] colors, Color32[] initColors, Point point, int brushSize, Color32 brushColor, bool pencilMode = false, bool blockColor = false, Color32? c = null)
        {
            int r = brushSize >> 1;

            int px, nx, py, ny, d, pos;

            for (int x = 0; x <= r; x++)
            {
                d = (int)Mathf.Ceil(Mathf.Sqrt(r * r - x * x));
                for (int y = 0; y <= d; y++)
                {
                    px = point.x + x;
                    nx = point.x - x;
                    py = point.y + y;
                    ny = point.y - y;

                    pos = py * aTex.width + px;
                    if (pos > -1 && pos < colors.Length && px < aTex.width && px > -1 && /*initColors[pos].r > BLACK_WHITE_POINT &&*/ initColors[pos].a > 0)
                        if (!blockColor || (c != null && (blockColor && !initColors[pos].Equals(c))))
                        {
                            if (pencilMode)
                            {
                                int i = Random.Range(0, 15);
                                if (i == 0)
                                    colors[pos] = brushColor;
                            }
                            else
                                colors[pos] = brushColor;
                        }


                    pos = py * aTex.width + nx;
                    if (pos > -1 && pos < colors.Length && nx < aTex.width && nx > -1 && /*initColors[pos].r > BLACK_WHITE_POINT &&*/ initColors[pos].a > 0)
                        if (!blockColor || (c != null && (blockColor && !initColors[pos].Equals(c))))
                        {
                            if (pencilMode)
                            {
                                int i = Random.Range(0, 15);
                                if (i == 0)
                                    colors[pos] = brushColor;
                            }
                            else
                                colors[pos] = brushColor;
                        }

                    pos = ny * aTex.width + px;
                    if (pos > -1 && pos < colors.Length && px < aTex.width && px > -1 && /*initColors[pos].r > BLACK_WHITE_POINT &&*/ VP_Utils.Painter.PaintOnAlpha(initColors[pos]))
                        if (!blockColor || (c != null && (blockColor && !initColors[pos].Equals(c))))
                        {
                            if (pencilMode)
                            {
                                int i = Random.Range(0, 15);
                                if (i == 0)
                                    colors[pos] = brushColor;
                            }
                            else
                                colors[pos] = brushColor;
                        }

                    pos = ny * aTex.width + nx;
                    if (pos > -1 && pos < colors.Length && nx < aTex.width && nx > -1 && /*initColors[pos].r > BLACK_WHITE_POINT &&*/ VP_Utils.Painter.PaintOnAlpha(initColors[pos]))
                        if (!blockColor || (c != null && (blockColor && !initColors[pos].Equals(c))))
                        {
                            if (pencilMode)
                            {
                                int i = Random.Range(0, 15);
                                if (i == 0)
                                    colors[pos] = brushColor;
                            }
                            else
                                colors[pos] = brushColor;
                        }
                }
            }
        }

        public static void DrawPurpurinaPoint(this Texture2D aTex, ref Color32[] colors, Color32[] initColors, Point point, int brushSize, Color32 brushColor)
        {
            var BLACK_WHITE_POINT = VP_PaintSetup.BLACK_WHITE_POINT;
            int r = brushSize >> 1;

            int px, nx, py, ny, d, pos;

            for (int x = 0; x <= r; x++)
            {
                d = (int)Mathf.Ceil(Mathf.Sqrt(r * r - x * x));
                for (int y = 0; y <= d; y++)
                {
                    px = point.x + x;
                    nx = point.x - x;
                    py = point.y + y;
                    ny = point.y - y;

                    pos = py * aTex.width + px;
                    if (pos > -1 && pos < colors.Length && px < aTex.width && px > -1 && initColors[pos].r > BLACK_WHITE_POINT && initColors[pos].a > 0)
                    {
                        int i = Random.Range(0, 300);
                        if (i <= 10)
                            colors[pos] = brushColor;
                        else if (i > 295)
                        {
                            colors[pos] = Color.black;
                        }
                    }

                    pos = py * aTex.width + nx;
                    if (pos > -1 && pos < colors.Length && nx < aTex.width && nx > -1 && initColors[pos].r > BLACK_WHITE_POINT && initColors[pos].a > 0)
                    {
                        int i = Random.Range(0, 300);
                        if (i <= 10)
                            colors[pos] = brushColor;
                        else if (i > 295)
                        {
                            colors[pos] = Color.black;
                        }

                    }

                    pos = ny * aTex.width + px;
                    if (pos > -1 && pos < colors.Length && px < aTex.width && px > -1 && initColors[pos].r > BLACK_WHITE_POINT && initColors[pos].a > 0)
                    {
                        int i = Random.Range(0, 300);
                        if (i <= 10)
                            colors[pos] = brushColor;
                        else if (i > 295)
                        {
                            colors[pos] = Color.black;
                        }
                    }

                    pos = ny * aTex.width + nx;
                    if (pos > -1 && pos < colors.Length && nx < aTex.width && nx > -1 && initColors[pos].r > BLACK_WHITE_POINT && initColors[pos].a > 0)
                    {
                        int i = Random.Range(0, 300);
                        if (i <= 10)
                            colors[pos] = brushColor;
                        else if (i > 295)
                        {
                            colors[pos] = Color.black;
                        }
                    }
                }
            }
        }


    }
}
#endif