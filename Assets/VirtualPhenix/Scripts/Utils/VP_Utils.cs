using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using VirtualPhenix.Dialog;
using VirtualPhenix.Fade;
using System.Net;

#if USE_MORE_EFFECTIVE_COROUTINES
using MEC;
#endif

namespace VirtualPhenix
{
    public static partial class VP_Utils
	{
   
        public static void OpenFolder(string path)
        {
            if (System.IO.Directory.Exists(path))
            {
                System.Diagnostics.Process.Start(Path.GetFullPath(path));
            }
            else
            {
                System.IO.Directory.CreateDirectory(path);
                System.Diagnostics.Process.Start(Path.GetFullPath(path));
            }
        }

        public static bool FileExists(string filePath)
        {
            return System.IO.File.Exists(filePath);
        }

        public static void ShuffleItems<T>(IList<T> arr)
        {
            List<T> aux = new List<T>(arr);

            for (int i = 0; i < arr.Count; i++)
            {
                var indexRandom = UnityEngine.Random.Range(0, aux.Count);
                arr[i] = aux[indexRandom];
                aux.RemoveAt(indexRandom);
            }
        }

        public static void ShuffleArray<T>(T[] array)
        {
            for (int i = array.Length; i > 1; i--)
            {
                // Pick random element to swap.
                int indexRandom = UnityEngine.Random.Range(0, array.Length); // 0 <= j <= i-1
                                                                 // Swap.
                T tmp = array[indexRandom];
                array[indexRandom] = array[i - 1];
                array[i - 1] = tmp;
            }
        }

        public static string ChooseFilePathInsideAssets(string _title, string _defaultPath = "Assets/", string _extension = ".asset")
        {
            string whereToSaveAll = "";
#if UNITY_EDITOR
            whereToSaveAll = UnityEditor.EditorUtility.OpenFilePanel(_title, _defaultPath, _extension);

#else
#if USE_STANDALONE_FILEBROWSER
            //SFB.StandaloneFileBrowser.Choose
#endif
#endif

            return GetProjectAssetsFolderFromPath(whereToSaveAll);
        }


        public static string ChooseFilePath(string _title, string _defaultPath = "Assets/", string _extension = ".asset")
        {
            string whereToSaveAll = "";
#if UNITY_EDITOR
            whereToSaveAll = UnityEditor.EditorUtility.OpenFilePanel(_title, _defaultPath, _extension);

#else
#if USE_STANDALONE_FILEBROWSER
            //SFB.StandaloneFileBrowser.Choose
#endif
#endif

            return whereToSaveAll;
        }

        public static string ChooseFolderPath(string _title, string _defaultPath = "Assets/", string _folder = "")
        {
            string whereToSaveAll = "";
#if UNITY_EDITOR
            whereToSaveAll = UnityEditor.EditorUtility.OpenFolderPanel(_title, _defaultPath, _folder);

#else
#if USE_STANDALONE_FILEBROWSER
            //SFB.StandaloneFileBrowser.Choose
#endif
#endif

            return whereToSaveAll;
        }


        public static string ChooseSaveFolder(string _title, string _defaultPath = "Assets/", string _folder="")
        {
            string whereToSaveAll = "";
#if UNITY_EDITOR
            whereToSaveAll = UnityEditor.EditorUtility.SaveFolderPanel(_title, _defaultPath, _folder);

#else
#if USE_STANDALONE_FILEBROWSER
            //SFB.StandaloneFileBrowser.Choose
#endif
#endif
            
            return whereToSaveAll;
        }

        public static string ChooseSaveFile(string _title, string _defaultPath = "Assets/", string _defaultName ="New File", string _extension = "asset")
        {
            string whereToSaveAll = "";
#if UNITY_EDITOR
            whereToSaveAll = UnityEditor.EditorUtility.SaveFilePanel(_title, _defaultPath, _defaultName, _extension);

#else
#if USE_STANDALONE_FILEBROWSER
            //SFB.StandaloneFileBrowser.Choose
#endif
#endif

            return whereToSaveAll;
        }

#if UNITY_EDITOR

