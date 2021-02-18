#if USE_CUSTOM_ASSET_BUNDLES_LOADER
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UIElements;
using UnityEngine.Networking;

namespace VirtualPhenix.Bundles
{

	// Loaded assetBundle contains the references count which can be used to unload dependent assetBundles automatically.
	public class VP_LoadedAssetBundle
	{
		public AssetBundle m_AssetBundle;
		public int m_ReferencedCount;

		public VP_LoadedAssetBundle(AssetBundle assetBundle)
		{
			m_AssetBundle = assetBundle;
			m_ReferencedCount = 1;
		}
	}


	public class VP_AssetBundleManager : VP_SingletonMonobehaviour<VP_AssetBundleManager>
	{
		const string kSimulateAssetBundles = "SimulateAssetBundles";

		[SerializeField] protected bool m_debug = true;

		[SerializeField] protected string m_baseDownloadingURL = "";
		[SerializeField] protected string m_manifestFolder = "";
		[SerializeField] protected string m_manifestAsset = "";
		[SerializeField] protected string m_manifestAssetURL = "";
		[SerializeField] protected string[] m_variants = { };
		// Was manifest loaded or not
		[SerializeField] protected bool m_isManifestLoaded;
		[SerializeField] protected AssetBundleManifest m_assetBundleManifest = null;
#if UNITY_EDITOR
		[SerializeField] protected int m_simulateAssetBundleInEditor = -1;

#endif

		[SerializeField] protected Dictionary<string, VP_LoadedAssetBundle> m_loadedAssetBundles = new Dictionary<string, VP_LoadedAssetBundle>();
		[SerializeField] protected Dictionary<string, UnityWebRequest> m_downloadingWWWs = new Dictionary<string, UnityWebRequest>();
		[SerializeField] protected Dictionary<string, string> m_downloadingErrors = new Dictionary<string, string>();
		[SerializeField] protected List<VP_AssetBundleLoadOperation> m_inProgressOperations = new List<VP_AssetBundleLoadOperation>();
		[SerializeField] protected Dictionary<string, string[]> m_dependencies = new Dictionary<string, string[]>();

		public Dictionary<string, UnityWebRequest> DownloadingWWWs
		{
			get { return m_downloadingWWWs; }
		}

		[SerializeField] protected bool m_remoteAsset = true;

		[SerializeField] protected Dictionary<string, string> m_assetsToDisk = new Dictionary<string, string>();

		[SerializeField] protected string m_downloadingProcess;
		[SerializeField] protected static bool m_showProgress;
		[SerializeField] protected bool m_unloadingInProcess = false;
		protected bool m_deletingBundles = false;

		public bool ShowProgress
		{
			get { return m_showProgress; }
			set { m_showProgress = value; }
		}

		public bool DeletingBundles
		{
			get { return m_deletingBundles; }
			set { m_deletingBundles = value; }
		}

		// The base downloading url which is used to generate the full downloading url with the assetBundle names.
		public string BaseDownloadingURL
		{
			get { return m_baseDownloadingURL; }
			set { m_baseDownloadingURL = value; }
		}

		public string ManifestFolder
		{
			get { return m_manifestFolder; }
			set { m_manifestFolder = value; }
		}

		public string ManifestAssetURL
		{
			get { return m_manifestAssetURL; }
			set { m_manifestAssetURL = value; }
		}

		public string ManifestAsset
		{
			get { return m_manifestAsset; }
			set { m_manifestAsset = value; }
		}

		// Variants which is used to define the active variants.
		public string[] Variants
		{
			get { return m_variants; }
			set { m_variants = value; }
		}

		// AssetBundleManifest object which can be used to load the dependecies and check suitable assetBundle variants.
		public AssetBundleManifest AssetBundleManifestObject
		{
			set { m_assetBundleManifest = value; }
			get { return m_assetBundleManifest; }
		}

