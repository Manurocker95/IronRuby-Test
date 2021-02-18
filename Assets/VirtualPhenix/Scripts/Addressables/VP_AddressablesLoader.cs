using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USE_ADDRESSABLES
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace VirtualPhenix
{
    [   AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+ "/Addressables/Addressables Loader"), 
        DefaultExecutionOrder(VP_ExecutingOrderSetup.ADDRESSABLES_MANAGER)]
    public class VP_AddressablesLoader : VP_SingletonMonobehaviour<VP_AddressablesLoader>
    {
        public virtual void LoadResourceAsync<T>(string _key, UnityEngine.Events.UnityAction<T> _callback)
        {
            StartCoroutine(LoadResourceAsyncIE(_key, _callback));
        }

        protected virtual IEnumerator LoadResourceAsyncIE<T>(string _key, UnityEngine.Events.UnityAction<T> _callback)
        {
            var op = Addressables.LoadAssetAsync<T>(_key);
            yield return op;
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                T data = op.Result;
                if (_callback != null)
                {
                    _callback.Invoke(data);
                }
            }
        }

        public virtual void LoadResourceAsync<T>(IResourceLocation _location, UnityEngine.Events.UnityAction<T> _callback)
        {
            StartCoroutine(LoadResourceAsyncIE(_location, _callback));
        }

        protected virtual IEnumerator LoadResourceAsyncIE<T>(IResourceLocation _location, UnityEngine.Events.UnityAction<T> _callback)
        {
            var op = Addressables.LoadAssetAsync<T>(_location);
            yield return op;
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                T data = op.Result;
                if (_callback != null)
                {
                    _callback.Invoke(data);
                }
            }
        }
    }
}
#endif