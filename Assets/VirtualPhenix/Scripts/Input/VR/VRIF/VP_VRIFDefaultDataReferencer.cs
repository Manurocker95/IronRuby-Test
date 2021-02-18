using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.ResourceReference;
using VirtualPhenix.Inputs.VR;

namespace VirtualPhenix.Integrations.Inputs.VR
{
#if USE_VRIF
    [CreateAssetMenu(fileName = "VRIF Input Default Data", menuName = "Virtual Phenix/Resource Dictionary/Inputs/VR/VRIF Input Action Default Data", order = 1)]
    public class VP_VRIFDefaultDataReferencer : VP_ResourceReferencer<string, VP_VRIFInputKeyData>
    {

    }
#endif
}