		public bool UnloadingInProcess
		{
			get { return m_unloadingInProcess; }
		}

#if UNITY_EDITOR
		// Flag to indicate if we want to simulate assetBundles in Editor without building them actually.
		public virtual bool SimulateAssetBundleInEditor
		{
			get
			{
				if (m_simulateAssetBundleInEditor == -1)
					m_simulateAssetBundleInEditor = EditorPrefs.GetBool(kSimulateAssetBundles, true) ? 1 : 0;

				return m_simulateAssetBundleInEditor != 0;
			}
			set
			{
				int newValue = value ? 1 : 0;
				if (newValue != m_simulateAssetBundleInEditor)
				{
					m_simulateAssetBundleInEditor = newValue;
					EditorPrefs.SetBool(kSimulateAssetBundles, value);
				}
			}
		}
#endif

		// Get loaded AssetBundle, only return vaild object when all the dependencies are downloaded successfully.
	     public virtual VP_LoadedAssetBundle GetLoadedAssetBundle(string assetBundleName, out string error, bool noDepedencies = false)
		{
			if (m_downloadingErrors.TryGetValue(assetBundleName, out error))
				return null;

			VP_LoadedAssetBundle bundle = null;
			m_loadedAssetBundles.TryGetValue(assetBundleName, out bundle);
			if (bundle == null)
				return null;

			// No dependencies are recorded, only the bundle itself is required.
			string[] dependencies = null;
			if (!m_dependencies.TryGetValue(assetBundleName, out dependencies) || noDepedencies)
				return bundle;

			// Make sure all dependencies are loaded
			foreach (var dependency in dependencies)
			{
				if (m_downloadingErrors.TryGetValue(assetBundleName, out error))
					return bundle;

				// Wait all the dependent assetBundles being loaded.
				VP_LoadedAssetBundle dependentBundle;
				m_loadedAssetBundles.TryGetValue(dependency, out dependentBundle);
				if (dependentBundle == null)
					return null;
			}

			return bundle;
		}

		// Load AssetBundleManifest.
		public virtual VP_AssetBundleLoadManifestOperation Initialize(string manifestAssetBundleName, bool connectionToManifest = false)
		{
			if (!GameObject.Find("AssetBundleManager"))
			{
				var go = new GameObject("AssetBundleManager", typeof(VP_AssetBundleManager));
				m_unloadingInProcess = false;
				Object.DontDestroyOnLoad(go);
			}

			// Not deleting bundles
			//deletingBundles = false;

#if UNITY_EDITOR
			// If we're in Editor simulation mode, we don't need the manifest assetBundle.
			if (SimulateAssetBundleInEditor)
				return null;
#endif

			LoadAssetBundle(manifestAssetBundleName, true, connectionToManifest);

			var operation = new VP_AssetBundleLoadManifestOperation(manifestAssetBundleName, "AssetBundleManifest", typeof(AssetBundleManifest));
			m_inProgressOperations.Add(operation);
			return operation;
		}

		// Load AssetBundleManifest.
		public virtual VP_AssetBundleLoadManifestOperation LoadVoidBundle(string manifestAssetBundleName, string path, bool voidAssetBundle = false, bool isManifest = false, bool installingManifest = false)
		{
			if (!GameObject.Find("AssetBundleManager"))
			{
				var go = new GameObject("AssetBundleManager", typeof(VP_AssetBundleManager));
				Object.DontDestroyOnLoad(go);
			}

			// Deleting bundles
			//deletingBundles = true;

#if UNITY_EDITOR
			// If we're in Editor simulation mode, we don't need the manifest assetBundle.
			if (SimulateAssetBundleInEditor)
				return null;
#endif

			LoadVoidAssetBundle(manifestAssetBundleName, path, voidAssetBundle, isManifest, installingManifest);

			var operation = new VP_AssetBundleLoadManifestOperation(manifestAssetBundleName, "AssetBundleManifest", typeof(AssetBundleManifest));
			m_inProgressOperations.Add(operation);
			return operation;
		}

