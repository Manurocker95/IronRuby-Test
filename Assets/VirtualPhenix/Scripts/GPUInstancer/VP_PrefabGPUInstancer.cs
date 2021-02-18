using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if GPU_INSTANCER
using GPUInstancer;
#endif

namespace VirtualPhenix.Integrations.GPUInstancing
{
    [DefaultExecutionOrder(VP_ExecutingOrderSetup.GPU_INSTANCER), AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/Prefab/GPU Instancer")]
    public class VP_PrefabGPUInstancer : VP_SingletonMonobehaviour<VP_PrefabGPUInstancer>
    {
        [Header("GPU instancer"), Space]
        [SerializeField] protected bool m_spawnOnInit = true;
        [SerializeField] protected bool m_random = true;

#if GPU_INSTANCER
        [SerializeField] protected List<GPUInstancerPrefab> m_spawningPrefabs = new List<GPUInstancerPrefab>();
        [SerializeField] protected GPUInstancerPrefabManager m_prefabManager;

        protected List<GPUInstancerPrefab> m_spawnedInstances = new List<GPUInstancerPrefab>();
#endif

        [SerializeField, Range(0, 200000)] protected int m_count = 50000;     
        [SerializeField] protected Transform m_parentTrs = null;

#if GPU_INSTANCER
        public int SpawnedInstancesCount { get { return m_spawnedInstances.Count; } }
#endif

        protected override void Initialize()
        {
            base.Initialize();

            if (m_spawnOnInit)
            {
#if GPU_INSTANCER
                m_spawnedInstances.Clear();
                if (m_spawningPrefabs.Count > 0)
                {
                    for (int i = 0; i < m_count; i++)
                    {
                        Vector3 pos = Vector3.zero;
                        Vector3 scale = Vector3.zero;
                        Quaternion rot = Quaternion.identity;
                        GetSpawnTransformFunction(out pos, out rot, out scale);
                        var prefab = m_random ? m_spawningPrefabs[Random.Range(0, m_spawningPrefabs.Count)] : m_spawningPrefabs[0];
                        m_spawnedInstances.Add(InstantiateInGPU(prefab, pos, rot, scale));
                    }

                    Debug.Log("Instantiated " + SpawnedInstancesCount + " objects.");
                }
#endif
            }
        }

        protected virtual void GetSpawnTransformFunction(out Vector3 position, out Quaternion rotation, out Vector3 scale)
        {
            position = Vector3.zero;
            rotation = Quaternion.identity;
            scale = Vector3.one;
        }

        protected override void Start()
        {
            base.Start();
#if GPU_INSTANCER
            if (m_prefabManager != null && m_prefabManager.gameObject.activeSelf && m_prefabManager.enabled)
            {
                GPUInstancerAPI.RegisterPrefabInstanceList(m_prefabManager, m_spawnedInstances);
                GPUInstancerAPI.InitializeGPUInstancer(m_prefabManager);
            }
#endif
        }

#if GPU_INSTANCER
        protected virtual GPUInstancerPrefab InstantiateInGPU(GPUInstancerPrefab prefab, Vector3 position, Quaternion rotation, Vector3 scale, Transform _parent = null)
        {
            var m_allocatedGO = Instantiate(prefab, position, rotation);
            SetTransform(ref m_allocatedGO, position, rotation, scale);
            return m_allocatedGO;
        }

        protected virtual void SetTransform(ref GPUInstancerPrefab instanced, Vector3 position, Quaternion rotation, Vector3 scale)
        {
      
        }
#endif
    }
}
