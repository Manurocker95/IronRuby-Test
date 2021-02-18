using UnityEngine;
using System.Collections.Generic;

namespace VirtualPhenix
{
	[DefaultExecutionOrder(VP_ExecutingOrderSetup.GENERIC_POOL_MANAGER)]
    /// <summary>
    /// Create/Destroy: About real resources
    /// Allocate/Release: About pooled resources
    /// </summary>
    /// <typeparam name="T">Class name overriden for Singleton class</typeparam>
    /// <typeparam name="T1">Main Resource type</typeparam>
    /// <typeparam name="T2">ID/Second parameter for creating resources</typeparam>
    public class VP_PoolManager<T, T1, T2> : VP_SingletonMonobehaviour<T> where T : VP_Monobehaviour
    {
        public enum PoolCreationTime
        {
            OnInit,
            OnAwake,
            OnStart,
            OnEnable,
            None
        }

        public enum PoolDestroyTime
        {
            OnDestroy,
            OnDisable,
            None
        }

        [Header("Pool Manager"), Space]
        [SerializeField] protected PoolCreationTime m_creationTime = PoolCreationTime.OnInit;
        [SerializeField] protected PoolDestroyTime m_destroyTime = PoolDestroyTime.OnDestroy;
        [SerializeField] protected int m_initialSize;
        [SerializeField] protected int m_maxSize = 100;
        [SerializeField] protected bool m_resizable = true;
        [SerializeField] protected bool m_allowExternalCreation = false;


#if GPU_INSTANCER
        [Header("GPU Instancer"), Space]
        [SerializeField] protected bool m_initializeGPUInstancer = false;
        [SerializeField] protected GPUInstancer.GPUInstancerPrefabManager m_gpuPrefabManager;
        [SerializeField] protected List<GPUInstancer.GPUInstancerPrefab> m_gpuInstancedPrefabs;
#endif

        [Header("Debug Logs"),Space]
        [SerializeField] protected bool m_logPoolExpansion = true;
        [SerializeField] protected string m_logCustomPoolName;
        [SerializeField] protected string m_logCustomResourceName;

        /// <summary>
        /// Debug read only: Indicates how much memory the pool is taking up, including unused items.
        /// </summary>
        [SerializeField] protected int debugCurrentSize;

        protected readonly List<Item> m_pool = new List<Item>();
        protected string LogPoolName { get { return string.IsNullOrEmpty(m_logCustomPoolName) ? gameObject.name : m_logCustomPoolName; } }
        protected string LogResourceName { get { return string.IsNullOrEmpty(m_logCustomResourceName) ? $"resource {typeof (T1)}" : m_logCustomResourceName; } }
        public int PoolCount { get { return m_pool.Count; } }

        protected override void Initialize()
        {
            base.Initialize();

            if (m_creationTime == PoolCreationTime.OnInit)
            {
                // Allocate initial
                for (var i = 0; i < m_initialSize; i++)
                {
                    Create(default(T2));
                }
            }

#if GPU_INSTANCER
            if (m_gpuPrefabManager == null)
                m_gpuPrefabManager = GameObject.FindObjectOfType<GPUInstancer.GPUInstancerPrefabManager>();

            if (m_initializeGPUInstancer)
            {
                GPUInstancer.GPUInstancerAPI.InitializeGPUInstancer(m_gpuPrefabManager);
            }
#endif
        }

        protected override void Awake()
        {
            base.Awake();

            if (m_creationTime == PoolCreationTime.OnAwake)
            {
                // Allocate initial
                for (var i = 0; i < m_initialSize; i++)
                {
                    Create(default(T2));
                }
            }
        }

