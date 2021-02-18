using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.ResourceReference;

namespace VirtualPhenix
{
    [DefaultExecutionOrder(VP_ExecutingOrderSetup.VFX_POOL_MANAGER), AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/VFX/VFX Pool Manager")]
    public class VP_VFXPoolManager : VP_VFXPoolManagerBase<string>
    {
        public static new VP_VFXPoolManager Instance
        {
            get
            {
                return (VP_VFXPoolManager)m_instance;
            }
        }
    }

    [DefaultExecutionOrder(VP_ExecutingOrderSetup.VFX_POOL_MANAGER), AddComponentMenu("")]
    public class VP_VFXPoolManagerBase<T> : VP_PoolManager<VP_VFXPoolManagerBase<T>, GameObject, T>
    {
        [SerializeField] protected VP_ResourceReferencer<T, GameObject> m_database;

        protected override void Reset()
        {
            m_creationTime = PoolCreationTime.None;
            m_destroyTime = PoolDestroyTime.OnDestroy;
            m_initializationTime = InitializationTime.Singleton;
        }

        public virtual GameObject InstantiateVFX(T _cast, VirtualPhenix.ResourceReference.VP_ResourceReferencer<T, GameObject> _library)
        {
           GameObject go = _library != null ? _library.GetResource(_cast) : null;
            return go != null ? Instantiate(go) : null;
        }

        public virtual GameObject InstantiateVFX (T _cast)
        {
            GameObject go = m_database != null ? m_database.GetResource(_cast) : null;
            return go != null ? Instantiate(go) : null;
        }

        public virtual GameObject InstantiateVFX(T _cast, GameObject _resource)
        {
            return _resource != null ? Instantiate(_resource): null;
        }

        public virtual GameObject InstantiateVFX(T _cast, GameObject _resource, Transform _parent)
        {
            return _resource != null ? Instantiate(_resource, _parent) : null;
        }

        public virtual GameObject InstantiateVFX(T _cast, GameObject _resource, Vector3 position, Quaternion _rotation)
        {
            return _resource != null ? Instantiate(_resource, position, _rotation) : null;
        }

        public virtual GameObject InstantiateVFX(T _cast, VirtualPhenix.ResourceReference.VP_ResourceReferencer<T, GameObject> _library, Transform _parent)
        {
            return _library != null ? Instantiate(_library.GetResource(_cast), _parent) : null;
        }

        public virtual GameObject InstantiateVFX(T _cast, VirtualPhenix.ResourceReference.VP_ResourceReferencer<T, GameObject> _library, Vector3 position, Quaternion _rotation)
        {
            return _library != null ? Instantiate(_library.GetResource(_cast), position, _rotation) : null;
        }

        public virtual GameObject InstantiateVFX(T _cast, Vector3 position, Quaternion _rotation)
        {
            GameObject go = m_database != null ? m_database.GetResource(_cast) : null;
            return go != null ? Instantiate(go, position, _rotation) : null;
        }

        protected override GameObject CreateResource(T _id)
        {
            return InstantiateVFX(_id);
        }

        protected override GameObject CreateResource(T _id, VP_ResourceReferencer<T, GameObject> _library)
        {
            return InstantiateVFX(_id, _library);
        }

        protected override GameObject CreateResource(T _id, GameObject _instanced)
        {
            return InstantiateVFX(_id, _instanced);
        }

        public override GameObject CreateDefaultResource(GameObject _other)
        {
            return Instantiate(_other);
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
