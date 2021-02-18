#if USE_CUSTOM_ASSET_BUNDLES_LOADER
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace VirtualPhenix.Bundles
{

	public abstract class VP_AssetBundleLoadOperation : IEnumerator
	{
		public object Current
		{
			get
			{
				return null;
			}
		}
		public bool MoveNext()
		{
			return !IsDone();
		}

		public void Reset()
		{
		}

		abstract public bool Update();

		abstract public bool IsDone();
	}

	public class VP_AssetBundleLoadLevelSimulationOperation : VP_AssetBundleLoadOperation
	{
		public VP_AssetBundleLoadLevelSimulationOperation()
		{
		}

		public override bool Update()
		{
			return false;
		}

		public override bool IsDone()
		{
			return true;
		}
	}

	public class VP_AssetBundleLoadLevelOperation : VP_AssetBundleLoadOperation
	{
		protected string m_AssetBundleName;
		protected string m_LevelName;
		protected bool m_IsAdditive;
		protected string m_DownloadingError;
		protected AsyncOperation m_Request;

		public VP_AssetBundleLoadLevelOperation(string assetbundleName, string levelName, bool isAdditive)
		{
			m_AssetBundleName = assetbundleName;
			m_LevelName = levelName;
			m_IsAdditive = isAdditive;
		}

		public override bool Update()
		{
			//if (AssetBundleManager.UnloadingInProcess)
			//	return false;

			if (m_Request != null)
				return false;

			VP_LoadedAssetBundle bundle = VP_AssetBundleManager.Instance.GetLoadedAssetBundle(m_AssetBundleName, out m_DownloadingError);
			if (bundle != null)
			{
				if (m_IsAdditive)
					m_Request = SceneManager.LoadSceneAsync(m_LevelName, LoadSceneMode.Additive);
				else
					m_Request = SceneManager.LoadSceneAsync(m_LevelName);
				return false;
			}
			else
				return true;
		}

		public override bool IsDone()
		{
			// Return if meeting downloading error.
			// m_DownloadingError might come from the dependency downloading.
			if (m_Request == null && m_DownloadingError != null)
			{
				//Debug.LogError(m_DownloadingError);
				return true;
			}

			return m_Request != null && m_Request.isDone;
		}
	}

	public abstract class VP_AssetBundleLoadAssetOperation : VP_AssetBundleLoadOperation
	{
		public abstract T GetAsset<T>() where T : UnityEngine.Object;
	}

	public class VP_AssetBundleLoadAssetOperationSimulation : VP_AssetBundleLoadAssetOperation
	{
		Object m_SimulatedObject;

		public VP_AssetBundleLoadAssetOperationSimulation(Object simulatedObject)
		{
			m_SimulatedObject = simulatedObject;
		}

		public override T GetAsset<T>()
		{
			return m_SimulatedObject as T;
		}

		public override bool Update()
		{
			return false;
		}

		public override bool IsDone()
		{
			return true;
		}
	}

	public class VP_AssetBundleLoadAssetOperationFull : VP_AssetBundleLoadAssetOperation
	{
		protected string m_AssetBundleName;
		protected string m_AssetName;
		protected string m_DownloadingError;
		protected System.Type m_Type;
		protected AssetBundleRequest m_Request = null;
		protected string m_storePath = null;

		public VP_AssetBundleLoadAssetOperationFull(string bundleName, string assetName, System.Type type)
		{
			m_AssetBundleName = bundleName;
			m_AssetName = assetName;
			m_Type = type;
		}

		public override T GetAsset<T>()
		{
			if (m_Request != null && m_Request.isDone)
				return m_Request.asset as T;
			else
				return null;
		}

		// Returns true if more Update calls are required.
		public override bool Update()
		{
			//if (AssetBundleManager.UnloadingInProcess)
			//	return false;

			if (m_Request != null)
				return false;

			VP_LoadedAssetBundle bundle = VP_AssetBundleManager.Instance.GetLoadedAssetBundle(m_AssetBundleName, out m_DownloadingError);
			if (bundle != null)
			{
				// Asset bundle not loaded due to error in internet connection
				if (bundle.m_AssetBundle != null)
				{
					m_Request = bundle.m_AssetBundle.LoadAssetAsync(m_AssetName, m_Type);
					return false;
				}
				else
				{
					return true;
				}
			}
			else
			{
				return true;
			}
		}

		public override bool IsDone()
		{
			// Return if meeting downloading error.
			// m_DownloadingError might come from the dependency downloading.
			if (m_Request == null && m_DownloadingError != null)
			{
				//Debug.LogError(m_DownloadingError);
				return true;
			}

			return m_Request != null && m_Request.isDone;
		}
	}

	public class VP_AssetBundleLoadManifestOperation : VP_AssetBundleLoadAssetOperationFull
	{
		public VP_AssetBundleLoadManifestOperation(string bundleName, string assetName, System.Type type)
			: base(bundleName, assetName, type)
		{
		}

		public override bool Update()
		{
			//if (AssetBundleManager.UnloadingInProcess)
			//	return false;

			base.Update();

			if (m_Request != null && m_Request.isDone)
			{
				//			Debug.Log (">>>>>>>>>>>>>>>>>>>>>>>>>> AssetBundleManifest LOADED!!!!!!! " + m_storePath);
				VP_AssetBundleManager.Instance.AssetBundleManifestObject = GetAsset<AssetBundleManifest>();

				return false;
			}
			else
				return true;
		}
	}


}
#endif