        public static void PingObject(UnityEngine.Object obj)
        {
            UnityEditor.EditorGUIUtility.PingObject(obj);
        }
#endif

        public static string GetProjectAssetsFolderToSaveFile(string _title = "Choose the folder", string _defaultFolder = "Assets/", string extension = ".asset")
        {
            string path = ChooseSaveFile(_title, _defaultFolder, extension);
            return GetProjectAssetsFolderFromPath(path);
        }

        public static string GetProjectAssetsFolderToSave(string _title = "Choose the folder", string _defaultFolder = "Assets/", string filter = "")
        {
            string path = ChooseSaveFolder(_title, _defaultFolder, filter);
            return GetProjectAssetsFolderFromPath(path);
        }

        public static string GetProjectAssetsFolderFromPath(string path)
        {
            string assetsStr = "Assets";
            int index = GetWordIndexInString(path, assetsStr) - assetsStr.Length;
            return path.Substring(index, path.Length - index);
        }

		public static bool StringStartsWithCharacter(string input, char _character)
		{
			return input.StartsWithCharacter(_character);
		}
		
		public static Color ColorFromHexString(string _hex)
		{
			Color c = Color.white;
			ColorUtility.TryParseHtmlString(_hex, out c);
			return c;
		}
		
		public static void ExitApplication()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.ExitPlaymode();
#else
			Application.Quit();
#endif
		}
		
#if UNITY_EDITOR
		public static void ExitPlayMode()
		{
			UnityEditor.EditorApplication.ExitPlaymode();
		}

		public static List<T> GetAllObjectsOfTypeInProject<T>(string T_AS_STR) where T : UnityEngine.Object
		{
			string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:{T_AS_STR}");
			List<T> finalList = new List<T>();  
			foreach (string ttype in guids)
			{
				string path = UnityEditor.AssetDatabase.GUIDToAssetPath(ttype);
				T val = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
				if (val != null)
				{
					finalList.Add(val);
				}
			}
			
			return finalList;
		}
		
		public static T GetObjectOfTypeInProject<T>(string T_AS_STR, Func<T, bool> _condition) where T : UnityEngine.Object
		{
			string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:{T_AS_STR}");
			foreach (string ttype in guids)
			{
				string path = UnityEditor.AssetDatabase.GUIDToAssetPath(ttype);
				T val = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
				if (val != null && (_condition == null ||_condition.Invoke(val)))
				{
					return val;
				}
			}
			
			return default(T);
		}

		public static T GetObjectOfTypeInProject<T>(string T_AS_STR, Func<T, string, bool> _condition) where T : UnityEngine.Object
		{
			string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:{T_AS_STR}");
			foreach (string ttype in guids)
			{
				string path = UnityEditor.AssetDatabase.GUIDToAssetPath(ttype);
				T val = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
				if (val != null && (_condition == null || _condition.Invoke(val, UnityEditor.AssetDatabase.GetAssetPath(val))))
				{
					return val;
				}
			}
			
			return default(T);
		}
		
		public static void GetObjectOfTypeInProject<T>(out T _data, Func<T, bool> _condition) where T : UnityEngine.Object
		{
			_data = default(T);
			string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:{_data.GetType().ToString()}");
			foreach (string ttype in guids)
			{
				string path = UnityEditor.AssetDatabase.GUIDToAssetPath(ttype);
				T val = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
				if (val != null && (_condition == null ||_condition.Invoke(val)))
				{
					_data = val;
					return;
				}
			}
		}
        
