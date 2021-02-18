using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.ResourceReference;

namespace VirtualPhenix.Misc
{
    [AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/Gameplay/Volumetric Blood Spawner")]
    public class VP_VolumetricBloodSpawner : VP_MonoBehaviour
    {
        [Header("Blood Prefabs"),Space]
        [SerializeField] protected string m_bloodDecalID = "Decal";
        [SerializeField] protected GameObject m_bloodDecalPrefab;
        [SerializeField] protected VP_PrefabResources m_bloods;
	    [SerializeField] protected bool m_random = true;
	    [SerializeField] protected bool m_sequential = false;
	    [SerializeField] protected int m_index = 0;
	    protected int m_currentIndex = 0;
	    
	    
        [Header("Blood Spawn Configuration"), Space]

        [SerializeField] protected bool m_overrideFloorY = true;
        [SerializeField] protected float m_bloodYPos = 0f;
        [SerializeField] protected bool m_useVFXPool = true;
        [SerializeField] protected float m_bloodTime = 0.75f;
        [SerializeField] protected Light m_dirLight;
#if USE_VOLUMETRIC_BLOOD
        [SerializeField] protected bool m_overrideAllSettings = true;
        [SerializeField] protected BFX_BloodSettings m_newBloodSettings;
#endif

        [Header("Decal"), Space]
        [SerializeField] protected bool m_useVFXPoolForDecal = true;
        [SerializeField] protected bool m_infiniteDecal = false;
        [SerializeField] protected float m_decalTime = 10f;

        protected Vector3 m_direction;

        protected virtual void Reset()
        {
            m_initializationTime = InitializationTime.None;

            m_bloodDecalID = VP_GameSetup.Misc.GameplayIds.BLOOD_DECAL_ID;

            foreach (Light l in FindObjectsOfType<Light>())
            {
                if (l.type == LightType.Directional)
                {
                    m_dirLight = l;
                    break;
                }
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected virtual Transform GetNearestObject(Transform hit, Vector3 hitPos)
        {
            var closestPos = 100f;
            Transform closestBone = null;
            var childs = hit.GetComponentsInChildren<Transform>();

            foreach (var child in childs)
            {
                var dist = Vector3.Distance(child.position, hitPos);
                if (dist < closestPos)
                {
                    closestPos = dist;
                    closestBone = child;
                }
            }

            var distRoot = Vector3.Distance(hit.position, hitPos);
            if (distRoot < closestPos)
            {
                closestPos = distRoot;
                closestBone = hit;
            }
            return closestBone;
        }

        public virtual void GenerateBlood(RaycastHit hit)
        {
            GenerateBlood(hit.point, hit.normal);
        }

        public virtual void GenerateBlood(Vector3 hitPoint, Vector3 hitNormal)
        {
            if (m_bloods == null)
            {
                Debug.LogError("No Blood resources");
                return;
            }
	        string bloodID = "";
	        GameObject bloodObj;
	        if (m_random)
	        {
	        	var kvp = m_bloods.GetRandomElementInResource();
		        bloodID= kvp.Key;
		        bloodObj = kvp.Value;
	        }
	        else
	        {
	        	if (m_sequential)
	        	{
	        		m_index++;
	        		
	        		if (m_index >= m_bloods.ResourceCount)
		        		m_index = 0;
	        	}
	        	
		        var kvp = m_bloods.GetElementInResourceAtIndex(m_index);
		        bloodID= kvp.Key;
		        bloodObj = kvp.Value;
	        }
            
	        if (bloodID.IsNullOrEmpty() || bloodObj == null)
		        return;
            
	        if (m_useVFXPool)
	        {
		        var it = VP_VFXPoolManager.Instance.Allocate(bloodID, m_bloods);

		        if (it != null)
		        {
			        SpawnBlood(it.m_resource, hitPoint, hitNormal, () => VP_VFXPoolManager.Instance.Release(it), bloodID);
		        }
	        }
	        else
	        {
		        var go = Instantiate(bloodObj);
		        SpawnBlood(go, hitPoint, hitNormal, ()=>Destroy(go), bloodID);
	        }
        }

        public virtual void SpawnBlood(GameObject go, Vector3 hitPoint, Vector3 hitNormal, UnityEngine.Events.UnityAction _OnWaitEnd, string _bloodID)
        {
            if (go != null)
            {
                // var randRotation = new Vector3(0, Random.value * 360f, 0);
                // var dir = CalculateAngle(Vector3.forward, hit.normal);
                float angle = Mathf.Atan2(hitNormal.x, hitNormal.z) * Mathf.Rad2Deg + 180;

                go.transform.SetPositionAndRotation(hitPoint, Quaternion.Euler(0, angle + 90, 0));
                go.SetActive(true);
#if USE_VOLUMETRIC_BLOOD
                if (go.TryGetComponent<BFX_BloodSettings>(out var bloodSettings))
                {
                    if (!m_overrideAllSettings || !m_newBloodSettings)
                    {
                        if (m_infiniteDecal && m_bloodDecalPrefab)
                            bloodSettings.FreezeDecalDisappearance = true;

                        if (m_overrideFloorY)
                            bloodSettings.GroundHeight = m_bloodYPos;

                    }
                    else
                    {
                        bloodSettings.FreezeDecalDisappearance = m_newBloodSettings.FreezeDecalDisappearance;
                        bloodSettings.GroundHeight = m_newBloodSettings.GroundHeight;
                        bloodSettings.ClampDecalSideSurface = m_newBloodSettings.ClampDecalSideSurface;
                        bloodSettings.DecalRenderinMode = m_newBloodSettings.DecalRenderinMode;
                        bloodSettings.AnimationSpeed = m_newBloodSettings.AnimationSpeed;
                    }

                    if (m_dirLight)
                        bloodSettings.LightIntensityMultiplier = m_dirLight.intensity;
                }
#endif
                var nearestBone = GetNearestObject(transform.root, hitPoint);
                if (nearestBone != null)
                {
                    if (m_bloodDecalPrefab != null)
                    {
                        if (m_useVFXPoolForDecal)
                        {
                            var it = VP_VFXPoolManager.Instance.Allocate(_bloodID + "_" + m_bloodDecalID, m_bloodDecalPrefab);

                            if (it != null)
                            {
                                var attachedInstance = it.m_resource;
                                SpawnDecal(attachedInstance, hitPoint, hitNormal, nearestBone, () =>
                                {
                                    if (!m_infiniteDecal)
                                    {
                                        VP_VFXPoolManager.Instance.Release(it);
                                    }
                                });
                            }
                        }
                        else
                        {
                            var attachedInstance = Instantiate(m_bloodDecalPrefab);
                            SpawnDecal(attachedInstance, hitPoint, hitNormal, nearestBone, () =>
                            {
                                if (!m_infiniteDecal)
                                {
                                    Destroy(attachedInstance);
                                }
                            });

                        }
                    }                    
                }

                VP_Utils.RunWaitTimeCoroutine(m_bloodTime, _OnWaitEnd, go);
            }
        }

        public virtual void SpawnDecal(GameObject attachBloodInstance, Vector3 hitPoint, Vector3 hitNormal, Transform nearestBone, UnityEngine.Events.UnityAction _OnWaitEnd)
        {
            var bloodT = attachBloodInstance.transform;
            bloodT.position = hitPoint;
            bloodT.localRotation = Quaternion.identity;
            bloodT.localScale = Vector3.one * Random.Range(0.75f, 1.2f);
            bloodT.LookAt(hitPoint + hitNormal, m_direction);
            bloodT.Rotate(90, 0, 0);
            bloodT.transform.parent = nearestBone;

            VP_Utils.RunWaitTimeCoroutine(m_decalTime, _OnWaitEnd, attachBloodInstance);
        }
    }

}