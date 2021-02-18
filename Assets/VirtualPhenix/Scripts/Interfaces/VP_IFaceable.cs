using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    /// <summary>
    /// All scripts that inherit from VP_IFaceable can be looked at by VP_LookAnimator scripts
    /// </summary>
    public interface VP_IFaceable
    {
        bool CanBeFaced();
        void SetCanBeFaced(bool _canBeFaced);

        /// <summary>
        /// Face position related
        /// </summary>
        /// <returns></returns>
        Vector3 GetFace();
        void SetFace(Transform _newFace);
        void SetFacePosition(Vector3 _newFacePoint); 
    }
}