        public static void GetObjectOfTypeInProject<T>(out T _data, string _type, Func<T, bool> _condition) where T : UnityEngine.Object
		{
			_data = default(T);
			string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:{_type}");
			foreach (string ttype in guids)
			{
				string path = UnityEditor.AssetDatabase.GUIDToAssetPath(ttype);
				T val = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
				if (val != null && (_condition == null ||_condition.Invoke(val)))
				{
					_data = val;
					return;
				}
			}
		}
#endif
		
		
		public static string GetSaveLocation(SaveLocation m_location, Func<string> _customLocation = null, string _playerPrefKey = "")
		{
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
			if (m_location == SaveLocation.Console)
				m_location = SaveLocation.PersistentDatapath;
#elif UNITY_ANDROID || UNITY_IOS
			if (m_location == SaveLocation.Documents || m_location == SaveLocation.Console)
			m_location = SaveLocation.PersistentDatapath;
			
#if UNITY_IOS
			if (m_location == SaveLocation.DataPath)
			m_location = SaveLocation.PersistentDatapath;
#endif

#elif UNITY_PS4 || UNITY_SWITCH || UNITY_XBOXONE
			if (m_location != SaveLocation.Console)
			m_location = SaveLocation.Console;

#endif
			
			switch (m_location)
			{
			case SaveLocation.DataPath:
				string dp = Application.dataPath;
				if (System.IO.Directory.Exists(dp))
					return dp;

				VP_Debug.Log("Trying Persistent DataPath");

				return Application.persistentDataPath;
			case SaveLocation.PersistentDatapath:
				return Application.persistentDataPath;
			case SaveLocation.Documents:
				return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
			case SaveLocation.Console:
#if UNITY_EDITOR
				return Application.persistentDataPath;
#else
				var sm = Save.VP_SaveManagerBase.Instance;
				return sm != null ? sm.MountName : "MountSave";
#endif
			case SaveLocation.PlayerPrefs:
				return _playerPrefKey;
			case SaveLocation.Custom:
				return _customLocation.Invoke();
			}

			return Application.persistentDataPath;
		}

#if UNITY_EDITOR
        public static bool CheckPathOfDefineSymbol(VP_DefineSymbolsReferencer _symbol, string _path)
        {
            if (_symbol == null || _path.IsNullOrEmpty())
                return false;

            return _path == UnityEditor.AssetDatabase.GetAssetPath(_symbol);
        }
#endif
        public static object GetInstanceOfClassByStringName(string assembly, string className)
		{
			var typename = $"{assembly}.{className}";
			var objectType = System.Type.GetType(typename);
			if (objectType != null)
			{
				var result = Activator.CreateInstance(objectType);			
				object m = result as object;
				
				if (m != null)
				{				
					Debug.Log("Creating " + result.GetType().AssemblyQualifiedName + " when class name is" + typename);
					return m;
				}
			}
			
			Debug.LogError("Can't create class for Type " + className);
			return null;
		}
		
		public static T GetInstanceOfClassByStringName<T>(string assembly, string className)
		{
			var typename = $"{assembly}.{className}";
			var objectType = System.Type.GetType(typename);
			if (objectType != null)
			{
				var result = Activator.CreateInstance(objectType);			
				T m = (T)result;
				
				if (m != null)
				{				
					Debug.Log("Creating " + result.GetType().AssemblyQualifiedName + " when class name is" + typename);
					return m;
				}
			}
			
			Debug.LogError("Can't create class for Type " + className);
			return default(T);
		}
		
		public static bool GetGameObjectInParentWithName(string name, GameObject _current, out GameObject returnedGO)
		{
			if (_current == null || name.IsNullOrEmpty())
			{
				returnedGO = null;
				return false;
			}
			
			if (_current.name == name)
			{
				returnedGO = _current;
				return true;
			}
			
			if (_current.transform.parent == null)
			{
				returnedGO = null;
				return false;
			}
			
			GameObject parent = _current.transform.parent.gameObject;
			bool found = GetGameObjectInParentWithName(name, parent, out returnedGO);
			if (found)
			{
				return true;
			}
		
			returnedGO = null;
			return false;
		}
		
		public static bool GetGameObjectInChildrenWithName(string name, GameObject _current, out GameObject returnedGO)
		{
			if (_current == null || name.IsNullOrEmpty())
			{
				returnedGO = null;
				return false;
			}
			
			if (_current.name == name)
			{
				returnedGO = _current;
				return true;
			}
			
			if (_current.transform.childCount == 0)
			{
				returnedGO = null;
				return false;
			}
			
			for (int i = 0; i < _current.transform.childCount; i++)
			{
				GameObject childObj = _current.transform.GetChild(i).gameObject;
				bool found = GetGameObjectInChildrenWithName(name, childObj, out returnedGO);
				if (found)
				{
					return true;
				}
			}
			
			returnedGO = null;
			return false;
		}
		
