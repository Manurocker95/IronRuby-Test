using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VirtualPhenix.Actions
{
    public enum CUSTOM_GAME_ACTIONS
    {
        NONE,
        DIALOG,
        DIALOG_WITH_OPTIONS,
        WAIT,
        SET_VARIABLE,
        SET_GAME_SWITCH,
        CALL_METHOD,
        UNITY_EVENTS,
        CUSTOM_EVENTS,
        PLAY_AUDIO,
        STOP_AUDIO,
        SET_POSITION,
        PLAY_ANIMATION,
        PLAY_VFX,
        MOVE_TO,
        ROTATE_TO,
        ROTATE_TO_PLAYER,
        STOP_NAVMESH_MOVEMENT,
        SET_LIST_INDEX,
        INIT_OTHER_NPC_INTERACTION,
        FADE_EFFECT,
        CHANGE_SCENE, // Load async a scene
        LOAD_ADDITIVE_SCENE,
        CHANGE_SCENE_WITH_FADE
    }

    public enum FADE_EFFECT_TYPE
    {
        UP,
        DOWN
    }

    
    [Serializable]
    public class VP_CustomActions // this needs to be separated in child objects set by inspector
    {
        protected Action m_onStartCallback;
        protected Action m_onEndCallback;
        protected Action<int> m_onListCallback;

        [Header("-- Type --"),Space(10)]
        protected CUSTOM_GAME_ACTIONS m_action;

        public CUSTOM_GAME_ACTIONS ActionType { get { return m_action; } }
        public string m_interactableObjectName = "";

        public VP_CustomActions()
        {
            
        }

        public virtual void InitActions(Action _initCallback = null, Action _invokeCallback = null, Action<int> _indexCallback = null)
        {
            if (_initCallback != null)
                m_onStartCallback = new Action(_initCallback);

            if (_invokeCallback != null)
                m_onEndCallback = new Action(_invokeCallback);

            if (_indexCallback != null)
                m_onListCallback = new Action<int>(_indexCallback);

            VP_ActionManager.Instance.AddGameplayAction(this.InvokeAction);
        }

        public virtual void InvokeAction()
        {
            if (m_onStartCallback != null)
                m_onStartCallback.Invoke();

        }
   
    }
}
