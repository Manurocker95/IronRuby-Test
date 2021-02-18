using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if USE_FADE
using VirtualPhenix.Fade;
#endif

#if ODIN_INSPECTOR
using Sirenix.Serialization;
#endif
using VirtualPhenix.Variables;
using VirtualPhenix.Actions;
using VirtualPhenix.Dialog;

#if USE_MORE_EFFECTIVE_COROUTINES
using MEC;
#endif


namespace VirtualPhenix
{
	[DefaultExecutionOrder(VP_ExecutingOrderSetup.ACTION_MANAGER),AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/Actions/Action Manager")]
    public class VP_ActionManager : VP_SingletonMonobehaviour<VP_ActionManager>
	{
		[Header("Gameplay Actions - Debug"),Space]
		[SerializeField] protected Queue <UnityAction> m_gameplayActionQueue;
        [SerializeField] protected Queue<UnityAction> m_loopActionQueue;
		[SerializeField] protected Queue<UnityAction> m_waitingQueue;
        
		[Header("Runtime Database"),Space]
		[SerializeField] protected VP_VariableDataBase m_gameVariables;
        
		[Header("Initial Game Variables"),Space]
		[SerializeField] protected VP_GameVariables m_initialGameVar;
        
        
        protected VP_CustomLoopAction m_lastLoopAction;
		[SerializeField] protected UnityEvent m_lastActionPerformed;

        /// <summary>
        ///  Properties
        /// </summary>
		public virtual bool HasPendingEvents { get { return m_gameplayActionQueue.Count > 0; } }
        public virtual bool HasPendingLoopActions { get { return m_loopActionQueue.Count > 0; } }
        public virtual int PendingActionCount { get { return m_gameplayActionQueue.Count; } }
        public virtual int PendingLoopActionCount { get { return m_loopActionQueue.Count; } }

        public virtual VP_VariableDataBase GameVariables { get { return m_gameVariables; } set { m_gameVariables = value; } }

		protected bool m_waiting = false;



        protected override void Initialize()
        {
            base.Initialize();
	        m_gameplayActionQueue = new Queue<UnityAction>();
            m_loopActionQueue = new Queue<UnityAction>();
            m_waitingQueue = new Queue<UnityAction>();
            m_gameVariables = new VP_VariableDataBase();
        }

        // no parameters allowed yet
		public void AddGameplayAction(params UnityAction [] _actions)
        {
            if (m_gameplayActionQueue != null)
            {
            	foreach (UnityAction ua in _actions)
            		m_gameplayActionQueue.Enqueue(ua);              
            }
        }

        public static void ListenToLastActionPerformed(UnityAction _callback)
        {
            if (_callback != null)
            {
            	if (VP_ActionManager.TryGetInstance(out VP_ActionManager am))
            	{
            		am.m_lastActionPerformed.AddListener(_callback);
            	}
            }
        }

        public static void StopListeningToLastActionPerformed(UnityAction _callback)
        {
            if (_callback != null)
            {
	            if (VP_ActionManager.TryGetInstance(out VP_ActionManager am))
            	{
            		am.m_lastActionPerformed.RemoveListener(_callback);
            	}
            }
        }


        public static void TriggerLastActionPerformed()
        {
	        if (VP_ActionManager.TryGetInstance(out VP_ActionManager am))
	        {
		        am.m_lastActionPerformed.Invoke();
	        }
        }


        public virtual void DoGameplayAction()
        {
            if (HasPendingEvents)
            {
                UnityAction ac = m_gameplayActionQueue.Dequeue();
                Debug.LogWarning("PLAYED ACTION " + ac.Method.Name);
                ac.Invoke();
            }
            else
            {
                Debug.LogWarning("NO MORE  PENDING EVENTS");
                
            }

        }


        public virtual void DoActionBasedOnSelection(int _index)
        {
            if (_index <= m_gameplayActionQueue.Count - 1)
            {
                for (int i = 0; i < _index; i++)
                {
                    m_gameplayActionQueue.Dequeue();
                }
            }
            // TODO CAN ONLY INTERACT
            if (HasPendingEvents)
            {
                m_gameplayActionQueue.Dequeue().Invoke();
            }
            else
            {
               // TODO UNLOCK ALL INPUTS
            }
        }


        public virtual void ClearPendingActions()
        {
            m_gameplayActionQueue.Clear();
        }


        public virtual void AddLoopAction(UnityAction _action, VP_CustomLoopAction _loopAction)
        {
            if (m_lastLoopAction == null)
            {
                m_lastLoopAction = _loopAction;
                m_loopActionQueue.Enqueue(_action);
            }
        }

		public virtual void DoLoopAction()
        {
            if (HasPendingLoopActions)
            {
                m_loopActionQueue.Dequeue().Invoke();
            }
            else
            {
                if (m_lastLoopAction != null)
                {
                    m_lastLoopAction.CheckAllLoopAction();
                }
            }

        }

		public virtual void CancelLoopActions()
        {
            m_loopActionQueue.Clear();
            m_lastLoopAction = null;
        }

#if !USE_MORE_EFFECTIVE_COROUTINES
		protected virtual IEnumerator WaitTime(float _timeToWait = 1f, UnityAction _callback = null)
        {
            m_waiting = true;
            
            float timer = 0;
            while (timer <= _timeToWait)
            {
                if (_timeToWait == 0)
                    break;

                timer += Time.deltaTime;
                yield return null;
            }
            m_waiting = false;

            if (_callback != null)
                _callback.Invoke();
		}
#else
		protected virtual IEnumerator<float> WaitTime(float _timeToWait = 1f, UnityAction _callback = null)
		{
			m_waiting = true;
            
			float timer = 0;
			
			while (timer <= _timeToWait)
			{
				if (_timeToWait == 0)
					break;

				timer += Timing.DeltaTime;
				yield return Timing.WaitForOneFrame;
			}
			
			m_waiting = false;

			if (_callback != null)
				_callback.Invoke();
		}
#endif

		public virtual void StartWaitTime(float _timeToWait = 1f, UnityAction _callback = null)
        {
            if (m_waiting)
            {
                Debug.LogError("Waiting");

                //if (_callback != null)
                   // m_waitingQueue.Enqueue(_callback);

                return;
            }
#if !USE_MORE_EFFECTIVE_COROUTINES
            //StopCoroutine(WaitTime(_timeToWait, _callback));
	        StartCoroutine(WaitTime(_timeToWait, _callback));
#else
	        Timing.RunCoroutine(WaitTime(_timeToWait, _callback).CancelWith(gameObject));
#endif
        }
        
    }

}