        public static string GetResourcePathFromAssets(string _original)
        {
            string assetsStr = "Assets";
            int index = GetWordIndexInString(_original, assetsStr) - assetsStr.Length;
            return _original.Substring(index, _original.Length - index);
        }


        public static void StartTimer(float _time, bool _up, UnityEngine.Events.UnityAction _callback = null, VP_Timer _timer = null,  bool _resetIfRunning = true)
        {
            if (_timer == null)
                _timer = GameObject.FindObjectOfType<VP_Timer>();

            if (_timer)
            {
                _timer.StartTimer(_time, _up, _resetIfRunning, _callback);
            }
        }

        public static void StopTimer(VP_Timer _timer = null)
        {
            if (_timer == null)
                _timer = GameObject.FindObjectOfType<VP_Timer>();

            if (_timer)
            {
                _timer.StopTimer();
            }
        }

        public static void ShakeCamera(float intensity, float _time, float frequencyGrain = 2f, VP_CinemachineCameraShake _cameraShake = null, UnityEngine.Events.UnityAction _callback = null)
        {
            if (_cameraShake == null)
                _cameraShake = GameObject.FindObjectOfType<VP_CinemachineCameraShake>();

            if (_cameraShake)
            {
                _cameraShake.Shake(intensity, _time, frequencyGrain, _callback);
            }
        }

        public static bool IsObjectEqualsTo<T>(T _object1, T _object2)
        {
            return EqualityComparer<T>.Default.Equals(_object1, _object2);
        }

#if USE_MORE_EFFECTIVE_COROUTINES
        public static void RunWaitTimeCoroutineWithCallback(float _time, UnityEngine.Events.UnityAction _callback, GameObject _endWithGO = null, string _coroutineTag = "", MEC.Segment _segment = MEC.Segment.Update, bool _killPrevious = false)
        {
            if (_killPrevious && _coroutineTag.IsNotNullNorEmpty())
            {
                Timing.KillCoroutines(_coroutineTag);
            }

            MEC.Timing.RunCoroutine(WaitTime(_time, _callback).CancelWith(_endWithGO), _segment, _coroutineTag);
        }


        public static void RunWaitTimeCoroutineWithFunction(float _time, Func<bool> _exitCallback, UnityEngine.Events.UnityAction _callback = null, GameObject _endWithGO = null, string _coroutineTag = "", MEC.Segment _segment = MEC.Segment.Update, bool _killPrevious = false)
		{
			if (_killPrevious && _coroutineTag.IsNotNullNorEmpty())
			{
				Timing.KillCoroutines(_coroutineTag);
			}
			
			MEC.Timing.RunCoroutine(WaitTime(_time, _callback, _exitCallback).CancelWith(_endWithGO), _segment, _coroutineTag);
		}

		public static void RunMECCoroutine(IEnumerator<float>_coroutine, UnityEngine.Events.UnityAction _callback = null, GameObject _endWithGO = null, string _coroutineTag = "", MEC.Segment _segment = MEC.Segment.Update, bool _killPrevious = false)
		{
			if (_killPrevious && _coroutineTag.IsNotNullNorEmpty())
			{
				Timing.KillCoroutines(_coroutineTag);
			}
			
			MEC.Timing.RunCoroutine(_coroutine.CancelWith(_endWithGO), _segment, _coroutineTag);
		}

		public static void RunWaitTimeCoroutine(float _time, UnityEngine.Events.UnityAction _callback = null, GameObject _endWithGO = null, string _coroutineTag = "", MEC.Segment _segment = MEC.Segment.Update, bool _killPrevious = false)
		{
			if (_killPrevious && _coroutineTag.IsNotNullNorEmpty())
			{
				Timing.KillCoroutines(_coroutineTag);
			}
			
			MEC.Timing.RunCoroutine(WaitTime(_time, _callback).CancelWith(_endWithGO), _segment, _coroutineTag);
		}

		public static IEnumerator<float> WaitTime(float _time, UnityEngine.Events.UnityAction _callback, Func<bool> _exitCallback)
		{
			float _timer = 0f;
			while (_timer < _time && !_exitCallback.Invoke())
			{
				_timer += Time.deltaTime;
				yield return Timing.WaitForOneFrame;
			}
	
			if (_callback != null)
				_callback.Invoke();
		}
		
