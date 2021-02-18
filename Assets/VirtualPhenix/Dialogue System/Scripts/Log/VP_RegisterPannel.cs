using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VirtualPhenix.Dialog
{
    public class VP_RegisterPannel : MonoBehaviour
    {
        [SerializeField] private GameObject m_logBackground;
        [SerializeField] private GameObject m_button;
        [SerializeField] private GameObject m_logItemPrefab;
        [SerializeField] private Transform m_contentTransform = null;

        private bool m_showingLog = false;

        private void Awake()
        {
            if (m_logItemPrefab == null)
            {
                m_logItemPrefab = Resources.Load<GameObject>(VP_DialogSetup.Log.PREFAB_PATH);
            }

            if (m_logBackground == null)
            {
                m_logBackground = transform.GetChild(0).GetChild(0).gameObject;
            }

            if (m_button == null)
            {
                m_button = transform.GetChild(0).GetChild(1).gameObject;
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            StartAllListeners();
        }

        private void OnDisable()
        {
            StopAllListeners();
        }

        void StartAllListeners()
        {
            VP_DialogManager.StartListeningToOnRegisterDialogAble(ShowLogButton);
            VP_DialogManager.StartListeningToOnRegisterDialogDisable(HideLogButton);
        }

        void StopAllListeners()
        {
            VP_DialogManager.StopListeningToOnRegisterDialogAble(ShowLogButton);
            VP_DialogManager.StopListeningToOnRegisterDialogDisable(HideLogButton);
        }

        void ParseLogs()
        {
            List<VP_DialogLog> logs = VP_DialogManager.Instance.GetRegisteredLogs();
            if (logs != null && logs.Count > 0)
            {
                foreach (VP_DialogLog log in logs)
                {
                    GameObject go = Instantiate(m_logItemPrefab, m_contentTransform);
                    VP_DialogLogObject logObj = go.GetComponent<VP_DialogLogObject>();
                    string chr = log.m_character ? log.m_character.characterName : "";
                    logObj.SetData(chr, log.m_saidText);
                }
            }
        }

        void ClearLogs()
        {
            foreach (Transform t in m_contentTransform)
                Destroy(t.gameObject);
        }

        public void ShowLogs()
        {
            m_showingLog = !m_showingLog;

            if (m_showingLog)
            {
                ParseLogs();
            }
            else
            {
                ClearLogs();
            }

            m_logBackground.SetActive(m_showingLog);
        }

        void ShowLogButton()
        {
            m_button.SetActive(true);
        }

        void HideLogButton()
        {
            m_button.SetActive(false);
        }

    }
}
