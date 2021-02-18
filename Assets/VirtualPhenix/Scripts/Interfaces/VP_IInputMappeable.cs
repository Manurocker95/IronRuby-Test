using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    public interface VP_IInputMappeable
    {
        bool HasChanged();
        bool IsPressed();
        bool WasPressed();
        bool WasReleased();
    }
}