		 public static IEnumerator<float> WaitTime(float _time, UnityEngine.Events.UnityAction _callback)
		{
			float _timer = 0f;
			while (_timer < _time)
			{
				_timer += Time.deltaTime;
				yield return Timing.WaitForOneFrame;
			}
	
			if (_callback != null)
				_callback.Invoke();
		}
#else
        public static void RunWaitTimeCoroutine(float _time, UnityEngine.Events.UnityAction _callback = null, bool _killPrevious = false, GameObject _endWithGO = null)
        {
            // Check if any VP MonoBehavior one can handle that coroutine
            VP_MonoBehaviour mono = GameObject.FindObjectOfType<VP_MonoBehaviour>();

            if (mono != null)
            {
                mono.WaitTime(_time, _callback, _killPrevious);
            }
        }

        public static void RunWaitTimeCoroutine(float _time, UnityEngine.Events.UnityAction _callback = null, GameObject _gameObject = null, bool _killPrevious = false)
        {
            VP_MonoBehaviour mono = null;
            mono = (_gameObject == null) ? GameObject.FindObjectOfType<VP_MonoBehaviour>() : _gameObject.GetComponent<VP_MonoBehaviour>();

            if (mono != null)
            {
                mono.WaitTime(_time, _callback, _killPrevious);
            }
        }
#endif

#if UNITY_EDITOR
        public static T CreateOrReplaceAsset<T>(T asset, string path) where T : UnityEngine.Object
		{
			T existingAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);

			if (existingAsset == null)
			{
				UnityEditor.AssetDatabase.CreateAsset(asset, path);
				existingAsset = asset;
			}
			else
			{
				UnityEditor.EditorUtility.CopySerialized(asset, existingAsset);
			}

			return existingAsset;
		}
