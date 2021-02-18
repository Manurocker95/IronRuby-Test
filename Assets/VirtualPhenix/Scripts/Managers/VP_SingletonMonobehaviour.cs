using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace VirtualPhenix
{
    public class VP_MonobehaviourSettings
    {
        /// <summary>
        /// If it doesn't exist any instance, do it
        /// </summary>
        public static bool CreateNewInstance = true;
        public static bool Quiting = false;
    }

    public class VP_SingletonMonoBehaviour<T> : VP_SingletonMonobehaviour<T> where T : VP_Monobehaviour
    {

    }

    /// <summary>
    /// A singleton implementation for MonoBehaviours
    /// Just heritate from any manager and voil�! 
    /// Singleton instance!
    /// </summary>
    /// <typeparam name="T">Any type we want</typeparam>
    public class VP_SingletonMonobehaviour<T> : VP_Monobehaviour where T : VP_Monobehaviour
    {
        /// <summary>
        /// The actual instance of this type.
        /// </summary>
        protected static VP_Monobehaviour m_instance;

        [Header("Singleton"), Space(10)]
        /// <summary>
        /// Do we wanna destroy this GO across levels
        /// </summary>
        [SerializeField] protected bool m_destroyOnLoad = false;
        [Header("Instance Setup"), Space(10)]
        /// <summary>
        /// if only the first instance persists
        /// </summary>
        [SerializeField] protected bool m_singleInstance = true;
        /// <summary>
        /// If you really want to use Singleton in the inherited script. If not, it acts as VP_Monobehaviour
        /// </summary>
        [SerializeField] protected bool m_initInstance = true;

        public virtual bool dontDestroyOnLoad { get { return !m_destroyOnLoad; } }
        public virtual bool Initialized { get { return m_initialized; } }

        protected virtual void Reset()
        {
            m_initializationTime = InitializationTime.Singleton;
            m_startListeningTime = StartListenTime.None;
            m_stopListeningTime = StopListenTime.None;
        }

        /// <summary>
        /// Get an instance to this MonoBehaviour.Always returns a valid object.
        /// </summary>
        public static T NotNullableInstance
        {
            get
            {
                if (m_instance == null)
                {
                	if (VP_MonobehaviourSettings.Quiting)
                	{
                		return null;
                	}
                	
                    // first search the scene for an instance
                    T[] scene = FindObjectsOfType<T>();


                    if (scene != null && scene.Length > 0)
                    {
                        m_instance = scene[0];

                        for (int i = 1; i < scene.Length; i++)
                        {
                            Destroy(scene[i]);
                        }
                    }
                    else if (!VP_MonobehaviourSettings.Quiting && VP_MonobehaviourSettings.CreateNewInstance)
                    {
                        GameObject go = new GameObject();
                        string type_name = typeof(T).ToString();
                        int i = type_name.LastIndexOf('.') + 1;
                        go.name = (i > 0 ? type_name.Substring(i) : type_name) + " Singleton";
                        T inst = go.AddComponent<T>();
                        VP_SingletonMonobehaviour<T> cast = inst as VP_SingletonMonobehaviour<T>;
                        if (cast != null)
                            cast.Initialize();
                        m_instance = (VP_Monobehaviour)inst;
                    }

                    if (m_instance && !((VP_SingletonMonobehaviour<T>)m_instance).m_destroyOnLoad)
                        Object.DontDestroyOnLoad(m_instance.gameObject);
                }

                return (T)m_instance;
            }
        }

        /// <summary>
        /// Return the instance if it has been initialized, null otherwise.
        /// </summary>
        public static bool TryGetInstance(out T inst)
        {
            inst = Instance;
            return inst != null;
        }


        /// <summary>
        /// Return the instance if it has been initialized, null otherwise.
        /// </summary>
        public static bool TryGetInstance<T0>(out T0 inst) where T0 : T
        {
            inst = (T0)m_instance;
            return inst != null;
        }

        /// <summary>
        /// Return the instance if it has been initialized, null otherwise.
        /// </summary>
        public static T Instance
        {
            get { return (T)m_instance; }
        }

        /// <summary>
        /// Return the instance if it has been initialized, null otherwise.
        /// </summary>
        public static T nullableInstance
        {
            get { return Instance; }
        }

        /// <summary>
        /// Returns real instance (not VP_SingletonMonobehaviour in case child of VP ones are set. If null, it will create an object to initialize it
        /// 
        /// E.G:
        /// 
        /// SaveManager saveManager = SaveManager.GetCastedNullableInstance<SaveManager>();
        /// 
        /// returns SaveManager even if it is child of VP_SaveManager
        /// 
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <returns></returns>

        public static T0 GetCastedInstance<T0>() where T0 : T
        {
            return Instance as T0;
        }

        /// <summary>
        /// Returns real instance (not VP_SingletonMonobehaviour in case child of VP ones are set. Can be null.
        /// 
        /// E.G:
        /// 
        /// SaveManager saveManager = SaveManager.GetCastedNullableInstance<SaveManager>();
        /// 
        /// returns SaveManager even if it is child of VP_SaveManager
        /// 
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <returns></returns>
        public static T0 GetCastedNullableInstance<T0>() where T0 : T
        {
            return nullableInstance as T0;
        }

        /// <summary>
        /// If overriding, be sure to call base.Awake().
        /// </summary>
        protected override void Awake()
        {
            if (m_initInstance)
            {
                if (m_instance == null)
                {
                    m_instance = this;

                    if (!VP_MonobehaviourSettings.Quiting)
                    {
                        if (!m_destroyOnLoad)
                            DontDestroyOnLoad(m_instance.gameObject);

                        base.Awake();

                        // we call the initialize method-> Only singleton
                        if (m_initializationTime == InitializationTime.Singleton)
                        {
                            Initialize();
                        }
                    }
                }
                else if (m_instance != this && m_singleInstance)
                {
                    Destroy(this.gameObject);
                }
            } 
            else
            {
                // we call the initialize method-> Only singleton
                if (m_initializationTime == InitializationTime.Singleton)
                {
                    Initialize();
                }
            }
        }

        /// <summary>
        /// Called when an instance is initialized due to no previous instance found.  Use this to
		/// initialize any resources this singleton requires (eg, if this is a gui item or prefab,
        /// build out the hierarchy in here or instantiate stuff).
        /// </summary>
        protected override void Initialize()
        {
            if (m_initialized)
                return;

            base.Initialize();
        }

        protected virtual void OnApplicationQuit()
        {
            VP_MonobehaviourSettings.Quiting = true;
        }

        protected override void OnDestroy()
        {
            if (!VP_MonobehaviourSettings.Quiting && (VP_SingletonMonobehaviour<T>)m_instance)
            {
                VP_Debug.Log("Destroying " + name);
                base.OnDestroy();
            }
        }

    }
}
