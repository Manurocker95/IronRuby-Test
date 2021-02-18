using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace VirtualPhenix
{
	public static partial class VP_Extensions
	{
		public static Vector3 Round(this Vector3 vector)
		{
			return new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
		}

		public static Vector3 Clamp(this Vector3 v, float length)
		{
			float m = v.magnitude;
			if (m > length)
			{
				return (v / m) * length;
			}
			return v;
		}

		public static Vector3 Snap(this Vector3 v, float cellSize)
		{
			v = (v / cellSize);
			return new Vector3((int)v.x, (int)v.y, (int)v.z) * cellSize;
		}

		public static Vector3 SetX(this Vector3 vector, float x)
		{
			return new Vector3(x, vector.y, vector.z);
		}

		public static Vector3 SetY(this Vector3 vector, float y)
		{
			return new Vector3(vector.x, y, vector.z);
		}

		public static Vector3 SetZ(this Vector3 vector, float z)
		{
			return new Vector3(vector.x, vector.y, z);
		}

		public static Vector2 SetX(this Vector2 vector, float x)
		{
			return new Vector2(x, vector.y);
		}

		public static Vector2 SetY(this Vector2 vector, float y)
		{
			return new Vector2(vector.x, y);
		}

		public static Vector3 TransformComponents(this Vector3 vector, System.Func<float, float> transformation)
		{
			return new Vector3(transformation(vector.x), transformation(vector.y), transformation(vector.z));
		}

		public static int Mod(this int x, int m)
		{
			return (x % m + m) % m;
		}

		public static float Sqr(this float v)
		{
			return v * v;
		}

		public static Color SetColorAlpha(this Color color, float a)
		{
			return new Color(color.r, color.g, color.b, a);
		}


		public static Vector3 WorldMousePosition(this Camera camera, float? distanceFromCamera = null)
		{
			if (distanceFromCamera == null)
			{
				distanceFromCamera = 0;
			}
			return camera.ScreenToWorldPoint(Input.mousePosition.SetZ(distanceFromCamera.Value));
		}

		public static Vector3 RandomWithin(this Bounds bounds)
		{
			Vector3 randomWithin = new Vector3(
				bounds.size.x * UnityEngine.Random.value,
				bounds.size.y * UnityEngine.Random.value,
				bounds.size.z * UnityEngine.Random.value);
			return bounds.min + randomWithin;
		}

		public static Rect SplitHorizontal(this Rect rect, int number, int index, float spacing = 5)
		{
			float width = (rect.width - (spacing * System.Math.Max(0, number - 1))) / number;
			return new Rect(rect.x + (width + spacing) * index, rect.y, width, rect.height);
		}

		public static Rect SplitHorizontal(this Rect rect, float splitPercent, bool first, float spacing = 5)
		{
			return new Rect(
				rect.x + (first ? 0 : (rect.width - spacing) * splitPercent + spacing),
				rect.y,
				(rect.width - spacing) * (first ? splitPercent : 1 - splitPercent),
				rect.height);
		}

		public static Rect SplitVertical(this Rect rect, int number, int index, float spacing = 5)
		{
			float height = (rect.height - (spacing * System.Math.Max(0, number - 1))) / number;
			return new Rect(rect.x, rect.y + (height + spacing) * index, rect.width, height);
		}


		public static Vector3 TransformPointTo(this RectTransform from, Vector3 point, RectTransform to)
		{
			return to.InverseTransformPoint(from.TransformPoint(point));
		}

		public static Vector2 WorldToRectTransform(Vector3 worldPosition, RectTransform toTransform)
		{
			Vector2 viewportPoint = Camera.main.WorldToViewportPoint(worldPosition);
			RectTransform canvasRect = toTransform.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
			Vector2 canvasPoint = Vector2.Scale(canvasRect.sizeDelta, (viewportPoint - Vector2.one * 0.5f));
			return TransformPointTo(canvasRect, canvasPoint, toTransform);
		}


		public static bool IsPointWithin(this Collider collider, Vector3 point)
		{
			if (!collider.bounds.Contains(point))
			{
				return false;
			}

			Vector3 towardCenter = collider.bounds.center - point;
			RaycastHit hit;
			return !collider.Raycast(new Ray(point, towardCenter), out hit, towardCenter.magnitude);
		}

		public static IEnumerable<T> Yield<T>(this T item)
		{
			yield return item;
		}

		public static string ColorToHexString(this Color32 color)
		{
			string hex = "#" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
			return hex;
		}

		public static Color HexStringToColor(string hex)
		{
			try
			{
				byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
				byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
				byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
				return new Color32(r, g, b, 255);
			}
			catch (System.FormatException)
			{
				return Color.black;
			}
		}
		public static void Swap<T>(this T[] source, int index1, int index2)
		{
			if (source != null && index1 >= 0 && index2 != 0 && index1 < source.Length && index2 < source.Length)
			{
				T temp = source[index1];
				source[index1] = source[index2];
				source[index2] = temp;
			}
		}

		public static void Swap<T>(this IList<T> list, int index1, int index2)
		{
			if (list != null && index1 >= 0 && index2 != 0 && index1 < list.Count && index2 < list.Count)
			{
				T temp = list[index1];
				list[index1] = list[index2];
				list[index2] = temp;
			}
		}

		public static void ShiftLeft<T>(this T[] source, int index, int count, int positions)
		{
			for (int j = 0; j < positions; ++j)
			{
				for (int i = index; i < index + count; ++i)
					source.Swap(i, i - 1);
				index--;
			}
		}

		public static void ShiftRight<T>(this T[] source, int index, int count, int positions)
		{
			for (int j = 0; j < positions; ++j)
			{
				for (int i = index + count - 1; i >= index; --i)
					source.Swap(i, i + 1);
				index++;
			}
		}

		public static bool AreValid(this Bounds bounds)
		{
			return !(float.IsNaN(bounds.center.x) || float.IsInfinity(bounds.center.x) ||
				float.IsNaN(bounds.center.y) || float.IsInfinity(bounds.center.y) ||
				float.IsNaN(bounds.center.z) || float.IsInfinity(bounds.center.z));
		}

		public static Bounds Transform(this Bounds b, Matrix4x4 m)
		{
			var xa = m.GetColumn(0) * b.min.x;
			var xb = m.GetColumn(0) * b.max.x;

			var ya = m.GetColumn(1) * b.min.y;
			var yb = m.GetColumn(1) * b.max.y;

			var za = m.GetColumn(2) * b.min.z;
			var zb = m.GetColumn(2) * b.max.z;

			Bounds result = new Bounds();
			Vector3 pos = m.GetColumn(3);
			result.SetMinMax(Vector3.Min(xa, xb) + Vector3.Min(ya, yb) + Vector3.Min(za, zb) + pos,
				Vector3.Max(xa, xb) + Vector3.Max(ya, yb) + Vector3.Max(za, zb) + pos);


			return result;
		}
		
		public static float Remap(this float value, float from1, float to1, float from2, float to2)
		{
			return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
		}

		public static Matrix4x4 Add(this Matrix4x4 a, Matrix4x4 other)
		{
			for (int i = 0; i < 16; ++i)
				a[i] += other[i];
			return a;
		}

		public static Matrix4x4 ScalarMultiply(this Matrix4x4 a, float s)
		{
			for (int i = 0; i < 16; ++i)
				a[i] *= s;
			return a;
		}


		public static Vector3 UnitOrthogonal(this Vector3 input)
		{
			// Find a vector to cross() the input with.
			if (!(input.x < input.z * Mathf.Epsilon)
				|| !(input.y < input.z * Mathf.Epsilon))
			{
				float invnm = 1 / Vector3.Magnitude(new Vector2(input.x, input.y));
				return new Vector3(-input.y * invnm, input.x * invnm, 0);
			}
			else
			{
				float invnm = 1 / Vector3.Magnitude(new Vector2(input.y, input.z));
				return new Vector3(0, -input.z * invnm, input.y * invnm);
			}
		}
		
		public static int PureSign(this float val)
		{
			return ((0 <= val) ? 1 : 0) - ((val < 0) ? 1 : 0);
		}
		
		///asumes = 9 is on the +x axis, and positive degrees in counterclockwise
		public static Vector2 DegreeToUnitVector2(this float degrees)
		{
			float radians = degrees * Mathf.Deg2Rad; 
			return new Vector2(Mathf.Sin(radians), Mathf.Cos(radians));
		}

		#region Collections

		public static void IndexedForEach<T>(this IEnumerable<T> collection, UnityEngine.Events.UnityAction<T, int> action)
		{
			int i = 0;
			foreach (T element in collection)
			{
				action(element, i++);
			}
		}

		public static void BufferedForEach<T>(this IEnumerable<T> collection, System.Func<T, bool> condition, UnityEngine.Events.UnityAction<T> performIf)
		{
			LinkedList<T> buffer = new LinkedList<T>();
			foreach (T obj in collection)
			{
				if (condition(obj))
				{
					buffer.AddFirst(obj);
				}
			}
			foreach (T obj in buffer)
			{
				performIf(obj);
			}
		}

		public static T MinValue<T>(this IEnumerable<T> collection, System.Func<T, float> heuristic)
		{
			T minObj = default(T);
			float min = float.PositiveInfinity;
			foreach (T t in collection)
			{
				float value = heuristic(t);
				if (value < min)
				{
					minObj = t;
					min = value;
				}
			}
			return minObj;
		}

		public static T MaxValue<T>(this IEnumerable<T> collection, System.Func<T, float> heuristic)
		{
			T minObj = default(T);
			float max = float.NegativeInfinity;
			foreach (T t in collection)
			{
				float value = heuristic(t);
				if (value > max)
				{
					minObj = t;
					max = value;
				}
			}
			return minObj;
		}

		public static int MinValueIndex<T>(this IEnumerable<T> collection, System.Func<T, float> heuristic)
		{
			int minIndex = 0;
			float min = float.PositiveInfinity;
			int index = 0;
			foreach (T t in collection)
			{
				float value = heuristic(t);
				if (value < min)
				{
					minIndex = index;
					min = value;
				}
				index++;
			}
			return minIndex;
		}

		public static int IndexOf<TSource>(this IEnumerable<TSource> source, System.Func<TSource, bool> predicate)
		{
			int index = 0;
			foreach (var item in source)
			{
				if (predicate(item))
				{
					return index;
				}
				index++;
			}
			return -1;
		}

		public static int LastIndexOf<TSource>(this IEnumerable<TSource> source, System.Func<TSource, bool> predicate)
		{
			int reverseIndex = source.Reverse().IndexOf(predicate);
			if (reverseIndex == -1)
			{
				return -1;
			}
			return source.Count() - 1 - reverseIndex;
		}

		public static bool AtLeast<T>(this IEnumerable<T> collection, int count, System.Func<T, bool> predicate = null)
		{
			if (predicate == null)
			{
				predicate = item => true;
			}

			int itemsSeen = 0;
			foreach (T item in collection)
			{
				if (predicate(item))
				{
					itemsSeen++;
					if (itemsSeen == count)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static IList<T> Shuffle<T>(this IList<T> list)
		{
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = UnityEngine.Random.Range(0, n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
			return list;
		}

		public static void Resize<T>(this List<T> list, int size, T element = default(T))
		{
			int count = list.Count;

			if (size < count)
			{
				list.RemoveRange(size, count - size);
			}
			else if (size > count)
			{
				if (size > list.Capacity)
				{
					list.Capacity = size; // Optimization.
				}
				list.AddRange(Enumerable.Repeat(element, size - count));
			}
		}

		public static T PickRandom<T>(this IList<T> list)
		{
			return list[UnityEngine.Random.Range(0, list.Count)];
		}

		public static T PickRandom<T>(this IEnumerable<T> list)
		{
			int count = list.Count();
			if (count == 0)
			{
				return default(T);
			}
			return list.ElementAt(UnityEngine.Random.Range(0, count));
		}


		public static T PickRandomFromEnumerable<T>(this IEnumerable<T> list)
		{
			int count = list.Count();
			if (count == 0)
			{
				return default(T);
			}
			return list.ElementAt(UnityEngine.Random.Range(0, count));
		}


		public static T PickRandomWithWeights<T>(this List<T> list, IList<float> weights)
		{
			float total = weights.Sum();
			float value = UnityEngine.Random.Range(0, total);

			int index = 0;
			float sum = 0;
			while (index < weights.Count && sum + weights[index] < value)
			{
				sum += weights[index];
				index++;
			}
			if (index >= weights.Count)
			{
				Debug.LogWarning("Something went wrong...");
				return list[0];
			}
			return list[index];
		}

		public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
		{
			TValue value;
			return dictionary.TryGetValue(key, out value) ? value : defaultValue;
		}

		#endregion
	}

}