#endif
    	
        public static bool CheckIfInputButtonExists(string _btnName)
        {
            try
            {
                Input.GetButton(_btnName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool CheckIfInputAxisExists(string _axisName)
        {
            try
            {
                Input.GetAxis(_axisName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool HasInternetConnection()
        {
            string htmlText = GetHtmlFromUri("http://google.com");
            if (string.IsNullOrEmpty(htmlText))
            {
                //No connection
                return false;
            }
            else if (!htmlText.Contains("schema.org/WebPage"))
            {
                //Redirecting since the beginning of googles html contains that 
                //phrase and it was not found
                return false;
            }
            else
            {
                //success
                return true;
            }
        }

        public static string GetHtmlFromUri(string resource)
        {
            string html = string.Empty;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(resource);
            try
            {
                using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
                {
                    bool isSuccess = (int)resp.StatusCode < 299 && (int)resp.StatusCode >= 200;
                    if (isSuccess)
                    {
                        using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                        {
                            //We are limiting the array to 80 so we don't have
                            //to parse the entire html document feel free to 
                            //adjust (probably stay under 300)
                            char[] cs = new char[80];
                            reader.Read(cs, 0, cs.Length);
                            foreach (char ch in cs)
                            {
                                html += ch;
                            }
                        }
                    }
                }
            }
            catch
            {
                return "";
            }
            return html;
        }


        public static string AddZerosToCharacterNumber(int number, int maxCharacter = 3)
        {
            return number.ToString().PadLeft(maxCharacter, '0');
        }
        /// <summary>
        /// Method that saves a png based on png and render to texture
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="path"></param>
        public static void SavePNGFromTexture(Texture2D texture, string path)
        {
            byte[] bytes;
            bytes = texture.EncodeToPNG();

            System.IO.File.WriteAllBytes(path, bytes);
        }

        public static Texture2D TakeScreenshot(Camera renderingCamera, int sqrX, int sqrY, string path)
        {
            // capture the virtuCam and save it as a square PNG.
            int width = Screen.width;
            int height = Screen.height;

            // recall that the height is now the "actual" size from now on

            RenderTexture tempRT = new RenderTexture(width, height, 24);
            // the 24 can be 0,16,24, formats like
            // RenderTextureFormat.Default, ARGB32 etc.
            renderingCamera.targetTexture = tempRT;
            renderingCamera.Render();

            RenderTexture.active = tempRT;
            Texture2D virtualPhoto = new Texture2D(sqrX, sqrY, TextureFormat.RGB24, false);
            virtualPhoto.ReadPixels(new Rect(width / 2 - sqrX / 2, height / 2 - sqrY / 2, sqrX, sqrY), 0, 0);
            virtualPhoto.Apply();

            RenderTexture.active = null; //can help avoid errors 
            renderingCamera.targetTexture = null;

            UnityEngine.Object.Destroy(tempRT);

            SavePNGFromTexture(virtualPhoto, path);

            return virtualPhoto;
        }

        // extracts the name of a scene from the path pointing to it
        public static string ExtractSceneNameFromPath(string path)
        {
            int i = path.Length - 1;
            string currentChar = "";

            // browse through characters
            while (i > 0 && currentChar != "/" && currentChar != "\\")
            {
                i--;
                currentChar = path[i].ToString();
            };

            int start = i + 1, length = path.Length - i - 7;
            if (length < 0) { length = 0; Debug.LogWarning("Failed to extract scene name from scene path."); }
            return path.Substring(start, length);
        }

       
        public static string CreateID()
        {
            StringBuilder builder = new StringBuilder();
            Enumerable
               .Range(65, 26)
                .Select(e => ((char)e).ToString())
                .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                .OrderBy(e => Guid.NewGuid())
                .Take(11)
                .ToList().ForEach(e => builder.Append(e));
            string id = builder.ToString();
            //Debug.Log("Creating ID: " + id);
            return id;
        }

        public static string GetStringBetweenStrings(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = GetWordIndexInString(strSource, strStart);
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                UnityEngine.Debug.Log("words not contained in source string");
                return "";
            }
        }

        public static int GetWordIndexInString(string strSource, string word)
        {
            int ret = -1;
            if (strSource.Contains(word))
            {
                ret = strSource.IndexOf(word, 0) + word.Length;
            }
            return ret;
        }

        public static string SplitCamelCase(string str)
        {
            return Regex.Replace(Regex.Replace(str, "(\\P{Ll})(\\P{Ll}\\p{Ll})", "$1 $2"), "(\\p{Ll})(\\P{Ll})", "$1 $2");
        }

        public static T GetFieldValue<T>(object obj, string fieldName)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.Public |
                                                          System.Reflection.BindingFlags.NonPublic |
                                                          System.Reflection.BindingFlags.Instance);

            if (field == null)
                throw new ArgumentException(fieldName, "No such field was found.");

            if (!typeof(T).IsAssignableFrom(field.FieldType))
                throw new InvalidOperationException("Field type and requested type are not compatible.");

            return (T)field.GetValue(obj);
        }

        //-------------------------------------------------------------------------
        public static string GetStringBetweenChars(string fullStr, char element, char element2)
        {
            int str1 = fullStr.IndexOf(element);
            int str2 = fullStr.IndexOf(element2);

            return "";
        }

        public static int GetIndexFromString(string fullStr, char element)
        {
            return fullStr.IndexOf(element);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        static public string[] SplitStringByCommas(string line)
        {
            return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
            @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)",
            System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
                    select m.Groups[1].Value).ToArray();
        }

        public static bool IsStringVersionHigher(string newVersion, string versionToCompare, int _numToCompare)
        {
            string[] version1Strs = newVersion.Split('.');
            string[] version2Strs = versionToCompare.Split('.');

            if (_numToCompare > version1Strs.Length)
            {
                _numToCompare = version1Strs.Length;
            }

            if (_numToCompare > version2Strs.Length)
            {
                _numToCompare = version2Strs.Length;
            }

            for (int i = 0; i < _numToCompare; i++)
            {
                int value = 0;
                int.TryParse(version1Strs[i], out value);
                int value2 = 0;
                int.TryParse(version2Strs[i], out value2);
                if (value > value2)
                {
                    return true;
                }
                else if (value < value2)
                {
                    return false;
                }
            }

            return false;
        }

    }
}
