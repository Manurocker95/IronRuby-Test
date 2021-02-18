using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.ResourceReference;
namespace VirtualPhenix
{
    [DefaultExecutionOrder(VP_ExecutingOrderSetup.PREFAB_MANAGER),AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/Pooling/Prefab Pool Manager")]
    public class VP_PrefabPoolManager : VP_GenericPrefabPoolManager<string>
    {
        public override GameObject InstantiatePrefab(string _cast)
        {
            return Instantiate(m_prefabs.GetPrefab(_cast));
        }
    }
	[DefaultExecutionOrder(VP_ExecutingOrderSetup.PREFAB_MANAGER)]
    public class VP_GenericPrefabPoolManager<T> : VP_PoolManager<VP_GenericPrefabPoolManager<T>, GameObject, T>
    {
        [Header("Prefabs"),Space]
        [SerializeField] protected VP_PrefabResources m_prefabs;
        public bool m_inGPU = false;

        protected override void Reset()
        {
            m_creationTime = PoolCreationTime.None;
            m_destroyTime = PoolDestroyTime.OnDestroy;
            m_initializationTime = InitializationTime.Singleton;
        }

        public virtual Item AllocateInGPU(T cast, bool _deactiveOnAllocate = true)
        {
            m_inGPU = true;

            foreach (Item item in m_pool)
            {
                if (!item.m_id.Equals(cast) || item.m_used)
                    continue;

                // Return unused item
                item.m_used = true;
                return item;
            }

            // No unused items
            // Create if allowed

            if (m_resizable)
            {
                if (m_pool.Count < m_maxSize)
                {
                    if (m_logPoolExpansion)
                    {
                        Debug.Log($"{LogPoolName} full. Creating new {LogResourceName}. New size is {m_pool.Count + 1}.");
                    }

                    GameObject go = null;

                    go = InstantiatePrefab(cast);

                    if (!go)
                        return null;

                    if (_deactiveOnAllocate)
                        go.SetActive(false);

                    return CreateAndInstanceInGPU(go, cast, true);
                }

                VP_Debug.LogWarning($"{LogPoolName} full." + $"Resizing allowed but max size ({m_maxSize}) is reached. Returning null.");
                return null;
            }

            VP_Debug.LogWarning($"{LogPoolName} full. Resizing not allowed. Returning null.");
            return null;
        }

        public override Item Allocate(T cast)
        {
            foreach (Item item in m_pool)
            {
                if (!item.m_id.Equals(cast) || item.m_used)
                    continue;

                // Return unused item
                item.m_used = true;
                return item;
            }

            // No unused items
            // Create if allowed

            if (m_resizable)
            {
                if (m_pool.Count < m_maxSize)
                {
                    if (m_logPoolExpansion)
                    {
                        Debug.Log($"{LogPoolName} full. Creating new {LogResourceName}. New size is {m_pool.Count + 1}.");
                    }

                    GameObject go = null;

                    go = InstantiatePrefab(cast);

                    if (go != null)
                        go.SetActive(false);
                    else
                        return null;

                    return Create(go, cast, true);
                }

                VP_Debug.LogWarning($"{LogPoolName} full." + $"Resizing allowed but max size ({m_maxSize}) is reached. Returning null.");
                return null;
            }

            VP_Debug.LogWarning($"{LogPoolName} full. Resizing not allowed. Returning null.");
            return null;
        }

        public virtual GameObject InstantiatePrefab(T _cast)
        {
            return null;
        }

        public override void Release(Item item)
        {
            base.Release(item);
            item.m_resource.SetActive(false);
        }

        protected override void DestroyResource(GameObject resource)
        {
            base.DestroyResource(resource);
            Destroy(resource);
        }
    }
}
