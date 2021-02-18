using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Localization;
using System.Linq;

namespace VirtualPhenix
{
	public static partial class VP_Extensions
	{
#if UNITY_EDITOR
		public static bool CreateInstanceAndSaveIt<T>(this VP_ScriptableObject input, out T _so, string _name = "new SO") where T : VP_ScriptableObject
		{
			string whereToSaveAll = VP_Utils.GetProjectAssetsFolderToSave("Choose the folder where to save the Scriptable Object");

			if (whereToSaveAll.IsNullOrEmpty())
			{
				_so = default(T);
				return false;
			}

			string assetPathAndName = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(whereToSaveAll + "/" + _name + ".asset");

			_so = ScriptableObject.CreateInstance<T>();
			UnityEditor.AssetDatabase.CreateAsset(_so, assetPathAndName);

			UnityEditor.AssetDatabase.SaveAssets();
			UnityEditor.AssetDatabase.Refresh();
			UnityEditor.EditorUtility.FocusProjectWindow();

			return true;
		}
#endif

		public static string CheckStringStartsWithCharacter(this string input, char _character)
		{
			return (input.StartsWithCharacter(_character)) ? input : _character.ToString() + input;
		}
		
		public static bool StartsWithCharacter(this string input, char _character)
		{
			return input.IsNotNullNorEmpty() && input[0].Equals(_character);
		}
		
		public static string Append(this string input, string _stringToAdd)
		{
			return input + _stringToAdd;
		}

		public static string FirstCharToUpper(this string input)
		{
			switch (input)
			{
				case null: throw new System.ArgumentNullException(nameof(input));
				case "": throw new System.ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
				default: return input.First().ToString().ToUpper() + input.Substring(1);
			}
		}

		public static string FirtsCharToUpperRestLow(this string input)
        {
			return input.ToLower().FirstCharToUpper();
		}

		public static string GetUnicodeString(this string str)
		{
			byte[] utf8Bytes = System.Text.Encoding.Unicode.GetBytes(str);
			return System.Text.Encoding.Unicode.GetString(utf8Bytes);
		}
		
		public static string GetUTF8String(this string str)
		{
			byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(str);
			return System.Text.Encoding.UTF8.GetString(utf8Bytes);
		}
		
		public static string StripUnicodeCharacters(this string str)
		{
			return System.Text.RegularExpressions.Regex.Replace(str, @"[^\u0020-\u007E]", string.Empty);
		}
		public static string ReplaceUnicodeCharacters(this string str)
		{
			return System.Text.RegularExpressions.Regex.Unescape($@"{str}");
		}
		
		public static string ConvertToASCII(this string str)
		{
			string asAscii = System.Text.Encoding.ASCII.GetString(
    			System.Text.Encoding.Convert(
					System.Text.Encoding.UTF8,
					System.Text.Encoding.GetEncoding(
						System.Text.Encoding.ASCII.EncodingName,
						new System.Text.EncoderReplacementFallback(string.Empty),
						new System.Text.DecoderExceptionFallback()
					),
					System.Text.Encoding.UTF8.GetBytes(str)
    			)
			);
			
			return asAscii;
		}
		
	#if USE_ANIMANCER	
		public static Animancer.ClipState.Transition AnimancerTransitionDeepCopy(this Animancer.ClipState.Transition original, out Animancer.ClipState.Transition copy)
		{
			copy = new Animancer.ClipState.Transition();
			
			/*
			copy.Clip = original.Clip;
			copy.FadeMode = original.FadeMode; // Read Only
			copy.FadeDuration = original.FadeDuration;
			copy.Speed = original.Speed;
			copy.Events = original.Events; // Read Only
			copy.SerializedEvents = original.SerializedEvents;
			copy.AverageAngularSpeed = original.AverageAngularSpeed; // Read Only
			copy.AverageVelocity = original.AverageVelocity;
			copy.BaseState = original.BaseState;
			copy.IsLooping = original.IsLooping;
			copy.IsValid = original.IsValid;
			copy.Key = original.Key;
			copy.MainObject = original.MainObject;
			copy.MaximumDuration = original.MaximumDuration;
			copy.Name = original.Name;
			copy.NormalizedStartTime = original.NormalizedStartTime;
			copy.State = original.State;
			*/

			return copy;
		}
	#endif	
		// Deep clone -> A needs to be System.Serializable
		public static T DeepClone<T>(this T a)
		{
			using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
			{
				System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
				formatter.Serialize(stream, a);
				stream.Position = 0;
				return (T) formatter.Deserialize(stream);
			}
		}
		