		// Load AssetBundle and its dependencies.
		protected virtual void LoadVoidAssetBundle(string assetBundleName, string path, bool voidAssetBundle, bool isManifest, bool installingManifest)
		{
#if UNITY_EDITOR
			// If we're in Editor simulation mode, we don't have to really load the assetBundle and its dependencies.
			if (SimulateAssetBundleInEditor)
				return;
#endif

			// Check if the assetBundle has already been processed.
			LoadVoidAssetBundleInternal(assetBundleName, path, voidAssetBundle, isManifest, installingManifest);
		}

		// Load AssetBundle and its dependencies.
		protected virtual void LoadAssetBundle(string assetBundleName, bool isLoadingAssetBundleManifest = false, bool connectionToManifest = false)
		{
#if UNITY_EDITOR
			// If we're in Editor simulation mode, we don't have to really load the assetBundle and its dependencies.
			if (SimulateAssetBundleInEditor)
				return;
#endif
			if (!isLoadingAssetBundleManifest)
				assetBundleName = RemapVariantName(assetBundleName);

			// Check if the assetBundle has already been processed.
			bool isAlreadyProcessed = LoadAssetBundleInternal(assetBundleName, isLoadingAssetBundleManifest, connectionToManifest);

			// Load dependencies.
			if (!isAlreadyProcessed && !isLoadingAssetBundleManifest)
				LoadDependencies(assetBundleName);
		}

		// Remaps the asset bundle name to the best fitting asset bundle variant.
		protected virtual string RemapVariantName(string assetBundleName)
		{
			// If there is no connection, manifest is not inizialized
			if (m_assetBundleManifest == null)
				return assetBundleName;

			string[] bundlesWithVariant = m_assetBundleManifest.GetAllAssetBundlesWithVariant();

			// If the asset bundle doesn't have variant, simply return.
			if (System.Array.IndexOf(bundlesWithVariant, assetBundleName) < 0)
				return assetBundleName;

			string[] split = assetBundleName.Split('.');

			int bestFit = int.MaxValue;
			int bestFitIndex = -1;
			// Loop all the assetBundles with variant to find the best fit variant assetBundle.
			for (int i = 0; i < bundlesWithVariant.Length; i++)
			{
				string[] curSplit = bundlesWithVariant[i].Split('.');
				if (curSplit[0] != split[0])
					continue;

				int found = System.Array.IndexOf(m_variants, curSplit[1]);
				if (found != -1 && found < bestFit)
				{
					bestFit = found;
					bestFitIndex = i;
				}
			}

			if (bestFitIndex != -1)
				return bundlesWithVariant[bestFitIndex];
			else
				return assetBundleName;
		}

		// Where we actuall call WWW to download the assetBundle.
		protected virtual bool LoadVoidAssetBundleInternal(string assetBundleName, string path, bool voidAssetBundle, bool isManifest, bool installingManifest)
		{
			// Already loaded.
			VP_LoadedAssetBundle bundle = null;
			m_loadedAssetBundles.TryGetValue(assetBundleName, out bundle);

			if (bundle != null)
			{
				bundle.m_ReferencedCount++;
				return true;
			}

			// @TODO: Do we need to consider the referenced count of WWWs?
			// We never have duplicate WWWs as we wait LoadAssetAsync()/LoadLevelAsync() to be finished before calling another LoadAssetAsync()/LoadLevelAsync().
			// But in the real case, users can call LoadAssetAsync()/LoadLevelAsync() several times then wait them to be finished which might have duplicate WWWs.
			if (m_downloadingWWWs.ContainsKey(assetBundleName))
				return true;

			string url = path + "/" + assetBundleName;
			m_isManifestLoaded = true;

			if (m_debug)
			{
				Debug.Log("URL " + url);
			}

			StartCoroutine(DownloadBundleOrCacheCoroutine(url, assetBundleName, installingManifest, voidAssetBundle, isManifest));

			return false;
		}

