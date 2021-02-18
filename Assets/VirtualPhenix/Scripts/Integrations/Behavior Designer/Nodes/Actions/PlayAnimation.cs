#if USE_BEHAVIOR_DESIGNER
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using VirtualPhenix.Integrations.BehaviorDesignerTree;
using VirtualPhenix;
using VirtualPhenix.Controllers;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[TaskDescription("Play Animation in VP_Animator")]
	[TaskCategory(VP_PrefixSetup.MAIN_PREFIX+"/Animation")]
	[TaskIcon("Assets/VirtualPhenix/Editor/Icons/Behavior Designer/PlayAnimationIcon.png")]
	public class PlayAnimation : Action
	{
		[Header("Animator"),Space]
		[SerializeField] protected SharedVPAnimator m_animator;

#if USE_ANIMANCER
		[Header("Use Animancer"),Space]
		[SerializeField] protected SharedBool m_useAnimancer = true;
#endif		
		
		[Header("Possible Values"), Space]
		[SerializeField] private SharedVPAnimatorParameterVariable m_parameterData;

		[Header("Exit Node"), Space]
		[SerializeField] private SharedBool m_exitNode = true;

		public override void OnReset()
		{
			base.OnReset();
		}
		
		public override void OnStart()
		{
			if (m_animator.Value == null)
			{
				
				if (Owner.TryGetComponentInChildren<VP_Animator>(out VP_Animator animator))
				{
					m_animator = animator;
				}
			}
			
			VP_AnimatorParameter parameter = m_parameterData.Value;
			
			if (m_animator.Value != null && parameter != null && parameter.AnimationName.IsNotNullNorEmpty())
			{
				switch (parameter.ParameterType)
				{
				case VirtualPhenix.Dialog.VariableTypes.Bool:
					SetAnimatorParameter(parameter.AnimationName, parameter.BoolValue, parameter.MaskIndex, parameter.GroupIndex, parameter.AnimationSet);
					break;
				case VirtualPhenix.Dialog.VariableTypes.Int:
					SetAnimatorParameter(parameter.AnimationName, parameter.IntValue, parameter.MaskIndex, parameter.GroupIndex, parameter.AnimationSet);
					break;
				case VirtualPhenix.Dialog.VariableTypes.Float:
					SetAnimatorParameter(parameter.AnimationName, parameter.FloatValue, parameter.MaskIndex, parameter.GroupIndex, parameter.AnimationSet);
					break;
				case VirtualPhenix.Dialog.VariableTypes.String:
					SetAnimatorParameter(parameter.AnimationName, "", parameter.MaskIndex, parameter.GroupIndex, parameter.AnimationSet);
					break;
				}
			}
		}


		protected virtual void SetAnimatorParameter<T0>(string _name, T0 _value, int layer = -1, int dic = 0, string set = "Default")
		{
			
#if USE_ANIMANCER
			if (!m_useAnimancer.Value)
			{

				m_animator.Value.SetParameter(_name, _value);
				return;
			}

			if (_value is bool)
			{
				m_animator.Value.PlayAnimationFromSet(_name, layer, dic, set);
			}
			else if (_value is string)
			{
				m_animator.Value.PlayAnimationFromSet(_name,layer, dic, set);
			}
			else if (_value is int)
			{
				m_animator.Value.PlayAnimationFromSet(_name,layer, dic, set);
			}
			else
			{
				m_animator.Value.PlayAnimationFromSet(_name, layer, dic, set);
			}
#else
			m_animator.Value.SetParameter(_name, _value);
#endif
		}

		public override TaskStatus OnUpdate()
		{
			return m_exitNode.Value ? TaskStatus.Success : TaskStatus.Running;
		}
		
		
	}
}
#endif