		// Deep clone with ISerializable
		public static T SerializableDeepClone<T>(this T a) where T : System.Runtime.Serialization.ISerializable
		{
			using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
			{
				System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
				formatter.Serialize(stream, a);
				stream.Position = 0;
				return (T) formatter.Deserialize(stream);
			}
		}
		
		public static GameObject CloneAt(this GameObject obj, Vector3 position)
		{
			GameObject obj2 = GameObject.Instantiate<GameObject>(obj);
			obj2.transform.position = position;
			return obj2;
		}

		public static GameObject Clone(this GameObject obj)
		{
			GameObject obj2 = GameObject.Instantiate<GameObject>(obj);
			return obj2;
		}

		public static GameObject CloneInactive(this GameObject obj)
		{
			obj.gameObject.SetActive(false);
			GameObject obj2 = GameObject.Instantiate<GameObject>(obj);
			obj.gameObject.SetActive(true);
			return obj2;
		}


		public static IEnumerable<Transform> GetChildren(this Transform transform)
		{
			foreach (Transform child in transform)
			{
				yield return child;
			}
		}

		public static void DestroyChildren(this Transform transform)
		{
			foreach (Transform child in transform)
			{
				GameObject.Destroy(child.gameObject);
			}
		}

		public static string CamelCaseToSpaces(this string text)
		{
			return System.Text.RegularExpressions.Regex.Replace(text, "(\\B[A-Z])", " $1");
		}


		public static GameObject GetGameObject(this ResourceRequest request)
		{
			return request.asset as GameObject;
		}


		public static AudioSource PlayAndGetSource(this AudioClip clip, VP_AudioSetup.AUDIO_TYPE _type)
		{
			var am = VP_AudioManager.Instance;
			am.PlayAudioInSource(clip, _type);
			return am.GetAudioSourceForType(_type);
		}

		public static bool IncludesLayer(this LayerMask mask, int layer)
		{
			return mask == (mask | (1 << layer));
		}

		public static IEnumerator WrapAsEnumerator(this YieldInstruction yieldInstruction)
		{
			yield return yieldInstruction;
		}

		public static IEnumerable<T> FindComponentsOfTypeInScene<T>(this UnityEngine.SceneManagement.Scene scene) where T : Component
		{
			foreach (GameObject rootObj in scene.GetRootGameObjects())
			{
				foreach (T foundObj in rootObj.GetComponentsInChildren<T>())
				{
					yield return foundObj;
				}
			}
		}

		public static IEnumerable<Component> FindComponentsOfTypeInScene(this UnityEngine.SceneManagement.Scene scene, System.Type type)
		{
			foreach (GameObject rootObj in scene.GetRootGameObjects())
			{
				foreach (Component foundObj in rootObj.GetComponentsInChildren(type))
				{
					yield return foundObj;
				}
			}
		}

		public static T GetOrAddComponentInChildren<T>(this GameObject _gameObject) where T : Component
		{
			T _component = _gameObject.GetComponentInChildren<T>();
			return _component != null ? _component : _gameObject.AddComponent<T>();
		}
		
		public static T GetOrAddComponentInParent<T>(this GameObject _gameObject) where T : Component
		{
			T _component = _gameObject.GetComponentInParent<T>();
			return _component != null ? _component : _gameObject.AddComponent<T>();
		}
		
		public static T GetOrAddComponent<T>(this GameObject _gameObject) where T : Component
		{
			T _component = _gameObject.GetComponent<T>();
			return _component != null ? _component : _gameObject.AddComponent<T>();
		}
		
		public static T GetOrAddComponentInChildren<T>(this Transform _transform) where T : Component
		{
			T _component = _transform.GetComponentInChildren<T>();
			return _component != null ? _component : _transform.gameObject.AddComponent<T>();
		}
		
		public static T GetOrAddComponentInParent<T>(this Transform _transform) where T : Component
		{
			T _component = _transform.GetComponentInParent<T>();
			return _component != null ? _component : _transform.gameObject.AddComponent<T>();
		}
		
		public static T GetOrAddComponent<T>(this Transform _transform) where T : Component
		{
			T _component = _transform.GetComponent<T>();
			return _component != null ? _component : _transform.gameObject.AddComponent<T>();
		}
		
		//--
		public static T GetOrAddComponentInChildren<T>(this GameObject _transform, out T _component) where T : Component
		{
			_component = _transform.GetComponentInChildren<T>();
			return _component != null ? _component : _transform.AddComponent<T>();
		}
		
		public static T GetOrAddComponentInParent<T>(this GameObject _transform, out T _component) where T : Component
		{
			_component = _transform.GetComponentInParent<T>();
			return _component != null ? _component : _transform.AddComponent<T>();
		}
		
