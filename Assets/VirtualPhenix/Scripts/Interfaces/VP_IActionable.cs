using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    /// <summary>
    /// VP_IActionable inherited scripts can be interacted by the player
    /// </summary>
    public interface VP_IActionable
    {
        bool CanBeInteracted();
        void SetCanBeInteracted(bool _canBeInteracted);

        void OnAction();
        void OnAction(Transform trs);
    }
}
