using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    public interface VP_IPhysicsMovable
    {
        Vector3 GetDesiredMovementVector();

        Rigidbody GetRigidbody();
    }
}