		public static T GetOrAddComponent<T>(this GameObject _transform, out T _component) where T : Component
		{
			_component = _transform.GetComponent<T>();
			return _component != null ? _component : _transform.AddComponent<T>();
		}
		
		public static T GetOrAddComponentInChildren<T>(this Transform _transform, out T _component) where T : Component
		{
			_component = _transform.GetComponentInChildren<T>();
			return _component != null ? _component : _transform.gameObject.AddComponent<T>();
		}
		
		public static T GetOrAddComponentInParent<T>(this Transform _transform, out T _component) where T : Component
		{
			_component = _transform.GetComponentInParent<T>();
			return _component != null ? _component : _transform.gameObject.AddComponent<T>();
		}
		
		public static T GetOrAddComponent<T>(this Transform _transform, out T _component) where T : Component
		{
			_component = _transform.GetComponent<T>();
			return _component != null ? _component : _transform.gameObject.AddComponent<T>();
		}
		
		public static bool TryGetComponentInChildren<T>(this VP_Monobehaviour mono, out T _component)
		{
			Transform transform = mono.transform;
			if (transform.childCount > 0)
			{
				if (transform.TryGetComponent(out _component))
				{
					return true;
				}
				else
				{
					for (int i = 0; i < transform.childCount; i++)
					{
						bool childHas = transform.GetChild(i).TryGetComponentInChildren(out _component);
						if (childHas)
							return true;
					}

					_component = default(T);
					return false;
				}
			}
			else
			{
				_component = default(T);
				return false;
			}
		}

		public static bool TryGetComponentInParent<T>(this VP_Monobehaviour mono, out T _component)
		{
			Transform transform = mono.transform;
			if (transform.parent != null)
			{
				if (transform.TryGetComponent(out _component))
				{
					return true;
				}
				else
				{
					return transform.parent.TryGetComponentInParent(out _component);
				}
			}
			else
			{
				_component = default(T);
				return false;
			}
		}

		public static bool TryGetComponentInChildren<T>(this Transform transform, out T _component, System.Func<T, bool> _condition)
		{
			if (transform.childCount > 0)
			{
				if (transform.TryGetComponent(out _component))
				{
					if (_condition.Invoke(_component))
                    {
						return true;
					}
					else
                    {
						for (int i = 0; i < transform.childCount; i++)
						{
							bool childHas = transform.GetChild(i).TryGetComponentInChildren(out _component, _condition);
							if (childHas)
								return true;
						}

						_component = default(T);
						return false;
					}
				}
				else
				{
					for (int i = 0; i < transform.childCount; i++)
					{
						bool childHas = transform.GetChild(i).TryGetComponentInChildren(out _component, _condition);
						if (childHas)
							return true;
					}

					_component = default(T);
					return false;
				}
			}
			else
			{
				_component = default(T);
				return false;
			}
		}

		public static bool TryGetComponentInChildren<T>(this Transform transform, out T _component)
		{
			if (transform.childCount > 0)
			{
				if (transform.TryGetComponent(out _component))
				{
					return true;
				}
				else
				{
					for (int i = 0; i < transform.childCount; i++)
					{
						bool childHas = transform.GetChild(i).TryGetComponentInChildren(out _component);
						if (childHas)
							return true;
					}

					_component = default(T);
					return false;
				}
			}
			else
			{
				_component = default(T);
				return false;
			}
		}

		public static bool TryGetComponentInParent<T>(this Transform transform, out T _component)
		{
			T[] comps = transform.GetComponentsInParent<T>();

			if (comps.Length > 0)
            {
				_component = comps[0];
				return true;
            }
			else
            {
				_component = default(T);
				return false;
			}

			/*
			if (transform.parent != null)
			{
				if (transform.TryGetComponent(out _component))
				{
					return true;
				}
				else
				{
					return transform.parent.TryGetComponentInParent(out _component);
				}
			}
			else
			{
				_component = default(T);
				return false;
			}
			*/
		}

		public static bool TryGetComponentInChildren<T>(this Component thisComp, out T _component)
		{
			Transform transform = thisComp.transform;

			if (transform.childCount > 0)
			{
				if (transform.TryGetComponent(out _component))
				{
					return true;
				}
				else
				{
					for(int i = 0; i < transform.childCount; i++)
                    {
						bool childHas = transform.GetChild(i).TryGetComponentInChildren(out _component);
						if (childHas)
							return true;
					}

					_component = default(T);
					return false;
				}
			}
			else
			{
				_component = default(T);
				return false;
			}
		}

