#if USE_BEHAVIOR_DESIGNER
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[TaskDescription("Can See Target.")]
	[TaskCategory(VP_PrefixSetup.MAIN_PREFIX+"/Misc")]
	[TaskIcon("Assets/VirtualPhenix/Editor/Icons/Behavior Designer/{SkinColor}CanSeeObjectIcon.png")]
    public class CanSeePlayer : Conditional
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The object that we are searching for")]
        public SharedGameObject targetObject;
        
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Detection Point")]
        public SharedTransform detectionPoint;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("The field of view angle of the agent (in degrees)")]
        public SharedFloat fieldOfViewAngle = 90;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("The distance that the agent can see")]
        public SharedFloat viewDistance = 1000;


        [BehaviorDesigner.Runtime.Tasks.Tooltip("If the target must be directly seen in front ")]
        public SharedBool needToBeInFront;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("The object that is within sight")]
        public SharedGameObject returnedObject;


        public override void OnStart()
        {
            base.OnStart();
            
        }

        /// <summary>
        /// Returns success if an object was found otherwise failure
        /// </summary>
        /// <returns></returns>
        public override TaskStatus OnUpdate()
        {
            if (detectionPoint == null)
                detectionPoint.Value = this.transform;

            returnedObject.Value = WithinSight(targetObject.Value, fieldOfViewAngle.Value, viewDistance.Value);
            if (returnedObject.Value != null)
            {
                // Return success if an object was found
                return TaskStatus.Success;
            }
            // An object is not within sight so return failure
            return TaskStatus.Failure;
        }

        /// <summary>
        /// Determines if the targetObject is within sight of the transform.
        /// </summary>
	    protected virtual GameObject WithinSight(GameObject targetObject, float fieldOfViewAngle, float viewDistance)
        {
            if (targetObject == null)
            {
                return null;
            }
            
            Debug.DrawRay(detectionPoint.Value.position, detectionPoint.Value.TransformDirection(Vector3.forward) * viewDistance, Color.blue);

            var direction = targetObject.transform.position - detectionPoint.Value.position;
            direction.y = 0;
            var angle = Vector3.Angle(direction, transform.forward);
            if (direction.magnitude < viewDistance && angle < fieldOfViewAngle * 0.5f)
            {
                if (needToBeInFront.Value)
                {                
                    // The hit agent needs to be within view of the current agent
                    if (LineOfSight(targetObject))
                    {
                        return targetObject; // return the target object meaning it is within sight
                    }
                }
                else
                {
                    return targetObject; // return the target object meaning it is within sight
                }
            }
            return null;
        }

        /// <summary>
        /// Returns true if the target object is within the line of sight.
        /// </summary>
	    protected virtual bool LineOfSight(GameObject targetObject)
        {
            RaycastHit hit;
            if (Physics.Linecast(detectionPoint.Value.position, targetObject.transform.position, out hit))//if (Physics.Raycast(detectionPoint.Value.position, detectionPoint.Value.TransformDirection(Vector3.forward), out hit, viewDistance.Value))
            {
                return (hit.transform.gameObject == targetObject);
            }
            return false;
        }

        /// <summary>
        /// Draws the line of sight representation
        /// </summary>
        public override void OnDrawGizmos()
        {
#if UNITY_EDITOR
            var oldColor = UnityEditor.Handles.color;
            var color = Color.yellow;
            color.a = 0.1f;
            UnityEditor.Handles.color = color;

            var halfFOV = fieldOfViewAngle.Value * 0.5f;
            var beginDirection = Quaternion.AngleAxis(-halfFOV, Vector3.up) * Owner.transform.forward;
            UnityEditor.Handles.DrawSolidArc(Owner.transform.position, Owner.transform.up, beginDirection, fieldOfViewAngle.Value, viewDistance.Value);

            UnityEditor.Handles.color = oldColor;
#endif
        }
    }
}
#endif