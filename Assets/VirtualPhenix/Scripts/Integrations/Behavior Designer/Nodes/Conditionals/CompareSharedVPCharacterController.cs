#if USE_BEHAVIOR_DESIGNER

using BehaviorDesigner;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity;
using BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[TaskCategory(VP_PrefixSetup.MAIN_PREFIX+"/Characters/SharedVariable")]
	[TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
	public class CompareSharedVPCharacterController : Conditional
	{
		[Tooltip("The first variable to compare")]
		public SharedVPCharacterController variable;
		[Tooltip("The variable to compare to")]
		public SharedVPCharacterController compareTo;

		public override TaskStatus OnUpdate()
		{
			if (variable.Value == null && compareTo.Value != null)
				return TaskStatus.Failure;
			if (variable.Value == null && compareTo.Value == null)
				return TaskStatus.Success;

			return variable.Value.Equals(compareTo.Value) ? TaskStatus.Success : TaskStatus.Failure;
		}

		public override void OnReset()
		{
			variable = null;
			compareTo = null;
		}
	}
}
#endif