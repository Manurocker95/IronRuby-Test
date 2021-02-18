using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VirtualPhenix
{
    public class VP_Initializer : VP_SceneChanger
    {
        [Header("Initializer"), Space]
        [SerializeField] protected bool m_changeSceneOnInit = true;

        protected override void Initialize()
        {
            base.Initialize();
            DoOnInit();
        }

        protected virtual void DoOnInit()
        {
            if (m_changeSceneOnInit)
                ChangeScene();
        }
    }
}