		// Where we actual call WWW to download the assetBundle.
		protected virtual bool LoadAssetBundleInternal(string assetBundleName, bool isLoadingAssetBundleManifest, bool connectionToManifest = false)
		{
			// Already loaded.
			VP_LoadedAssetBundle bundle = null;
			m_loadedAssetBundles.TryGetValue(assetBundleName, out bundle);

			if (bundle != null)
			{
				bundle.m_ReferencedCount++;
				return true;
			}

			// @TODO: Do we need to consider the referenced count of WWWs?
			// We never have duplicate WWWs as we wait LoadAssetAsync()/LoadLevelAsync() to be finished before calling another LoadAssetAsync()/LoadLevelAsync().
			// But in the real case, users can call LoadAssetAsync()/LoadLevelAsync() several times then wait them to be finished which might have duplicate WWWs.
			if (m_downloadingWWWs.ContainsKey(assetBundleName))
				return true;

			string url;

			// If we are downloading main manifest asset
			if (isLoadingAssetBundleManifest)
			{
				// If we are loading app at first time, then try to update manifest
				// if we have connection download new manifest from server
				if (connectionToManifest)
				{
					if (m_debug)
						Debug.Log("We have connection, trying to update manifest");
					url = m_baseDownloadingURL + assetBundleName;
				}
				else
				{
					// If manifest exists and we have no internet connection, get manifest from cache
					if (File.Exists(ManifestAsset))
					{
						if (m_debug)
							Debug.Log("Exists local manifest " + ManifestAsset);
						url = m_manifestAssetURL + assetBundleName;
					}
					// If not exists manifest try to get from server
					else
					{
						if (m_debug)
							Debug.Log("NOT Exists local manifest " + ManifestAsset);

						url = m_baseDownloadingURL + assetBundleName;
					}
				}
			}
			// Other assets different than main manifest
			else
			{
				// Check if content is downloaded
				// @TODO: Later on File.Exists check have to be removed (performance boost)

				// Permament cache disabled due to large load times
				/*if ((LibraryConfig.Instance.SelectedLibraryContent.Bundle.isInstalled) && (File.Exists(ManifestFolder + assetBundleName)))
					url = m_ManifestAssetURL + assetBundleName;
				else*/
				url = m_baseDownloadingURL + assetBundleName;
			}

			if (m_debug)
			{
				//Debug.Log("baseURL" + m_BaseDownloadingURL);
				//Debug.Log("ABName " + assetBundleName); 
				Debug.Log("URL " + url);
			}

			StartCoroutine(DownloadBundleOrCacheCoroutine(url, assetBundleName, isLoadingAssetBundleManifest));

			return false;
		}

		public virtual IEnumerator DownloadBundleOrCacheCoroutine(string url, string assetBundleName, bool isLoadingAssetBundleManifest, bool voidAssetBundle = false, bool isVoidManifest = false)
		{
			/*yield return*/
			StartCoroutine(DownloadBundleOrCache(url, assetBundleName, isLoadingAssetBundleManifest, voidAssetBundle, isVoidManifest));

			yield return null;
		}

