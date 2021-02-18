using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VirtualPhenix.Demo.Test
{
    public class VP_DEMO_Menu : MonoBehaviour
    {
        private static VP_DEMO_Menu m_instance;
        public static VP_DEMO_Menu Instance { get { return m_instance; } }


        [SerializeField] private GameObject m_canvasGroup = null;
        [SerializeField] private bool m_inDialogs;

        private void Awake()
        {
            if (m_instance == null)
            {
                m_instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (m_inDialogs)
                {
                    m_canvasGroup.SetActive(true);
                    SceneManager.LoadScene(0);
                    m_inDialogs = false;
                }
                else
                {
                    Exit();
                }
            }
        }

        public void SelectButton(int val)
        {
            m_canvasGroup.SetActive(false);
            m_inDialogs = true;
            SceneManager.LoadScene(val+1);
        }

        public void Exit()
        {
            Application.Quit();
        }
    }

}
