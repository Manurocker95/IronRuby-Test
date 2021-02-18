#if USE_GRID_SYSTEM && USE_TILEMAP
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.Tilemaps
{
    public class VP_GridBrushEditorBase : GridBrushEditorBase
    {
        protected virtual void DrawButtonWithColor(string _text, UnityEngine.Events.UnityAction _callback, Color _color, Color _textColor)
        {
            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = _color;
            var style = new GUIStyle(GUI.skin.button);
            style.normal.textColor = _textColor;
            if (GUILayout.Button(_text, style))
            {
                _callback.Invoke();
            }
            GUI.backgroundColor = oldColor;
        }

    }
}
#endif