using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Helpers;

namespace VirtualPhenix
{
	[
        DefaultExecutionOrder(VP_ExecutingOrderSetup.EVENT_MANAGER),
        AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/Events/Event Manager")
    ]
    public class VP_EventManager : VP_SingletonMonobehaviour<VP_EventManager>
    {
        /// <summary>
        /// Event dictionary
        /// </summary>
        protected Dictionary<string, object> m_eventDictionary;

        /// <summary>
        /// Dictionary initialization
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            m_eventDictionary = new Dictionary<string, object>();
        }

        protected override void Reset()
        {
            base.Reset();
            m_startListeningTime = StartListenTime.None;
            m_stopListeningTime = StopListenTime.None;
        }

        /// <summary>
        /// The desired object is now listening if it wasn't
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="listener"></param>
        public static void StartListening(string eventName, UnityAction listener)
        {
            if (Instance == null || Instance.m_eventDictionary == null)
                return;

            UnityEvent thisEvent = null;
            object thisEventObject = null;
            if (Instance.m_eventDictionary.TryGetValue(eventName, out thisEventObject))
            {
                thisEvent = thisEventObject as UnityEvent;
                if (thisEvent != null)
                {
                    //thisEvent.
                    thisEvent.RemoveListener(listener);
                    thisEvent.AddListener(listener);
                }
            }
            else
            {
                thisEvent = new UnityEvent();
                thisEvent.AddListener(listener);
                Instance.m_eventDictionary.Add(eventName, thisEvent);
            }
        }
        /// <summary>
        /// The desired object is no longer listening
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="listener"></param>
        public static void StopListening(string eventName, UnityAction listener)
        {
            if (Instance == null || Instance.m_eventDictionary == null)
                return;

            UnityEvent thisEvent = null;
            object thisEventObject = null;
            if (Instance.m_eventDictionary.TryGetValue(eventName, out thisEventObject))
            {
                thisEvent = thisEventObject as UnityEvent;

                if (thisEvent != null)
                    thisEvent.RemoveListener(listener);
            }
        }
        /// <summary>
        /// Triggers the event and calls every object that is listening to that event
        /// </summary>
        /// <param name="eventName"></param>
        public static void TriggerEvent(string eventName)
        {
            if (Instance == null)
                return;

            UnityEvent thisEvent = null;
            object thisEventObject = null;
            if (Instance.m_eventDictionary.TryGetValue(eventName, out thisEventObject))
            {
                thisEvent = thisEventObject as UnityEvent;

                if (thisEvent != null)
                    thisEvent.Invoke();
            }
        }

        public static void StartListening<T>(string eventName, UnityAction<T> listener)
        {
            if (Instance == null)
                return;

            CustomEvent<T> thisEvent = null;
            object thisEventObject = null;

            if (Instance.m_eventDictionary.TryGetValue(eventName, out thisEventObject))
            {
                thisEvent = thisEventObject as CustomEvent<T>;
                if (thisEvent == null)
                {
                    thisEvent = new CustomEvent<T>();
                    /*
                    if(thisEvent==null){
                        Debug.LogError("Listen Event:" + eventName + " still has no listners");
                    }else{
                        Debug.Log("EventName:" + eventName + "Event: "+thisEvent);
                        Debug.Log("Listener " + listener);
                    }*/

                    thisEvent.AddListener(listener);

                    Instance.m_eventDictionary[eventName] = thisEvent;

                }
                else
                {
                    thisEvent.AddListener(listener);
                }
            }
            else
            {
                thisEvent = new CustomEvent<T>();
                thisEvent.AddListener(listener);
                Instance.m_eventDictionary.Add(eventName, thisEvent);
            }
        }

        public static void StopListening<T>(string eventName, UnityAction<T> listener)
        {
            if (Instance == null)
                return;

            CustomEvent<T> thisEvent = null;
            object thisEventObject = null;

            if (Instance.m_eventDictionary.TryGetValue(eventName, out thisEventObject))
            {
                thisEvent = thisEventObject as CustomEvent<T>;

                if (thisEvent != null)
                    thisEvent.RemoveListener(listener);
            }
        }

        public static void TriggerEvent<T>(string eventName, T value)
        {
            if (Instance == null)
                return;

            object eventObject = null;
            if (Instance.m_eventDictionary.TryGetValue(eventName, out eventObject))
            {
                CustomEvent<T> thisEvent = eventObject as CustomEvent<T>;
                if (thisEvent != null)
                {
                    thisEvent.Invoke(value);
                }
                else
                {
                    Debug.LogError("Event is null EventName: " + eventName + ", thisEvent: " + thisEvent);
                }

            }
        }

        public static void StartListening<T0, T1>(string eventName, UnityAction<T0, T1> listener)
        {
            if (Instance == null)
                return;

            CustomEvent<T0, T1> thisEvent = null;
            object thisEventObject = null;

            if (Instance.m_eventDictionary.TryGetValue(eventName, out thisEventObject))
            {
                thisEvent = thisEventObject as CustomEvent<T0, T1>;
                if (thisEvent == null)
                {
                    thisEvent = new CustomEvent<T0, T1>();
                    thisEvent.AddListener(listener);

                    Instance.m_eventDictionary[eventName] = thisEvent;
                }
                else
                {
                    thisEvent.AddListener(listener);
                }
            }
            else
            {
                thisEvent = new CustomEvent<T0, T1>();
                thisEvent.AddListener(listener);
                Instance.m_eventDictionary.Add(eventName, thisEvent);
            }
        }

