using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog
{
    public class VP_TestingConver : MonoBehaviour
    {
        [SerializeField] private Camera m_cam;

        [SerializeField] private Transform m_trs1;
        [SerializeField] private Transform m_trs2;

        [SerializeField] private VP_DialogPositionData data1;

        private void Start()
        {
            if (!m_cam)
                m_cam = Camera.main;

            if (data1 != null)
            {
                VP_DialogManager.Instance.SetVariable("cube2name", "Cool Cube 2");
                VP_DialogManager.Instance.SetVariable("cube1name", "Amazing Cube 1");

                OtherSpeaker();
                VP_DialogManager.SendDialogMessage(VP_DialogSetup.InitEvents.TEST_INIT_EVENT_DEMO4);
            }
            else
            {
                data1 = (VP_DialogPositionData) ScriptableObject.CreateInstance("VP_DialogPositionData");
                VP_DialogManager.Instance.SetVariable("cube2name", "Cool Cube 2");
                VP_DialogManager.Instance.SetVariable("cube1name", "Amazing Cube 1");

                OtherSpeaker();
                VP_DialogManager.SendDialogMessage(VP_DialogSetup.InitEvents.TEST_INIT_EVENT_DEMO4);
            }

            VP_EventManager.StartListening("PlayerPos", PlayerPos);
            VP_EventManager.StartListening("OtherSpeaker", OtherSpeaker);
        }

        private void OnDestroy()
        {
            VP_EventManager.StopListening("PlayerPos", PlayerPos);
            VP_EventManager.StopListening("OtherSpeaker", OtherSpeaker);
        }

        void PlayerPos()
        {
            if (m_trs2 == null)
            {
                m_trs2 = GameObject.Find("Cube 2").transform;
            }

            data1.SetTargetTransform(m_trs2, m_cam);
        }

        void OtherSpeaker()
        {
            if (m_trs1 == null)
            {
                m_trs1 = GameObject.Find("Cube 1").transform;
            }

            data1.SetTargetTransform(m_trs1, m_cam);
        }
    }
}
