#if USE_BEHAVIOR_DESIGNER
using BehaviorDesigner.Runtime;
using VirtualPhenix.Controllers;
using VirtualPhenix.Dialog;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables;
using BehaviorDesigner.Runtime.Tasks;

namespace VirtualPhenix.Integrations.BehaviorDesignerTree
{
	[TaskCategory(VP_PrefixSetup.MAIN_PREFIX+"/SharedVariable")]
    [BehaviorDesigner.Runtime.Tasks.TaskDescription("Sets the SharedTransformList values from the Transforms. Returns Success.")]
    public class SharedDialogAnswersToDialogAnswerList : Action
    {
        [Tooltip("The Transforms value")]
	    public SharedDialogAnswer[] answers;
        [RequiredField]
        [Tooltip("The SharedTransformList to set")]
	    public SharedDialogAnswerList storedAnswerList;

        public override void OnAwake()
        {
	        storedAnswerList.Value = new List<VP_Dialog.Answer>();
        }

        public override TaskStatus OnUpdate()
        {
            if (answers == null || answers.Length == 0) {
                return TaskStatus.Failure;
            }

            storedAnswerList.Value.Clear();
            for (int i = 0; i < answers.Length; ++i) {
                storedAnswerList.Value.Add(answers[i].Value);
            }

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            answers = null;
            storedAnswerList = null;
        }
    }
}
#endif