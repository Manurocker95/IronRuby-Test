using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace VirtualPhenix.Dialog
{
    public abstract class VP_DialogBaseNode : Node
    {
        public bool m_renamed = false;
        public Color overrideNodeColor;
        public bool overrideColor;
	    public bool triggerFirstOnEnd = true;
	    
        [Input(backingValue = ShowBackingValue.Unconnected, typeConstraint = TypeConstraint.Inherited)]
        public VP_DialogBaseNode input;

        [Output(backingValue = ShowBackingValue.Unconnected)]
        public VP_DialogBaseNode output;


        [SerializeField] protected string m_ID;
        abstract public void Trigger();
        
        protected bool m_needToSkip;

        protected override void OnEnable()
        {
            base.OnEnable();
#if UNITY_EDITOR
            overrideNodeColor = XNodeEditor.NodeEditorPreferences.GetTypeColor(typeof(VP_DialogBaseNode));
#endif
            if (string.IsNullOrEmpty(m_ID))
                m_ID = VP_Utils.CreateID();


        }

        protected virtual void OnDisable()
        {

        }

        protected virtual void Awake()
        {
            if (Application.isPlaying)
                StartAllListeners();
        }

        protected virtual void OnDestroy()
        {
            if (Application.isPlaying)
                StopAllListeners();
        }

        protected virtual void StartAllListeners()
        {
            VP_DialogManager.StartListeningToOnSkip(StartNeedSkip);
        }

        protected virtual void StopAllListeners()
        {
            VP_DialogManager.StopListeningToOnSkip(StartNeedSkip);
        }

        protected virtual void StartNeedSkip()
        {
            m_needToSkip = true;
        }

        public virtual void RefreshInit()
        {

        }
        
        public virtual void Trigger<T>(T parameter)
        {
            Trigger();
        }
        
        public override object GetValue(NodePort port)
        {
            return port.node;
        }

        public virtual void CheckEndSequenceNode()
        {
            if (VP_DialogManager.OnDialogCompleteForOutput == null)
                VP_DialogManager.OnDialogEndAction();
            else
                VP_DialogManager.OnDialogCompleteOutputAction();
        }

        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            base.OnCreateConnection(from, to);

            if ((to.node as VP_DialogBaseNode) == this)
            {
                input = from.node as VP_DialogBaseNode;
            }

            if ((from.node as VP_DialogBaseNode) == this)
            {
                output = to.node as VP_DialogBaseNode;
            }

        }

        public override void OnRemoveConnection(NodePort port)
        {
            base.OnRemoveConnection(port);

            if (port == GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_OUTPUT)) //port.node as VP_DialogBaseNode )
            {
                output = null;
            }

            if (port == GetOutputPort(VP_DialogSetup.Fields.DIALOG_NODE_INPUT)) //port.node as VP_DialogBaseNode )
            {
                output = null;
            }
        }

        public virtual void OnDropObjects(Object[] objects)
        {
            Debug.Log("On drop in Dialog Base Node. Override in child.");
        }

    }

}
