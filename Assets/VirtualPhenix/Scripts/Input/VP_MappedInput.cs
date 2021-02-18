using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Inputs
{
    [System.Serializable]
    public class VP_MappedInput : VP_IInputMappeable
    {
        public int m_listeners;
        public List<UnityEngine.Events.UnityAction<VP_InputActions, VP_MappedInput>> m_callbacks;

        public VP_MappedInput()
        {
            m_callbacks = new List<UnityEngine.Events.UnityAction<VP_InputActions, VP_MappedInput>>();
        }

        public virtual void AddCallback(UnityEngine.Events.UnityAction<VP_InputActions, VP_MappedInput> _callback)
        {
            m_callbacks.Add(_callback);
            m_listeners = m_callbacks.Count;
        }

        public virtual void RemoveCallback(UnityEngine.Events.UnityAction<VP_InputActions, VP_MappedInput> _callback)
        {
            if (m_callbacks.Contains(_callback))
            {
                m_callbacks.Remove(_callback);
            }

            m_listeners = m_callbacks.Count;
        }

        public virtual bool HasCallback(UnityEngine.Events.UnityAction<VP_InputActions, VP_MappedInput> _callback)
        {
            return m_callbacks.Contains(_callback);
        }

        public virtual bool HasChanged()
        {
            return false;
        }

        public virtual bool IsPressed()
        {
            return false;
        }

        public virtual bool WasPressed()
        {
            return false;
        }

        public virtual bool WasReleased()
        {
            return false;
        }
    }
}