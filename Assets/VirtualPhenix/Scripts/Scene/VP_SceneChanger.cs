using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

#if USE_MORE_EFFECTIVE_COROUTINES
using MEC;
#endif

namespace VirtualPhenix
{
    public class VP_SceneChanger : VP_Monobehaviour
    {
        [Header("Scene Changer"),Space]
        [SerializeField] protected VP_SceneReference m_sceneToChange = new VP_SceneReference();
        [SerializeField] protected float m_delayAfterLoad = 1f;
        [SerializeField] protected bool m_loadingScreen = true;
        [SerializeField] protected bool m_useSceneManager = true;
        [SerializeField] protected UnityEvent m_preLoad;
        [SerializeField] protected UnityEvent m_afterLoad;

        public virtual void ChangeScene()
        {
            m_preLoad.Invoke();

            string scene = m_sceneToChange.SceneName;
            if (m_sceneToChange == null && string.IsNullOrEmpty(scene))
                scene = VP_SceneSetup.MENU;

            if (m_useSceneManager)
            {
                VP_SceneManager.LoadScene(scene, m_delayAfterLoad, VP_EventSetup.Init.AFTER_INIT, true, m_loadingScreen, true, m_afterLoad.Invoke);
            }
            else
            {
#if USE_MORE_EFFECTIVE_COROUTINES
                Timing.RunCoroutine(LoadSceneAsync(scene).CancelWith(gameObject));
#else
                StartCoroutine(LoadSceneAsync(scene));
#endif
            }
        }

        protected virtual void DoOnLoad()
        {

        }

#if USE_MORE_EFFECTIVE_COROUTINES

        protected virtual IEnumerator<float> LoadSceneAsync(string _scene)
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync(_scene, LoadSceneMode.Single);
            while (!ao.isDone)
            {
                DoOnLoad();
                yield return Timing.WaitForOneFrame;
            }
            yield return Timing.WaitForSeconds(m_delayAfterLoad);
            VP_EventManager.TriggerEvent(VP_EventSetup.Init.AFTER_INIT);
            m_afterLoad.Invoke();
        }
#else
        protected virtual IEnumerator LoadSceneAsync(string _scene)
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync(_scene, LoadSceneMode.Single);
            while (!ao.isDone)
            {
                DoOnLoad();
                yield return null;
            }
            yield return new WaitForSeconds(m_delayAfterLoad);
            VP_EventManager.TriggerEvent(VP_EventSetup.Init.AFTER_INIT);
            m_afterLoad.Invoke();
        }
#endif
    }
}