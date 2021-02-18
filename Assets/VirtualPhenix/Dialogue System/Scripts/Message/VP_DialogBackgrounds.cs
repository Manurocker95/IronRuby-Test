using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog
{
    [CreateAssetMenu(fileName = "DialogBackground", menuName = "Virtual Phenix/Dialogue System/Dialogue Backgrounds", order = 1)]
    public class VP_DialogBackgrounds : VP_ScriptableObject
    {
        /// <summary>
        /// Sprites for the background image:
        /// 0 = REGULAR
        /// 1 = MUGSHOT
        /// 2 = REGULAR_NAME
        /// 3 = MUGSHOT_AND_NAME
        /// </summary>
        [SerializeField] protected VP_SerializedDialogTypeSprites m_backgrounds;
        [SerializeField] protected Sprite m_defaultBackground;
        [SerializeField] protected string m_defaultPath = "Graphics\\Backgrounds\\";

        protected override void OnValidate()
        {
            base.OnValidate();

            if (m_defaultBackground == null)
            {
                m_defaultBackground = Resources.Load<Sprite>(m_defaultBackground + "\\default");
            }

            if (m_backgrounds == null)
                m_backgrounds = new VP_SerializedDialogTypeSprites();

            if (m_backgrounds.Count == 0)
            {
                Sprite normal = Resources.Load<Sprite>(m_defaultBackground + "\\REGULAR");
                if (normal)
                    m_backgrounds.Add(DIALOG_TYPE.REGULAR, normal);

                Sprite mugshot = Resources.Load<Sprite>(m_defaultBackground + "\\MUGSHOT");
                if (mugshot)
                    m_backgrounds.Add(DIALOG_TYPE.MUGSHOT, mugshot);

                Sprite mugshotName = Resources.Load<Sprite>(m_defaultBackground + "\\MUGSHOT_AND_NAME");
                if (mugshotName)
                    m_backgrounds.Add(DIALOG_TYPE.MUGSHOT_AND_NAME, mugshotName);

                Sprite mrefName = Resources.Load<Sprite>(m_defaultBackground + "\\REGULAR_NAME");
                if (mrefName)
                    m_backgrounds.Add(DIALOG_TYPE.REGULAR_NAME, mugshotName);
            }
        }

        public Sprite GetBackground(DIALOG_TYPE _type)
        {
            if (m_backgrounds.Contains(_type))
                return m_backgrounds[_type];

            return m_defaultBackground;
        }
    }
}
