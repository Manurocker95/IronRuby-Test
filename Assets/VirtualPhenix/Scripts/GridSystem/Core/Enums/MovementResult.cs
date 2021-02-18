#if USE_GRID_SYSTEM
using System;

namespace VirtualPhenix.GridEngine
{
    public enum MovementResult
    {
        Moved,
        Turned,
        Cooldown,
        Failed,
        NoTileAtPosition
    }

}
#endif