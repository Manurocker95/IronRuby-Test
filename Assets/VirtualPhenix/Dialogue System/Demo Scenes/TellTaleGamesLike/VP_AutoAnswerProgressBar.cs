using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VirtualPhenix.Dialog
{
	[AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/Demo/UI/AutoAnswer Progress Bar")]
    public class VP_AutoAnswerProgressBar : MonoBehaviour
    {
        [SerializeField] private Image m_barImage = null;
        [SerializeField] private bool m_updatingBar = false;

        private void Awake()
        {
            if (!m_barImage)
                m_barImage = GetComponent<Image>();

            DisappearBar();
        }

        void AppearBar()
        {
            m_barImage.enabled = true;
            m_updatingBar = true;
        }

        void DisappearBar()
        {
            m_barImage.enabled = false;
            m_updatingBar = false;
        }

        void StartListening()
        {
            VP_DialogManager.StartListeningToOnAutoAnswerTimeStart(AppearBar);
            VP_DialogManager.StartListeningToOnAutoAnswerTimeEnd(DisappearBar);
        }

        void StopListening()
        {
            VP_DialogManager.StopListeningToOnAutoAnswerTimeStart(AppearBar);
            VP_DialogManager.StopListeningToOnAutoAnswerTimeEnd(DisappearBar);
        }

        void OnEnable()
        {
            StartListening();
        }

        private void OnDisable()
        {
            StopListening();
        }


        // Update is called once per frame
        void Update()
        {
            if (m_updatingBar)
            {
                m_barImage.fillAmount = VP_DialogManager.GetAutoTimeProgress();
            }
        }
    }

}