		public virtual IEnumerator DownloadBundleOrCache(string url, string assetBundleName, bool isLoadingAssetBundleManifest, bool voidAssetBundle, bool isVoidManifest)
		{
			UnityWebRequest download = null;

			if (isLoadingAssetBundleManifest)
			{
				//			Debug.Log(">>> downloading AB");

				download = new UnityWebRequest(url);

				// Wait until download has finished
				yield return download.SendWebRequest();

				//Debug.Log(">>> downloading AB end " + download.bytesDownloaded);

				m_isManifestLoaded = false;

				if (string.IsNullOrEmpty(download.error))
				{
					if (File.Exists(ManifestAsset))
					{
						if (m_debug)
							Debug.Log("File exists >>> " + m_manifestAsset);

						m_isManifestLoaded = true;

						if (download.isDone)
						{
							if (m_debug)
								Debug.Log("Download manifest");

							var file = File.Open(ManifestAsset, FileMode.Create);
							file.Write(download.downloadHandler.data, 0, download.downloadHandler.data.Length);
							file.Close();

							// If Iphone device don't create backup on ICloud/ITunes 
#if UNITY_IPHONE
							UnityEngine.iOS.Device.SetNoBackupFlag(ManifestAsset);
#endif
						}
					}
					else
					{
						//Debug.Log ("manifest folder " + ManifestFolder);
						if (!Directory.Exists(ManifestFolder))
							Directory.CreateDirectory(ManifestFolder);

						// If manifest is downloaded
						if (download.isDone)
						{
							var file = File.Open(ManifestAsset, FileMode.Create);
							file.Write(download.downloadHandler.data, 0, download.downloadHandler.data.Length);
							file.Close();

							// If Iphone device don't create backup on ICloud/ITunes 
#if UNITY_IPHONE
							UnityEngine.iOS.Device.SetNoBackupFlag(ManifestAsset);
#endif

							m_isManifestLoaded = true;
						}
						else
						{
							if (m_debug)
								Debug.Log("[DownloadBundleOrCache] Error loading manifest " + assetBundleName + " " + ", zero bytes content: " + download.downloadedBytes);
						}
					}
				}
				else
				{
					if (m_debug)
						Debug.Log("[DownloadBundleOrCache] Error loading manifest: " + assetBundleName + " " + download.error);
				}

			}
			else
			{
				// If manifest is not loaded, then return
				// Only if not are deleting asset bundle on iOS/Android 
				if (!voidAssetBundle && ((m_assetBundleManifest == null) || !m_isManifestLoaded))
				{
					//				Debug.Log ("MANIFEST NOT LOADED");
					yield return null;
				}
				else
				{
					uint crc = 0;

					// Deleting asset bundle from cache
					if (voidAssetBundle)
					{
						// WEIRD! Force error to delete cached asset bundle
						crc = 666;

						if (m_debug)
							Debug.Log("bundle: " + url + " deleting.");

						// Loading manifest
						if (isVoidManifest)
							download = new UnityWebRequest(url);
						// Deleting asset bundles
						else
							download = UnityWebRequestAssetBundle.GetAssetBundle(url, m_assetBundleManifest.GetAssetBundleHash(assetBundleName), crc);
					}
					// Loading asset bundle from cache or download
					else                    //else if ((LibraryConfig.Instance.SelectedLibraryContent.Bundle.isInstalled) || assetsToDisk.ContainsKey(assetBundleName))
					{
						if (m_debug)
							Debug.Log("bundle: " + url + " loading from cache");

						download = UnityWebRequestAssetBundle.GetAssetBundle(url, m_assetBundleManifest.GetAssetBundleHash(assetBundleName), 0);

						BundleLoaded(assetBundleName, m_assetBundleManifest.GetAssetBundleHash(assetBundleName).ToString(), assetBundleName);

						if (m_debug)
							Debug.Log("Hash >>>>>>>>>>>> assetBundleName " + assetBundleName + " : " + m_assetBundleManifest.GetAssetBundleHash(assetBundleName));
					}

					//				Debug.Log(">>> downloading AB end cache");			
				}
			}

			if (m_isManifestLoaded)
			{
				//Debug.Log(">>>>>>>>>>>" + assetBundleName);
				UnityWebRequest bundleTemp;
				if (!m_downloadingWWWs.TryGetValue(assetBundleName, out bundleTemp))
				{
					m_downloadingWWWs.Add(assetBundleName, download);
				}
				//Debug.Log ("m_Downloading " + download.bytesDownloaded);			
			}
		}

		public void BundleLoaded(string bundleName, string bundleHash, string bundle)
		{
		}

