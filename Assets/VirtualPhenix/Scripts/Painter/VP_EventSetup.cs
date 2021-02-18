#if USE_CUSTOM_PAINTER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    public static partial class VP_EventSetup
    {
        public static class Painter
        {
            public const string START_PAINT = "StartPaint";
            public const string END_PAINT = "EndPaint";
            public const string PAINTING = "Painting";
            public const string GET_PAINT_TEXTURE = "GetPaintTexture";
            public const string ASK_FOR_TEXTURE = "AskPaintTexture";
            public const string SET_FILL_COLOR = "SetFillColor";
        }
    }
}
#endif