        protected override void Start()
        {
            base.Start();

            if (m_creationTime == PoolCreationTime.OnStart)
            {
                // Allocate initial
                for (var i = 0; i < m_initialSize; i++)
                {
                    Create(default(T2));
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();


            if (m_creationTime == PoolCreationTime.OnEnable)
            {
                // Allocate initial
                for (var i = 0; i < m_initialSize; i++)
                {
                    Create(default(T2));
                }
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (m_destroyTime == PoolDestroyTime.OnDisable)
            {
                foreach (var item in m_pool)
                {
                    DestroyResource(item.m_resource);

                    if (item.m_useGPUInstancing)
                    {
                        DestroyFromGPUInstancer(item);
                    }
                }
            }
        }

        protected override void OnDestroy()
        {
            if (m_destroyTime == PoolDestroyTime.OnDestroy)
            {
                foreach (var item in m_pool)
                {
                    DestroyResource(item.m_resource);

                    if (item.m_useGPUInstancing)
                    {
                        DestroyFromGPUInstancer(item);
                    }
                }
            }
        }

        public virtual void CreateResourcesFromList(List<KeyValuePair<T1, T2>> _list)
        {
            if (!m_allowExternalCreation)
                return;


            foreach (KeyValuePair<T1, T2> val in _list)
            {
                Create(val.Key, val.Value);
            }
        }

        public virtual Item CreateResourceExt(T1 _resource, T2 _id, bool _used = false)
        {
            if (m_allowExternalCreation)
                return Create(_resource, _id, _used);

            return null;
        }

        /// <summary>
        /// Override this to specify how to create a resource.
        /// </summary>
        /// <returns>Newly created resource</returns>
        protected virtual T1 CreateResource(T2 _id)
        {
            return CreateDefaultResource();
        }

        protected virtual T1 CreateResource(T2 _id, T1 _instanced)
        {
            return CreateDefaultResource(_instanced);
        }

        /// <summary>
        /// Override this to specify how to create a resource.
        /// </summary>
        /// <returns>Newly created resource</returns>
        protected virtual T1 CreateResource(T2 _id, VirtualPhenix.ResourceReference.VP_ResourceReferencer<T2, T1> _library)
        {
            return CreateDefaultResource();
        }

        /// <summary>
        /// Override this to specify how to destroy a resource.
        /// </summary>
        /// <param name="resource">Resource to destroy</param>
        protected virtual void DestroyResource(Item resource)
        {
            DestroyResource(resource.m_resource);
        }

        /// <summary>
        /// Override this to specify how to destroy a resource.
        /// </summary>
        /// <param name="resource">Resource to destroy</param>
        protected virtual void DestroyResource(T1 resource)
        {
            
        }

        /// <summary>
        /// Override this to specify how to destroy a resource.
        /// </summary>
        /// <param name="resource">Resource to destroy</param>
        protected virtual void DestroyAllResourcesWithID(T2 _id)
        {
            foreach (Item item in m_pool)
            {
                if (VP_Utils.IsObjectEqualsTo(item.m_id, _id))
                {
                    DestroyResource(item);
                }
            }
        }

        public virtual bool Deallocate(Item item, bool _destroyIt = true)
        {
            if (m_pool.Contains(item))
            {
                m_pool.Remove(item);

                if (_destroyIt)
                    DestroyResource(item);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gives the callee a pooled item.
        /// Callee must call Release or ReleaseAll after use.
        /// </summary>
        /// <returns>A new item (marked used), or null if pool is full.</returns>
        public virtual Item Allocate()
        {
            foreach (Item item in m_pool)
            {
                if (item.m_used) 
                    continue;

                // Return unused item

                item.m_used = true;
                return item;
            }

            // No unused items
            // Create if allowed

            if (m_pool.Count < m_maxSize)
            {
                return Create(default(T2), true);
            }

            if (m_resizable)
            {

                if (m_logPoolExpansion)
                {
                    VP_Debug.LogWarning($"{LogPoolName} full." + $"Resizing allowed but max size ({m_maxSize}) is reached.");
                    VP_Debug.Log($"Creating new {LogResourceName}. New size is {m_pool.Count + 1}.");
                }

                m_maxSize += 1;
                return Create(default(T2), true);
            }

            VP_Debug.LogWarning($"{LogPoolName} full. Resizing not allowed. Returning null.");
            return null;
        }

        public virtual T1 CreateDefaultResource(T1 _other)
        {
            return default(T1);
        }

        public virtual T1 CreateDefaultResource()
        {
            return default(T1);
        }

        public virtual T2 CreateDefaultID()
        {
            return default(T2);
        }

        public virtual Item Allocate(T2 cast, VirtualPhenix.ResourceReference.VP_ResourceReferencer<T2, T1> _library)
        {
            foreach (Item item in m_pool)
            {
                if (!VP_Utils.IsObjectEqualsTo(item.m_id, cast) || item.m_used)
                    continue;

                // Return unused item

                item.m_used = true;
                return item;
            }

            if (cast == null)
                cast = CreateDefaultID();

            // No unused items
            // Create if allowed

            if (m_pool.Count < m_maxSize)
            {
                return Create(cast, _library, true);
            }

            if (m_resizable)
            {

                if (m_logPoolExpansion)
                {
                    VP_Debug.LogWarning($"{LogPoolName} full." + $"Resizing allowed but max size ({m_maxSize}) is reached.");
                    VP_Debug.Log($"Creating new {LogResourceName}. New size is {m_pool.Count + 1}.");
                }

                m_maxSize += 1;
                return Create(cast, _library, true);
            }

            VP_Debug.LogWarning($"{LogPoolName} full. Resizing not allowed. Returning null.");
            return null;
        }


        public virtual Item Allocate(T2 cast, T1 _instanced)
        {
            foreach (Item item in m_pool)
            {
                if (!VP_Utils.IsObjectEqualsTo(item.m_id, cast) || item.m_used)
                    continue;

                // Return unused item

                item.m_used = true;
                return item;
            }

            if (cast == null)
                cast = CreateDefaultID();

            // No unused items
            // Create if allowed

            if (m_pool.Count < m_maxSize)
            {
                return Create(cast, _instanced, true);
            }

            if (m_resizable)
            {

                if (m_logPoolExpansion)
                {
                    VP_Debug.LogWarning($"{LogPoolName} full." + $"Resizing allowed but max size ({m_maxSize}) is reached.");
                    VP_Debug.Log($"Creating new {LogResourceName}. New size is {m_pool.Count + 1}.");
                }

                m_maxSize += 1;
                return Create(cast, _instanced, true);
            }

            VP_Debug.LogWarning($"{LogPoolName} full. Resizing not allowed. Returning null.");
            return null;
        }

        /// <summary>
        /// Gives the callee a pooled item.
        /// Callee must call Release or ReleaseAll after use.
        /// </summary>
        /// <returns>A new item (marked used), or null if pool is full.</returns>
        public virtual Item Allocate(T2 cast)
        {
            foreach (Item item in m_pool)
            {
                if (!VP_Utils.IsObjectEqualsTo(item.m_id, cast) || item.m_used)
                    continue;

                // Return unused item
                item.m_used = true;
                return item;
            }

            // No unused items
            // Create if allowed

            if (cast == null)
                cast = CreateDefaultID();

            if (m_pool.Count < m_maxSize)
            {
                return Create(cast, true);
            }

            if (m_resizable)
            {

                if (m_logPoolExpansion)
                {
                    VP_Debug.LogWarning($"{LogPoolName} full." + $"Resizing allowed but max size ({m_maxSize}) is reached.");
                    VP_Debug.Log($"Creating new {LogResourceName}. New size is {m_pool.Count + 1}.");
                }

                m_maxSize += 1;
                return Create(cast, true);
            }

            VP_Debug.LogWarning($"{LogPoolName} full. Resizing not allowed. Returning null.");
            return null;
        }

        public virtual void Release(Item item)
        {
            item.m_used = false;
        }


	    public virtual void Release(object item)
	    {
		    var it = item as Item;
		    Release(it);
	    }


        public virtual void ReleaseAll()
        {
            foreach (Item item in m_pool)
            {
                Release(item);
            }
        }

        protected virtual Item Create(T1 _resource, T2 _id, bool used = false)
        {
            // Create new pool item and add to pool

            var item = new Item() { m_resource = _resource, m_used = used, m_id = _id };
            m_pool.Add(item);

            // Update allocated count

            debugCurrentSize = m_pool.Count;

            return item;
        }

        protected virtual Item Create(T1 _resource, bool used = false)
        {
            // Create new pool item and add to pool

            var item = new Item() { m_resource = _resource, m_used = used, m_id = default(T2) };
            m_pool.Add(item);

            // Update allocated count

            debugCurrentSize = m_pool.Count;

            return item;
        }

        protected virtual Item Create(bool used = false)
        {
            // Create new pool item and add to pool

            var item = new Item() { m_resource = CreateResource(default(T2)), m_used = used, m_id = default(T2) };
            m_pool.Add(item);

            // Update allocated count
     
            debugCurrentSize = m_pool.Count;

            return item;
        }
        /// <summary>
        /// ID-> Some pools may need extra parameter: E.g: Instancing prefabs may need the ID/Name of the prefab. Just use that as third parameter.
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="used"></param>
        /// <returns></returns>
        protected virtual Item Create(T2 _id, bool used = false)
        {
            // Create new pool item and add to pool

            var item = new Item() { m_resource = CreateResource(_id), m_used = used, m_id = _id };
            m_pool.Add(item);

            // Update allocated count
  
            debugCurrentSize = m_pool.Count;

            return item;
        }        
        
        /// <summary>
        /// ID-> Some pools may need extra parameter: E.g: Instancing prefabs may need the ID/Name of the prefab. Just use that as third parameter.
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="used"></param>
        /// <returns></returns>
        protected virtual Item Create(T2 _id, VirtualPhenix.ResourceReference.VP_ResourceReferencer<T2, T1> _library, bool used = false)
        {
            // Create new pool item and add to pool

            var item = new Item() { m_resource = CreateResource(_id, _library), m_used = used, m_id = _id };
            m_pool.Add(item);

            // Update allocated count
  
            debugCurrentSize = m_pool.Count;

            return item;
        }

        /// <summary>
        /// ID-> Some pools may need extra parameter: E.g: Instancing prefabs may need the ID/Name of the prefab. Just use that as third parameter.
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="used"></param>
        /// <returns></returns>
        protected virtual Item Create(T2 _id, T1 _instanced, bool used = false)
        {
            // Create new pool item and add to pool

            var item = new Item() { m_resource = CreateResource(_id, _instanced), m_used = used, m_id = _id };
            m_pool.Add(item);

            // Update allocated count

            debugCurrentSize = m_pool.Count;

            return item;
        }

        protected virtual Item CreateAndInstanceInGPU(T2 _id, bool used = false)
        {
            // Create new pool item and add to pool

            var item = new Item() { m_resource = CreateResource(_id), m_used = used, m_id = _id };
            m_pool.Add(item);

            RegisterPrefab(ref item);

            // Update allocated count
            debugCurrentSize = m_pool.Count;

            return item;
        }

        protected virtual Item CreateAndInstanceInGPU(T1 _resource, T2 _id, bool used = false)
        {
            // Create new pool item and add to pool

            var item = new Item() { m_resource = _resource, m_used = used, m_id = _id };
            m_pool.Add(item);
            RegisterPrefab(ref item);
            // Update allocated count

            debugCurrentSize = m_pool.Count;

            return item;
        }


        protected virtual void RegisterPrefab(ref Item item)
        {
            item.m_useGPUInstancing = false;

#if GPU_INSTANCER
            if (m_gpuPrefabManager == null)
                m_gpuPrefabManager = GameObject.FindObjectOfType<GPUInstancer.GPUInstancerPrefabManager>();

            if (m_gpuPrefabManager != null && m_gpuPrefabManager.gameObject.activeInHierarchy && m_gpuPrefabManager.enabled)
            {

                if (item.m_resource is UnityEngine.Object)
                {
                    GPUInstancer.GPUInstancerPrefab prefab;

                    if (
                        (item.m_resource is UnityEngine.Component && (item.m_resource as UnityEngine.Component).TryGetComponent<GPUInstancer.GPUInstancerPrefab>(out prefab)) ||
                        (item.m_resource is UnityEngine.GameObject && (item.m_resource as UnityEngine.GameObject).TryGetComponent<GPUInstancer.GPUInstancerPrefab>(out prefab))
                       )
                    {
                        m_gpuPrefabManager.AddRegisteredPrefab(prefab);
                        item.m_useGPUInstancing = true;
                        item.m_gpuInstance = prefab;
                        m_gpuInstancedPrefabs.Add(prefab);
                    }
                 }
            }
#endif
        }

        protected virtual void DestroyFromGPUInstancer(Item item)
        {
#if GPU_INSTANCER
            if (m_gpuPrefabManager == null)
                m_gpuPrefabManager = GameObject.FindObjectOfType<GPUInstancer.GPUInstancerPrefabManager>();

            if (m_gpuPrefabManager != null && m_gpuPrefabManager.gameObject.activeInHierarchy && m_gpuPrefabManager.enabled)
                m_gpuPrefabManager.RemovePrefabInstance(item.m_gpuInstance);
#endif

            DestroyResource(item);
        }

        [System.Serializable]
        public class Item
        {
            public T2 m_id;
            public T1 m_resource;
            public bool m_used;
            public bool m_useGPUInstancing;
#if GPU_INSTANCER
            public GPUInstancer.GPUInstancerPrefab m_gpuInstance;
#endif
        }
    }

}