		// Where we get all the dependencies and load them all.
		protected void LoadDependencies(string assetBundleName)
		{

			if (m_assetBundleManifest == null)
			{
				if (m_debug)
					Debug.Log("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
				return;
			}

			// Get dependecies from the AssetBundleManifest object..
			string[] dependencies = m_assetBundleManifest.GetAllDependencies(assetBundleName);
			if (dependencies.Length == 0)
				return;

			for (int i = 0; i < dependencies.Length; i++)
				//Debug.Log ("dep >> " + dependencies[i]);
				dependencies[i] = RemapVariantName(dependencies[i]);

			// Record and load all dependencies.
			m_dependencies.Add(assetBundleName, dependencies);
			for (int i = 0; i < dependencies.Length; i++)
				LoadAssetBundleInternal(dependencies[i], false);
		}

		private void StoreToDisk(string bundlePath)
		{
			string bundleStoreLocalPath = ManifestFolder + GetBundleName(bundlePath);

			if (!File.Exists(bundleStoreLocalPath))
				StartCoroutine(StoreToDiskImpl(bundlePath, bundleStoreLocalPath));
		}

		private IEnumerator StoreToDiskImpl(string bundlePath, string bundleStoreLocalPath)
		{
			if (m_debug)
				Debug.Log("Start download of " + bundlePath);
			UnityWebRequest download = new UnityWebRequest(bundlePath);
			download.SendWebRequest();

			yield return download;

			if (m_debug)
				Debug.Log("End download of " + bundlePath);
			if ((string.IsNullOrEmpty(download.error)) && (download.downloadedBytes > 0))
			{
				//File.Delete(bundleStoreLocalPath);
				if (m_debug)
					Debug.Log("Storing on disk " + bundleStoreLocalPath);
				var file = File.Open(bundleStoreLocalPath, FileMode.Create);
				file.Write(download.downloadHandler.data, 0, download.downloadHandler.data.Length);
				file.Close();
				// If Iphone device don't create backup on ICloud/ITunes 
#if UNITY_IPHONE
				// TODO: UnityEngine.iOS.Device.SetNoBackupFlag(bundleStoreLocalPath);
#endif
			}
			else if (m_debug)
			{
				Debug.Log("Error downloading " + bundlePath + " " + download.error);
			}

			download.Dispose();
		}


		private void StoreToDisk2(string bundlePath, UnityWebRequest downloadBundle)
		{
			string bundleStoreLocalPath = ManifestFolder + GetBundleName(bundlePath);

			if (!File.Exists(bundleStoreLocalPath))
				StartCoroutine(SaveBundle(bundleStoreLocalPath, downloadBundle));
		}

		private IEnumerator SaveBundle(string bundleStoreLocalPath, UnityWebRequest download)
		{
			if (m_debug)
				Debug.Log("Storing on disk " + bundleStoreLocalPath);
			var file = File.Open(bundleStoreLocalPath, FileMode.Create);
			file.Write(download.downloadHandler.data, 0, download.downloadHandler.data.Length);
			file.Close();
			// If Iphone device don't create backup on ICloud/ITunes 
#if UNITY_IPHONE
		// TODO: UnityEngine.iOS.Device.SetNoBackupFlag(bundleStoreLocalPath);
#endif

			yield return null;
		}


		private string GetBundleName(string pathName)
		{
			return pathName.Substring(pathName.LastIndexOf("/") + 1);
		}

		// Unload assetbundle and its dependencies.
		public void UnloadAssetBundle(string assetBundleName)
		{
#if UNITY_EDITOR
			// If we're in Editor simulation mode, we don't have to load the manifest assetBundle.
			if (SimulateAssetBundleInEditor)
				return;
#endif

			//Debug.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory before unloading " + assetBundleName);

			UnloadAssetBundleInternal(assetBundleName);
			UnloadDependencies(assetBundleName);

			//Debug.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory after unloading " + assetBundleName);
		}

		protected void UnloadDependencies(string assetBundleName, bool unloadAllLoadedObjects = false)
		{
			string[] dependencies = null;
			if (!m_dependencies.TryGetValue(assetBundleName, out dependencies))
				return;

			// Loop dependencies.
			foreach (var dependency in dependencies)
			{
				UnloadAssetBundleInternal(dependency, unloadAllLoadedObjects);
			}

			m_dependencies.Remove(assetBundleName);
		}

		protected void UnloadAssetBundleInternal(string assetBundleName, bool unloadAllLoadedObjects = false)
		{
			string error;
			VP_LoadedAssetBundle bundle = GetLoadedAssetBundle(assetBundleName, out error);
			if (bundle == null)
				return;

			if (--bundle.m_ReferencedCount == 0)
			{
				bundle.m_AssetBundle.Unload(unloadAllLoadedObjects);
				m_loadedAssetBundles.Remove(assetBundleName);
				//Debug.Log("AssetBundle " + assetBundleName + " has been unloaded successfully");
			}
		}

		protected virtual void Update()
		{
			// Unloading process in progress, don't update anything... 
			// Collect all the finished WWWs.
			var keysToRemove = new List<string>();
			foreach (var keyValue in m_downloadingWWWs)
			{
				UnityWebRequest download = keyValue.Value;

				// If downloading fails.
				if (download.error != null)
				{
					Debug.Log(keyValue.Key + " " + download.error);

					// Security check to avoid duplicate keys in dictionary
					if (!m_downloadingErrors.ContainsKey(keyValue.Key))
						m_downloadingErrors.Add(keyValue.Key, download.error);

					keysToRemove.Add(keyValue.Key);
					continue;
				}

				// If downloading succeeds.
				if (download.isDone)
				{
					if (m_debug)
						Debug.Log("Downloading " + keyValue.Key + " is done at frame " + Time.frameCount);

					m_loadedAssetBundles.Add(keyValue.Key, new VP_LoadedAssetBundle(DownloadHandlerAssetBundle.GetContent(download)));
					keysToRemove.Add(keyValue.Key);

				}
				// Only show progress if we are loading content and every X time
				else if (m_showProgress)
				{
					// Shows download process
					StartCoroutine(WaitLoadingProcess(keyValue.Key, download.downloadProgress));
				}
			}

			// Remove the finished WWWs.
			foreach (var key in keysToRemove)
			{
				UnityWebRequest download = m_downloadingWWWs[key];
				m_downloadingWWWs.Remove(key);
				download.Dispose();
			}

			// Update all in progress operations
			for (int i = 0; i < m_inProgressOperations.Count;)
			{
				if (!m_inProgressOperations[i].Update())
				{
					m_inProgressOperations.RemoveAt(i);
				}
				else
					i++;
			}
		}

		protected virtual IEnumerator WaitLoadingProcess(string assetBundleName, float progress)
		{
			//Debug.Log("Showing progress...");

			m_showProgress = false;

			// @TODO: make real translation JSON entries
			string resultBundleName = null;
			switch (assetBundleName)
			{
				case "bullying":
					resultBundleName = "Allyson's Decision";
					break;
				case "bullying.unity3d":
					resultBundleName = "Allyson's Decision.unity3d";
					break;
				default:
					resultBundleName = assetBundleName;
					break;
			}

			bool m_isInstalled = IsInstalled(assetBundleName);

			if (m_isInstalled)
				m_downloadingProcess = "Loading " + resultBundleName + " " + (progress * 100).ToString("N0") + " %";
			else
				m_downloadingProcess = "Downloading " + resultBundleName + " " + (progress * 100).ToString("N0") + " %";

			// Wait X to not execute this function too may times
			yield return new WaitForSeconds(4);

			m_showProgress = true;
		}

		public virtual bool IsInstalled(string _bundle)
        {
			return false;
        }

		// Load asset from the given assetBundle.
		protected virtual VP_AssetBundleLoadAssetOperation LoadAssetAsync(string assetBundleName, string assetName, System.Type type)
		{
			VP_AssetBundleLoadAssetOperation operation = null;
#if UNITY_EDITOR
			if (SimulateAssetBundleInEditor)
			{
				string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
				if (assetPaths.Length == 0)
				{
					Debug.Log("There is no asset with name \"" + assetName + "\" in " + assetBundleName);
					return null;
				}

				// @TODO: Now we only get the main object from the first asset. Should consider type also.
				Object target = AssetDatabase.LoadMainAssetAtPath(assetPaths[0]);
				operation = new VP_AssetBundleLoadAssetOperationSimulation(target);
			}
			else
#endif
			{
				LoadAssetBundle(assetBundleName);
				operation = new VP_AssetBundleLoadAssetOperationFull(assetBundleName, assetName, type);

				m_inProgressOperations.Add(operation);
			}

			return operation;
		}

		// Load level from the given assetBundle.
		protected virtual VP_AssetBundleLoadOperation LoadLevelAsync(string assetBundleName, string levelName, bool isAdditive)
		{
			VP_AssetBundleLoadOperation operation = null;
#if UNITY_EDITOR
			if (SimulateAssetBundleInEditor)
			{
				string[] levelPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, levelName);

				foreach (string lvl in levelPaths)
				{
					if (m_debug)
						Debug.Log(">> " + lvl);
				}

				if (levelPaths.Length == 0)
				{
					///@TODO: The error needs to differentiate that an asset bundle name doesn't exist
					//        from that there right scene does not exist in the asset bundle...

					Debug.Log("There is no scene with name \"" + levelName + "\" in " + assetBundleName);
					return null;
				}

                


                if (isAdditive)
					UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(levelPaths[0], new UnityEngine.SceneManagement.LoadSceneParameters() { loadSceneMode = UnityEngine.SceneManagement.LoadSceneMode.Additive });
				else
					UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(levelPaths[0], new UnityEngine.SceneManagement.LoadSceneParameters() { loadSceneMode = UnityEngine.SceneManagement.LoadSceneMode.Single });

				operation = new VP_AssetBundleLoadLevelSimulationOperation();
			}
			else
#endif
			{
				if (m_debug)
				{
					Debug.Log("Asset name >> " + assetBundleName);
				}
				LoadAssetBundle(assetBundleName);
				operation = new VP_AssetBundleLoadLevelOperation(assetBundleName, levelName, isAdditive);

				m_inProgressOperations.Add(operation);
			}

			return operation;
		}