		public static bool TryGetComponentInParent<T>(this Component thisComp, out T _component)
		{
			Transform transform = thisComp.transform;

			if (transform.parent != null)
			{
				if (transform.TryGetComponent(out _component))
				{
					return true;
				}
				else
				{
					return transform.parent.TryGetComponentInParent(out _component);
				}
			}
			else
			{
				_component = default(T);
				return false;
			}
		}

		public static Dictionary<T0, T1> AsDictionary<T, T0, T1>(this T dictionary) where T : VP_SerializableDictionary<T0, T1>
		{
			return new Dictionary<T0, T1>(dictionary);
		}

		public static VP_SerializableDictionary<T0, T1> AsVPDictionary<T, T0, T1>(this T dictionary) where T : Dictionary<T0, T1>
		{
			return new VP_SerializableDictionary<T0, T1>(dictionary);
		}

		public static TR AsVPDictionary<T, T0, T1, TR>(this T dictionary) where T : IDictionary<T0, T1> where TR : VP_SerializableDictionary<T0, T1>
		{
			return (new VP_SerializableDictionary<T0, T1>(dictionary)) as TR;
		}

		public static System.Type[] GetKeyAndValueTypes(this IDictionary dictionary)
        {
			System.Type[] arguments = dictionary.GetType().GetGenericArguments();
			return new System.Type[] { arguments[0], arguments[1] };
		}

		/// <summary>
		/// Returns a random value within the Vector range [min,max], both inclusive
		/// </summary>
		/// <param name="vec"></param>
		/// <returns></returns>
		public static float RandomInRange(this Vector2 vec)
		{
			return Random.Range(vec.x,vec.y);
		}
		
		public static string AddZerosToTheLeft(this int number, int maxCharacter = 3)
		{
			return number.ToString().PadLeft(maxCharacter, '0');
		}
		
		public static float NormalizedValue(this float _currentValue, float _maxValue = 1f)
		{
			return Mathf.Clamp(_currentValue/_maxValue, 0f, 1f);
		}
		
		public static bool IsNotNullNorEmpty(this string _strToCheck)
		{
			return !string.IsNullOrEmpty(_strToCheck);
		}
		
		public static bool IsNullOrEmpty(this string _strToCheck)
		{
			return string.IsNullOrEmpty(_strToCheck);
		}
		
		public static string IsNullOrEmptyFallback(this string _strToCheck, string fallback)
		{
			if (string.IsNullOrEmpty(_strToCheck))
				_strToCheck = fallback;
				
			return _strToCheck;
		}
		
		public static Vector3 RandomPointInBounds(this Collider col)
		{
			Bounds bounds = col.bounds;
			
			return new Vector3(
				UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
				UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
				UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
			);
		}
		
		
		public static void SetRectValues(this RectTransform rt, Vector2 leftRight, Vector2 topBottom)
		{
			SetRectLeft(rt, leftRight.x);
			SetRectRight(rt, leftRight.y);
			SetRectTop(rt, topBottom.x);
			SetRectBottom(rt, topBottom.y);
		}
		public static void SetRectValues(this RectTransform rt, float left, float right, float top, float bottom)
		{
			SetRectLeft(rt, left);
			SetRectRight(rt, right);
			SetRectTop(rt, top);
			SetRectBottom(rt, bottom);
		}

		public static float MinHorizontalOffset(this RectTransform rt)
		{
			return rt.offsetMin.x;
		}

		public static float MaxHorizontalOffset(this RectTransform rt)
		{
			return rt.offsetMax.x * -1;
		}

		public static float MaxVerticalOffset(this RectTransform rt)
		{
			return rt.offsetMax.y * -1;
		}

		public static float RectVerticalMinffset(this RectTransform rt)
		{
			return rt.offsetMin.y;
		}

		public static Vector2 LeftRightOffset(this RectTransform rt)
		{
			return new Vector2(MinHorizontalOffset(rt), MaxHorizontalOffset(rt));
		}

		public static Vector2 TopBottomOffset(this RectTransform rt)
		{
			return new Vector2(rt.MaxVerticalOffset(), rt.RectVerticalMinffset());
		}

		public static void SetRectLeft(this RectTransform rt, float left)
		{
			rt.offsetMin = new Vector2(left, rt.offsetMin.y);
		}

		public static void SetRectRight(this RectTransform rt, float right)
		{
			rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
		}

		public static void SetRectTop(this RectTransform rt, float top)
		{
			rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
		}

		public static void SetRectBottom(this RectTransform rt, float bottom)
		{
			rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
		}
	}
}
