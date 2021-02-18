using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace VirtualPhenix.Dialog
{
    [NodeTint(VP_DialogSetup.NodeColors.INIT_EVENT_NODE), CreateNodeMenuAttribute("Init")]
    public class VP_DialogInitEvent : VP_DialogBaseNode
    {
        [NodeBoolean, SerializeField] public bool onStart = true;
        [TextArea, SerializeField] public string startEvent;
        [SerializeField] public VP_DialogInitEventKey keyT;


        protected override void Awake()
        {
            base.Awake();
            RefreshInit();
        }

        public override void RefreshInit()
        {
            base.RefreshInit();
            if (keyT == null)
            {
                keyT = Resources.Load<VP_DialogInitEventKey>("Dialogue/Data/defaultInitEvents");
                keyT.key = "";
                // 
            }

            if (string.IsNullOrEmpty(m_ID))
                m_ID = VP_Utils.CreateID();

            if (keyT.list != null && !keyT.list.ContainsKey(m_ID))
                keyT.list.Add(m_ID, "");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (keyT != null && keyT.list != null && keyT.list.ContainsKey(m_ID))
                keyT.list.Remove(m_ID);
        }
    

        public override void Trigger()
        {
            Debug.Log("init event with start event: " + startEvent);
            IsCurrent = false;

        }
    }

  
}