		protected virtual IEnumerator UnloadAllAssetBundles()
		{
			// Unloading starts...
			m_unloadingInProcess = true;

#if UNITY_EDITOR
			// If we're in Editor simulation mode, we don't have to load the manifest assetBundle.
			if (SimulateAssetBundleInEditor)
				yield return null;
#endif

			List<string> keys = new List<string>(m_loadedAssetBundles.Keys);

			for (int i = 0; i < keys.Count; i++)
			{
				string error;
				VP_LoadedAssetBundle bundle = GetLoadedAssetBundle(keys[i], out error, true);

				if (bundle == null)
					continue;

				if (bundle.m_AssetBundle == null)
					continue;

				//bundle.m_AssetBundle.Unload(false);
				bundle.m_AssetBundle.Unload(true);
				m_loadedAssetBundles.Remove(keys[i]);

				UnloadDependencies(keys[i], true);

				if (m_debug)
					Debug.Log("AssetBundle " + keys[i] + " has been unloaded successfully");
			}

			Debug.Log(m_loadedAssetBundles.Count + " assetbundle(s) in memory after unloading all assetbundles");

			//keysToRemove.Clear();

			m_downloadingWWWs.Clear();
			m_downloadingErrors.Clear();
			m_inProgressOperations.Clear();
			m_dependencies.Clear();
			m_loadedAssetBundles.Clear();
			m_assetsToDisk.Clear();

			Debug.Log("assetbundle unload end");

			m_unloadingInProcess = false;

			// Commented for performance boost! 
			// Clean cache as all bundles are store in disk
//#if !UNITY_IOS
//				Caching.CleanCache();
//#endif


			yield return null;
		}

	}
}
#endif