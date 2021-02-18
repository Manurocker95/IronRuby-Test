using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using VirtualPhenix.Controllers;
using VirtualPhenix.Actions;

namespace VirtualPhenix.Interaction
{
    [System.Serializable]
    public class VP_SavedInteractableObjectData
    {
        public string m_interactableObjectName;
        public int m_currentActionList;
        public bool m_canTrigger = true;
        public bool m_interactBubble = true;
        public bool m_rotateTowardsPlayer = true;
    }

    [System.Serializable]
    [DefaultExecutionOrder(VP_ExecutingOrderSetup.INTERACTABLE_OBJECT), AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/Object/Interactable Object")]
    public class VP_InteractableObject : VP_NonPlayableCharacterController
    {
        [Header("This interactable object specific data"), Space(10)]
        [SerializeField] protected VP_SavedInteractableObjectData m_savedData;

        [Header("-- Action List --"), Space(10)]
        [SerializeField] protected Dictionary<int, List<VP_CustomActions>> m_actionDic;

        [SerializeField] protected string m_talkingAnimatorBool = "talking2";
        [SerializeField] protected float m_turnDamping = 45;

        public Dictionary<int, List<VP_CustomActions>> ActionList { get { return m_actionDic; } }
        public int ActionListIndex { get { return m_savedData.m_currentActionList; } set { m_savedData.m_currentActionList = value; } }
        public VP_SavedInteractableObjectData SavedData { get { return m_savedData; } set { m_savedData = value; } }
        public System.Action<VP_SavedInteractableObjectData> m_onSetListIndex;
        public bool CanInteract { get { return m_savedData.m_canTrigger; } set { m_savedData.m_canTrigger = value; } }
        public bool ShowInteractBubble { get { return m_savedData.m_interactBubble; } }

        protected override void Start()
        {
            if (string.IsNullOrEmpty(m_savedData.m_interactableObjectName))
                m_savedData.m_interactableObjectName = this.gameObject.name;

            if (m_actionDic == null)
            {
                m_actionDic = new Dictionary<int, List<VP_CustomActions>>();
            }

            if (m_savedData.m_currentActionList > m_actionDic.Count)
                m_savedData.m_currentActionList = m_actionDic.Count - 1;
        }

        public virtual void RotateToPlayer(Vector3 _playerPos, System.Action _callback = null)
        {
            if (m_savedData.m_rotateTowardsPlayer)
            {
                var lookPos = _playerPos - transform.position;
                lookPos.y = 0;
                var rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * m_turnDamping);
            }

            if (_callback != null)
                _callback.Invoke();
        }

        public virtual void OnInteract(Vector3 _playerPos)
        {
            if (!m_savedData.m_canTrigger || VP_DialogManager.IsSpeaking)
                return;

            RotateToPlayer(_playerPos);

            if (m_actionDic.Count > 0 && m_actionDic.ContainsKey(m_savedData.m_currentActionList) && m_actionDic[m_savedData.m_currentActionList].Count > 0)
            {
                foreach (VP_CustomActions a in m_actionDic[m_savedData.m_currentActionList])
                {
                    if (a is VP_CustomSetListIndex)
                    {
                        a.InitActions(null, null, SetListIndex);
                    }
                    else if (a is VP_CustomBranchConditionAction)
                    {
                        a.m_interactableObjectName = m_savedData.m_interactableObjectName;
                        a.InitActions(OnDialogStart, OnDialogComplete, SetListIndex);
                    }
                    else
                    {
                        a.InitActions(null, null, null);
                    }

                }

                VP_ActionManager.Instance.DoGameplayAction();
            }
        }

        public virtual void ForceInteract(bool _rotateToPlayer)
        {
            if (VP_DialogManager.IsSpeaking)
                return;

            if (_rotateToPlayer)
            {
	            RotateToPlayer(Vector3.zero);
            }

            if (m_actionDic.Count > 0 && m_actionDic.ContainsKey(m_savedData.m_currentActionList) && m_actionDic[m_savedData.m_currentActionList].Count > 0)
            {
                foreach (VP_CustomActions a in m_actionDic[m_savedData.m_currentActionList])
                {
                    if (a is VP_CustomSetListIndex)
                    {
                        a.InitActions(null, null, SetListIndex);
                    }
                    else
                    {
                        a.InitActions(null, null, null);
                    }

                }

                VP_ActionManager.Instance.DoGameplayAction();
            }
        }

        public virtual void OnInteractTrigger()
        {
	        OnInteract(Vector3.zero);
        }

        public void SetListIndex(int _index)
        {
            if (_index < m_actionDic.Count)
            {
                m_savedData.m_currentActionList = _index;
            }

            if (m_onSetListIndex != null)
                m_onSetListIndex.Invoke(m_savedData);
        }

        protected virtual void OnDialogStart()
        {
            if (m_animator)
                m_animator.SetBool(m_talkingAnimatorBool, true);
        }

        protected virtual void OnDialogComplete()
        {
            if (m_animator)
                m_animator.SetBool(m_talkingAnimatorBool, false);
        }

    }
}