        public static void StopListening<T0, T1>(string eventName, UnityAction<T0, T1> listener)
        {
            if (Instance == null)
                return;

            CustomEvent<T0, T1> thisEvent = null;
            object thisEventObject = null;

            if (Instance.m_eventDictionary.TryGetValue(eventName, out thisEventObject))
            {
                thisEvent = thisEventObject as CustomEvent<T0, T1>;
                thisEvent.RemoveListener(listener);
            }
        }

        public static void TriggerEvent<T0, T1>(string eventName, T0 value0, T1 value1)
        {
            if (Instance == null)
                return;

            object eventObject = null;
            if (Instance.m_eventDictionary.TryGetValue(eventName, out eventObject))
            {
                CustomEvent<T0, T1> thisEvent = eventObject as CustomEvent<T0, T1>;
                if (thisEvent != null)
                    thisEvent.Invoke(value0, value1);
            }
        }


        public static void StartListening<T0, T1, T2>(string eventName, UnityAction<T0, T1, T2> listener)
        {
            if (Instance == null)
                return;

            CustomEvent<T0, T1, T2> thisEvent = null;
            object thisEventObject = null;

            if (Instance.m_eventDictionary.TryGetValue(eventName, out thisEventObject))
            {
                thisEvent = thisEventObject as CustomEvent<T0, T1, T2>;
                if (thisEvent == null)
                {
                    thisEvent = new CustomEvent<T0, T1, T2>();
                    thisEvent.AddListener(listener);

                    Instance.m_eventDictionary[eventName] = thisEvent;
                }
                else
                {
                    thisEvent.AddListener(listener);
                }
            }
            else
            {
                thisEvent = new CustomEvent<T0, T1, T2>();
                thisEvent.AddListener(listener);
                Instance.m_eventDictionary.Add(eventName, thisEvent);
            }
        }

        public static void StopListening<T0, T1, T2>(string eventName, UnityAction<T0, T1, T2> listener)
        {
            if (Instance == null)
                return;

            CustomEvent<T0, T1, T2> thisEvent = null;
            object thisEventObject = null;

            if (Instance.m_eventDictionary.TryGetValue(eventName, out thisEventObject))
            {
                thisEvent = thisEventObject as CustomEvent<T0, T1, T2>;
                thisEvent.RemoveListener(listener);
            }
        }

        public static void TriggerEvent<T0, T1, T2>(string eventName, T0 value0, T1 value1, T2 value2)
        {
            if (Instance == null)
                return;

            object eventObject = null;
            if (Instance.m_eventDictionary.TryGetValue(eventName, out eventObject))
            {
                CustomEvent<T0, T1, T2> thisEvent = eventObject as CustomEvent<T0, T1, T2>;
                if (thisEvent != null)
                    thisEvent.Invoke(value0, value1, value2);
            }
        }

        public static void StartListening<T0, T1, T2, T3>(string eventName, UnityAction<T0, T1, T2, T3> listener)
        {
            if (Instance == null)
                return;

            CustomEvent<T0, T1, T2, T3> thisEvent = null;
            object thisEventObject = null;

            if (Instance.m_eventDictionary.TryGetValue(eventName, out thisEventObject))
            {
                thisEvent = thisEventObject as CustomEvent<T0, T1, T2, T3>;
                if (thisEvent == null)
                {
                    thisEvent = new CustomEvent<T0, T1, T2, T3>();
                    thisEvent.AddListener(listener);

                    Instance.m_eventDictionary[eventName] = thisEvent;
                }
                else
                {
                    thisEvent.AddListener(listener);
                }
            }
            else
            {
                thisEvent = new CustomEvent<T0, T1, T2, T3>();
                thisEvent.AddListener(listener);
                Instance.m_eventDictionary.Add(eventName, thisEvent);
            }
        }

        public static void StopListening<T0, T1, T2, T3>(string eventName, UnityAction<T0, T1, T2, T3> listener)
        {
            if (Instance == null)
                return;

            CustomEvent<T0, T1, T2, T3> thisEvent = null;
            object thisEventObject = null;

            if (Instance.m_eventDictionary.TryGetValue(eventName, out thisEventObject))
            {
                thisEvent = thisEventObject as CustomEvent<T0, T1, T2, T3>;
                thisEvent.RemoveListener(listener);
            }
        }

        public static void TriggerEvent<T0, T1, T2, T3>(string eventName, T0 value0, T1 value1, T2 value2, T3 value3)
        {
            if (Instance == null)
                return;

            object eventObject = null;
            if (Instance.m_eventDictionary.TryGetValue(eventName, out eventObject))
            {
                CustomEvent<T0, T1, T2, T3> thisEvent = eventObject as CustomEvent<T0, T1, T2, T3>;
                if (thisEvent != null)
                    thisEvent.Invoke(value0, value1, value2, value3);
            }
        }

    }

    [System.Serializable]
    public class CustomEvent<T> : UnityEvent<T>
    {

    }

    [System.Serializable]
    public class CustomEvent<T0, T1> : UnityEvent<T0, T1>
    {

    }

    [System.Serializable]
    public class CustomEvent<T0, T1, T2> : UnityEvent<T0, T1, T2>
    {

    }

    [System.Serializable]
    public class CustomEvent<T0, T1, T2, T3> : UnityEvent<T0, T1, T2, T3>
    {

    }
}
