using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Environment;

namespace VirtualPhenix
{
    public interface VP_IFloatable
    {
        void RegisterWaterRegion(VP_WaterRegion _waterRegion);
        void UnRegisterWaterRegion(VP_WaterRegion _waterRegion);
    }
}