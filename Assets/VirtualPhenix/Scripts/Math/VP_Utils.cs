using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace VirtualPhenix
{
    public static partial class VP_Utils
	{
		public static partial class Constants
		{
			public const int MAX_VERTICES_PER_MESH = 65000;
			public const int MAX_INSTANCES_PER_BATCH = 1023;

			public const float PI = 3.141593f;
			public const float TAU = 6.283185f;
			public const float PHI = 1.618034f;
			public const float E = 2.718281f;

			public static float[] PrecalculatedSquareRoots = new float[] { 0, 1, 1.42f, 1.73f, 2, 2.24f, 2.45f, 2.65f, 2.83f, 3, 3.16f, 3.32f, 3.46f, 3.61f, 3.74f, 3.87f, 4 };
			public static string[] AlphabetDictionary = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
			public static string[] CapitalAlphabetDictionary = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
			public static string[] ZeroToNineDictionary = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
			public static string[] AlphabetAndZeroToNineDictionary = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
			public static string[] AlphabetAndAndCapitalAlphabetAndZeroToNineDictionary = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

		}

		public static partial class Math
		{
			public static float RandomNumber(float min, float max)
            {
				return UnityEngine.Random.Range(min, max);
            }

			public static Vector3 RandomPointAroundPosition(Vector3 pos, float multiplier = 1)
			{
				return (pos + UnityEngine.Random.insideUnitSphere * multiplier);
			}

			public static Vector3 GetRandomPositionForObject(Transform _objectTransform, MeshFilter _meshFilter)
			{

				if (_meshFilter == null)
					return Vector3.zero;

				Mesh planeMesh = _meshFilter.mesh;
				Bounds bounds = planeMesh.bounds;

				float minX = _objectTransform.position.x - _objectTransform.localScale.x * bounds.size.x * 0.5f;
				float minZ = _objectTransform.position.z - _objectTransform.localScale.z * bounds.size.z * 0.5f;

				Vector3 newVec = new Vector3(UnityEngine.Random.Range (minX, -minX),
					_objectTransform.position.y,
					UnityEngine.Random.Range (minZ, -minZ));
				return newVec;
			}

			public static Vector3 RandomPointOnPlane(Plane _plane, Vector3 _position, float _radius)
			{
				Vector3 randomPoint;
 
				do
				{
					randomPoint = Vector3.Cross(UnityEngine.Random.insideUnitSphere, _plane.normal);
				} while (randomPoint == Vector3.zero);

				randomPoint.Normalize();
				randomPoint *= _radius;
				randomPoint += _plane.normal;

				return randomPoint;
			}

			public static Vector3 RandomPointOnPlane(Vector3 _position,Vector3 _normal,  float _radius)
			{
				Vector3 randomPoint;
 
				do
				{
					randomPoint = Vector3.Cross(UnityEngine.Random.insideUnitSphere, _normal);
				} while (randomPoint == Vector3.zero);

				randomPoint.Normalize();
				randomPoint *= _radius;
				randomPoint += _normal;

				return randomPoint;
			}


			public static Vector3 RandomPointInBounds(Bounds bounds)
			{
				return new Vector3(
					UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
					UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
					UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
				);
			}
    	
			public static System.Int32 CreatePersonalID()
			{
				System.Int32 m_personalID  = UnityEngine.Random.Range(0, 256);
				m_personalID |= UnityEngine.Random.Range(0, 256) << 8;
				m_personalID |= UnityEngine.Random.Range(0, 256) << 16;
				m_personalID |= UnityEngine.Random.Range(0, 256) << 24;
				return m_personalID;
			}

			public static int ObjectToInt(object value)
			{
				if (value == null)
				{
					return 0;
				}
				else if (value is Color)
				{
					return ColorToInt((Color)value);
				}
				else if (value is Color32)
				{
					return ColorToInt((Color32)value);
				}
				else if (value is System.IConvertible)
				{
					try
					{
						return System.Convert.ToInt32(value);
					}
						catch
						{
							return 0;
						}
				}
				else
				{
					return ObjectToInt(value.ToString());
				}
			}

			public static void SetRectValues(RectTransform rt, Vector2 leftRight, Vector2 topBottom)
			{
				SetRectLeft(rt, leftRight.x);
				SetRectRight(rt, leftRight.y);
				SetRectTop(rt, topBottom.x);
				SetRectBottom(rt, topBottom.y);
			}
			public static void SetRectValues(RectTransform rt, float left, float right, float top, float bottom)
			{
				SetRectLeft(rt, left);
				SetRectRight(rt, right);
				SetRectTop(rt, top);
				SetRectBottom(rt, bottom);
			}

			public static float GetRectLeft(RectTransform rt)
			{
				return rt.offsetMin.x;
			}

			public static float GetRectRight(RectTransform rt)
			{
				return rt.offsetMax.x * -1;
			}

			public static float GetRectTop(RectTransform rt)
			{
				return rt.offsetMax.y * -1;
			}

			public static float GetRectBottom(RectTransform rt)
			{
				return rt.offsetMin.y;
			}

			public static Vector2 GetRectLeftRight(RectTransform rt)
			{
				return new Vector2(GetRectLeft(rt), GetRectRight(rt));
			}

			public static Vector2 GetRectTopBottom(RectTransform rt)
			{
				return new Vector2(GetRectTop(rt), GetRectBottom(rt));
			}

			public static void SetRectLeft(RectTransform rt, float left)
			{
				rt.offsetMin = new Vector2(left, rt.offsetMin.y);
			}

			public static void SetRectRight(RectTransform rt, float right)
			{
				rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
			}

			public static void SetRectTop(RectTransform rt, float top)
			{
				rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
			}

			public static void SetRectBottom(RectTransform rt, float bottom)
			{
				rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
			}

			public static float GetRandomDeviation(float value, float deviation)
			{
				return value + deviation * (UnityEngine.Random.value * 2f - 1f);
			}

			public static float AreaOfTriangle(Vector2 pt1, Vector2 pt2, Vector2 pt3)
			{
				float len = Vector2.Distance(pt1, pt2);
				float len2 = Vector2.Distance(pt2, pt3);
				float len3 = Vector2.Distance(pt3, pt1);
				return AreaOfTriangle(len, len2, len3);
			}

			public static float AreaOfTriangle(float len1, float len2, float len3)
			{
				float num = (len1 + len2 + len3) / 2f;
				return Mathf.Sqrt(Mathf.Max(num * (num - len1) * (num - len2) * (num - len3), 0f));
			}

			public static Color ObjectToColor(object value)
			{
				if (value is Color) return (Color)value;
				if (value is Color32) return ObjectToColor((Color32)value);
				if (value is Vector3) return ObjectToColor((Vector3)value);
				if (value is Vector4) return ObjectToColor((Vector4)value);
				return ObjectToColor(ObjectToInt(value));
			}

			public static int ColorToInt(Color32 color)
			{
				return (color.a << 24) +
				(color.r << 16) +
				(color.g << 8) +
					color.b;
			}

			public static Vector3 PointOnPlaneBetweenTwoPoints(Plane p, Vector3 a, Vector3 b)
			{
				Vector3 q = (b-a);
				q.Normalize();
				Vector3 planeEqation;
				Vector3 pointOnPlane = p.ClosestPointOnPlane(Vector3.zero);
				Vector3 normal = p.normal;
				planeEqation = normal;
				float offset = Vector3.Dot(pointOnPlane,normal);
				Debug.Log(planeEqation);
				float t = (offset-Vector3.Dot(a,planeEqation))/Vector3.Dot(q,planeEqation);
         
				return a+(q*t);
			}

			// Colour alphabet from https://www.aic-color.org/resources/Documents/jaic_v5_06.pdf
			public static readonly Color32[] colorAlphabet = new Color32[26]
			{
				new Color32(240,163,255,255),
				new Color32(0,117,220,255),
				new Color32(153,63,0,255),
				new Color32(76,0,92,255),
				new Color32(25,25,25,255),
				new Color32(0,92,49,255),
				new Color32(43,206,72,255),
				new Color32(255,204,153,255),
				new Color32(128,128,128,255),
				new Color32(148,255,181,255),
				new Color32(143,124,0,255),
				new Color32(157,204,0,255),
				new Color32(194,0,136,255),
				new Color32(0,51,128,255),
				new Color32(255,164,5,255),
				new Color32(255,168,187,255),
				new Color32(66,102,0,255),
				new Color32(255,0,16,255),
				new Color32(94,241,242,255),
				new Color32(0,153,143,255),
				new Color32(224,255,102,255),
				new Color32(116,10,255,255),
				new Color32(153,0,0,255),
				new Color32(255,255,128,255),
				new Color32(255,255,0,255),
				new Color32(255,80,5,255)
			};

			public static void DrawArrowGizmo(float bodyLenght, float bodyWidth, float headLenght, float headWidth)
			{

				float halfBodyLenght = bodyLenght * 0.5f;
				float halfBodyWidth = bodyWidth * 0.5f;

				// arrow body:
				Gizmos.DrawLine(new Vector3(halfBodyWidth, 0, -halfBodyLenght), new Vector3(halfBodyWidth, 0, halfBodyLenght));
				Gizmos.DrawLine(new Vector3(-halfBodyWidth, 0, -halfBodyLenght), new Vector3(-halfBodyWidth, 0, halfBodyLenght));
				Gizmos.DrawLine(new Vector3(-halfBodyWidth, 0, -halfBodyLenght), new Vector3(halfBodyWidth, 0, -halfBodyLenght));

				// arrow head:
				Gizmos.DrawLine(new Vector3(halfBodyWidth, 0, halfBodyLenght), new Vector3(headWidth, 0, halfBodyLenght));
				Gizmos.DrawLine(new Vector3(-halfBodyWidth, 0, halfBodyLenght), new Vector3(-headWidth, 0, halfBodyLenght));
				Gizmos.DrawLine(new Vector3(0, 0, halfBodyLenght + headLenght), new Vector3(headWidth, 0, halfBodyLenght));
				Gizmos.DrawLine(new Vector3(0, 0, halfBodyLenght + headLenght), new Vector3(-headWidth, 0, halfBodyLenght));
			}

			public static void DebugDrawCross(Vector3 pos, float size, Color color)
			{
				Debug.DrawLine(pos - Vector3.right * size, pos + Vector3.right * size, color);
				Debug.DrawLine(pos - Vector3.up * size, pos + Vector3.up * size, color);
				Debug.DrawLine(pos - Vector3.forward * size, pos + Vector3.forward * size, color);
			}

			public static void Swap<T>(ref T lhs, ref T rhs)
			{
				T temp = lhs;
				lhs = rhs;
				rhs = temp;
			}


			public static void Add(Vector3 a, Vector3 b, ref Vector3 result)
			{
				result.x = a.x + b.x;
				result.y = a.y + b.y;
				result.z = a.z + b.z;
			}

			/**
			* Modulo operator that also follows intuition for negative arguments. That is , -1 mod 3 = 2, not -1.
			*/
			public static float Mod(float a, float b)
			{
				return a - b * Mathf.Floor(a / b);
			}

			
			public static Vector3 ProjectPointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd, out float mu, bool clampToSegment = true)
			{
				Vector3 ap = point - lineStart;
				Vector3 ab = lineEnd - lineStart;

				mu = Vector3.Dot(ap, ab) / Vector3.Dot(ab, ab);

				if (clampToSegment)
					mu = Mathf.Clamp01(mu);

				return lineStart + ab * mu;
			}

			public static bool LinePlaneIntersection(Vector3 planePoint, Vector3 planeNormal, Vector3 linePoint, Vector3 lineDirection, out Vector3 point)
			{
				point = linePoint;
				Vector3 lineNormal = lineDirection.normalized;
				float denom = Vector3.Dot(planeNormal, lineNormal);

				if (Mathf.Approximately(denom, 0))
					return false;

				float t = (Vector3.Dot(planeNormal, planePoint) - Vector3.Dot(planeNormal, linePoint)) / denom;
				point = linePoint + lineNormal * t;
				return true;
			}

			public static float InvMassToMass(float invMass)
			{
				return 1.0f / invMass;
			}

			public static float MassToInvMass(float mass)
			{
				return 1.0f / Mathf.Max(mass, 0.00001f);
			}

			public static void NearestPointOnTri(in Vector3 p1,
				in Vector3 p2,
				in Vector3 p3,
				in Vector3 p,
				out Vector3 result)
			{
				float e0x = p2.x - p1.x;
				float e0y = p2.y - p1.y;
				float e0z = p2.z - p1.z;

				float e1x = p3.x - p1.x;
				float e1y = p3.y - p1.y;
				float e1z = p3.z - p1.z;

				float v0x = p1.x - p.x;
				float v0y = p1.y - p.y;
				float v0z = p1.z - p.z;

				float a00 = e0x * e0x + e0y * e0y + e0z * e0z;
				float a01 = e0x * e1x + e0y * e1y + e0z * e1z;
				float a11 = e1x * e1x + e1y * e1y + e1z * e1z;
				float b0 = e0x * v0x + e0y * v0y + e0z * v0z;
				float b1 = e1x * v0x + e1y * v0y + e1z * v0z;

				const float zero = 0;
				const float one = 1;

				float det = a00 * a11 - a01 * a01;
				float t0 = a01 * b1 - a11 * b0;
				float t1 = a01 * b0 - a00 * b1;

				if (t0 + t1 <= det)
				{
					if (t0 < zero)
					{
						if (t1 < zero)  // region 4
						{
							if (b0 < zero)
							{
								t1 = zero;
								if (-b0 >= a00)  // V0
								{
									t0 = one;
								}
								else  // E01
								{
									t0 = -b0 / a00;
								}
							}
							else
							{
								t0 = zero;
								if (b1 >= zero)  // V0
								{
									t1 = zero;
								}
								else if (-b1 >= a11)  // V2
								{
									t1 = one;
								}
								else  // E20
								{
									t1 = -b1 / a11;
								}
							}
						}
						else  // region 3
						{
							t0 = zero;
							if (b1 >= zero)  // V0
							{
								t1 = zero;
							}
							else if (-b1 >= a11)  // V2
							{
								t1 = one;
							}
							else  // E20
							{
								t1 = -b1 / a11;
							}
						}
					}
					else if (t1 < zero)  // region 5
					{
						t1 = zero;
						if (b0 >= zero)  // V0
						{
							t0 = zero;
						}
						else if (-b0 >= a00)  // V1
						{
							t0 = one;
						}
						else  // E01
						{
							t0 = -b0 / a00;
						}
					}
					else  // region 0, interior
					{
						float invDet = one / det;
						t0 *= invDet;
						t1 *= invDet;
					}
				}
				else
				{
					float tmp0, tmp1, numer, denom;

					if (t0 < zero)  // region 2
					{
						tmp0 = a01 + b0;
						tmp1 = a11 + b1;
						if (tmp1 > tmp0)
						{
							numer = tmp1 - tmp0;
							denom = a00 - 2 * a01 + a11;
							if (numer >= denom)  // V1
							{
								t0 = one;
								t1 = zero;
							}
							else  // E12
							{
								t0 = numer / denom;
								t1 = one - t0;
							}
						}
						else
						{
							t0 = zero;
							if (tmp1 <= zero)  // V2
							{
								t1 = one;
							}
							else if (b1 >= zero)  // V0
							{
								t1 = zero;
							}
							else  // E20
							{
								t1 = -b1 / a11;
							}
						}
					}
					else if (t1 < zero)  // region 6
					{
						tmp0 = a01 + b1;
						tmp1 = a00 + b0;
						if (tmp1 > tmp0)
						{
							numer = tmp1 - tmp0;
							denom = a00 - 2 * a01 + a11;
							if (numer >= denom)  // V2
							{
								t1 = one;
								t0 = zero;
							}
							else  // E12
							{
								t1 = numer / denom;
								t0 = one - t1;
							}
						}
						else
						{
							t1 = zero;
							if (tmp1 <= zero)  // V1
							{
								t0 = one;
							}
							else if (b0 >= zero)  // V0
							{
								t0 = zero;
							}
							else  // E01
							{
								t0 = -b0 / a00;
							}
						}
					}
					else  // region 1
					{
						numer = a11 + b1 - a01 - b0;
						if (numer <= zero)  // V2
						{
							t0 = zero;
							t1 = one;
						}
						else
						{
							denom = a00 - 2 * a01 + a11;
							if (numer >= denom)  // V1
							{
								t0 = one;
								t1 = zero;
							}
							else  // 12
							{
								t0 = numer / denom;
								t1 = one - t0;
							}
						}
					}
				}

				result.x = p1.x + t0 * e0x + t1 * e1x;
				result.y = p1.y + t0 * e0y + t1 * e1y;
				result.z = p1.z + t0 * e0z + t1 * e1z;
			}

			/**
			* Calculates the area of a triangle.
			*/
			public static float TriangleArea(Vector3 p1, Vector3 p2, Vector3 p3)
			{
				return Mathf.Sqrt(Vector3.Cross(p2 - p1, p3 - p1).sqrMagnitude) / 2f;
			}

			public static float EllipsoidVolume(Vector3 principalRadii)
			{
				return 4.0f / 3.0f * Mathf.PI * principalRadii.x * principalRadii.y * principalRadii.z;
			}

			public static Quaternion RestDarboux(Quaternion q1, Quaternion q2)
			{
				Quaternion darboux = Quaternion.Inverse(q1) * q2;
				Vector4 omega_plus, omega_minus;
				omega_plus = new Vector4(darboux.w, darboux.x, darboux.y, darboux.z) + new Vector4(1, 0, 0, 0);
				omega_minus = new Vector4(darboux.w, darboux.x, darboux.y, darboux.z) - new Vector4(1, 0, 0, 0);
				if (omega_minus.sqrMagnitude > omega_plus.sqrMagnitude)
				{
					darboux = new Quaternion(darboux.x * -1, darboux.y * -1, darboux.z * -1, darboux.w * -1);
				}
				return darboux;
			}

			public static float RestBendingConstraint(Vector3 positionA, Vector3 positionB, Vector3 positionC)
			{
				Vector3 center = (positionA + positionB + positionC) / 3;
				return (positionC - center).magnitude;
			}

			public static System.Collections.IEnumerable BilateralInterleaved(int count)
			{
				for (int i = 0; i < count; ++i)
				{
					if (i % 2 != 0)
						yield return count - (count % 2) - i;
					else yield return i;
				}
			}

			public static void BarycentricCoordinates(in Vector3 A,
				in Vector3 B,
				in Vector3 C,
				in Vector3 P,
				ref Vector3 bary)
			{

				// Compute vectors
				Vector3 v0 = C - A;
				Vector3 v1 = B - A;
				Vector3 v2 = P - A;

				// Compute dot products
				float dot00 = Vector3.Dot(v0, v0);
				float dot01 = Vector3.Dot(v0, v1);
				float dot02 = Vector3.Dot(v0, v2);
				float dot11 = Vector3.Dot(v1, v1);
				float dot12 = Vector3.Dot(v1, v2);

				// Compute barycentric coordinates
				float det = dot00 * dot11 - dot01 * dot01;
				if (System.Math.Abs(det) > 1E-38)
				{
					float u = (dot11 * dot02 - dot01 * dot12) / det;
					float v = (dot00 * dot12 - dot01 * dot02) / det;
					bary = new Vector3(1 - u - v, v, u);
				}

			}

			public static void BarycentricInterpolation(in Vector3 p1, in Vector3 p2, in Vector3 p3, in Vector3 coords, out Vector3 result)
			{
				result.x = coords.x * p1.x + coords.y * p2.x + coords.z * p3.x;
				result.y = coords.x * p1.y + coords.y * p2.y + coords.z * p3.y;
				result.z = coords.x * p1.z + coords.y * p2.z + coords.z * p3.z;
			}

			public static float BarycentricInterpolation(float p1, float p2, float p3, Vector3 coords)
			{
				return coords[0] * p1 + coords[1] * p2 + coords[2] * p3;
			}

			public static float BarycentricExtrapolationScale(Vector3 coords)
			{

				return 1.0f / (coords[0] * coords[0] +
					coords[1] * coords[1] +
					coords[2] * coords[2]);

			}

			public static Vector3[] CalculateAngleWeightedNormals(Vector3[] vertices, int[] triangles)
			{
				Vector3[] normals = new Vector3[vertices.Length];
				var normalBuffer = new Dictionary<Vector3, Vector3>();

				Vector3 v1, v2, v3, e1, e2;
				for (int i = 0; i < triangles.Length; i += 3)
				{
					v1 = vertices[triangles[i]];
					v2 = vertices[triangles[i + 1]];
					v3 = vertices[triangles[i + 2]];

					if (!normalBuffer.ContainsKey(v1))
						normalBuffer[v1] = Vector3.zero;
					if (!normalBuffer.ContainsKey(v2))
						normalBuffer[v2] = Vector3.zero;
					if (!normalBuffer.ContainsKey(v3))
						normalBuffer[v3] = Vector3.zero;

					e1 = v2 - v1;
					e2 = v3 - v1;
					normalBuffer[v1] += Vector3.Cross(e1, e2).normalized * Mathf.Acos(Vector3.Dot(e1.normalized, e2.normalized));

					e1 = v3 - v2;
					e2 = v1 - v2;
					normalBuffer[v2] += Vector3.Cross(e1, e2).normalized * Mathf.Acos(Vector3.Dot(e1.normalized, e2.normalized));

					e1 = v1 - v3;
					e2 = v2 - v3;
					normalBuffer[v3] += Vector3.Cross(e1, e2).normalized * Mathf.Acos(Vector3.Dot(e1.normalized, e2.normalized));
				}

				for (int i = 0; i < vertices.Length; ++i)
					normals[i] = normalBuffer[vertices[i]].normalized;

				return normals;
			}

			public static void EigenSolve(Matrix4x4 D, out Vector3 S, out Matrix4x4 V)
			{
				// D is symmetric
				// S is a vector whose elements are eigenvalues
				// V is a matrix whose columns are eigenvectors
				S = EigenValues(D);
				Vector3 V0, V1, V2;

				if (S[0] - S[1] > S[1] - S[2])
				{
					V0 = EigenVector(D, S[0]);
					if (S[1] - S[2] < Mathf.Epsilon)
					{
						V2 = V0.UnitOrthogonal();
					}
					else
					{
						V2 = EigenVector(D, S[2]); V2 -= V0 * Vector3.Dot(V0, V2); V2 = Vector3.Normalize(V2);
					}
					V1 = Vector3.Cross(V2, V0);
				}
				else
				{
					V2 = EigenVector(D, S[2]);
					if (S[0] - S[1] < Mathf.Epsilon)
					{
						V1 = V2.UnitOrthogonal();
					}
					else
					{
						V1 = EigenVector(D, S[1]); V1 -= V2 * Vector3.Dot(V2, V1); V1 = Vector3.Normalize(V1);
					}
					V0 = Vector3.Cross(V1, V2);
				}

				V = Matrix4x4.identity;
				V.SetColumn(0, V0);
				V.SetColumn(1, V1);
				V.SetColumn(2, V2);
			}


			// D is symmetric, S is an eigen value
			public static Vector3 EigenVector(Matrix4x4 D, float S)
			{
				// Compute a cofactor matrix of D - sI.
				Vector4 c0 = D.GetColumn(0); c0[0] -= S;
				Vector4 c1 = D.GetColumn(1); c1[1] -= S;
				Vector4 c2 = D.GetColumn(2); c2[2] -= S;

				// Use an upper triangle
				Vector3 c0p = new Vector3(c1[1] * c2[2] - c2[1] * c2[1], 0, 0);
				Vector3 c1p = new Vector3(c2[1] * c2[0] - c1[0] * c2[2], c0[0] * c2[2] - c2[0] * c2[0], 0);
				Vector3 c2p = new Vector3(c1[0] * c2[1] - c1[1] * c2[0], c1[0] * c2[0] - c0[0] * c2[1], c0[0] * c1[1] - c1[0] * c1[0]);

				// Get a column vector with a largest norm (non-zero).
				float C01s = c1p[0] * c1p[0];
				float C02s = c2p[0] * c2p[0];
				float C12s = c2p[1] * c2p[1];
				Vector3 norm = new Vector3(c0p[0] * c0p[0] + C01s + C02s,
					C01s + c1p[1] * c1p[1] + C12s,
					C02s + C12s + c2p[2] * c2p[2]);

				// index of largest:
				int index = 0;
				if (norm[0] > norm[1] && norm[0] > norm[2])
					index = 0;
				else if (norm[1] > norm[0] && norm[1] > norm[2])
					index = 1;
				else
					index = 2;

				Vector3 V = Vector3.zero;

				// special case
				if (norm[index] < Mathf.Epsilon)
				{
					V[0] = 1; return V;
				}
				else if (index == 0)
				{
					V[0] = c0p[0]; V[1] = c1p[0]; V[2] = c2p[0];
				}
				else if (index == 1)
				{
					V[0] = c1p[0]; V[1] = c1p[1]; V[2] = c2p[1];
				}
				else
				{
					V = c2p;
				}
				return Vector3.Normalize(V);
			}

			public static Vector3 EigenValues(Matrix4x4 D)
			{
				float one_third = 1 / 3.0f;
				float one_sixth = 1 / 6.0f;
				float three_sqrt = Mathf.Sqrt(3.0f);

				Vector3 c0 = D.GetColumn(0);
				Vector3 c1 = D.GetColumn(1);
				Vector3 c2 = D.GetColumn(2);

				float m = one_third * (c0[0] + c1[1] + c2[2]);

				// K is D - I*diag(S)
				float K00 = c0[0] - m;
				float K11 = c1[1] - m;
				float K22 = c2[2] - m;

				float K01s = c1[0] * c1[0];
				float K02s = c2[0] * c2[0];
				float K12s = c2[1] * c2[1];

				float q = 0.5f * (K00 * (K11 * K22 - K12s) - K22 * K01s - K11 * K02s) + c1[0] * c2[1] * c0[2];
				float p = one_sixth * (K00 * K00 + K11 * K11 + K22 * K22 + 2 * (K01s + K02s + K12s));

				float p_sqrt = Mathf.Sqrt(p);

				float tmp = p * p * p - q * q;
				float phi = one_third * Mathf.Atan2(Mathf.Sqrt(Mathf.Max(0, tmp)), q);
				float phi_c = Mathf.Cos(phi);
				float phi_s = Mathf.Sin(phi);
				float sqrt_p_c_phi = p_sqrt * phi_c;
				float sqrt_p_3_s_phi = p_sqrt * three_sqrt * phi_s;

				float e0 = m + 2 * sqrt_p_c_phi;
				float e1 = m - sqrt_p_c_phi - sqrt_p_3_s_phi;
				float e2 = m - sqrt_p_c_phi + sqrt_p_3_s_phi;

				float aux;
				if (e0 > e1)
				{
					aux = e0;
					e0 = e1;
					e1 = aux;
				}
				if (e0 > e2)
				{
					aux = e0;
					e0 = e2;
					e2 = aux;
				}
				if (e1 > e2)
				{
					aux = e1;
					e1 = e2;
					e2 = aux;
				}

				return new Vector3(e2, e1, e0);
			}

			public static Vector3 GetPointCloudCentroid(List<Vector3> points)
			{
				Vector3 centroid = Vector3.zero;
				for (int i = 0; i < points.Count; ++i)
					centroid += points[i];
				return centroid / points.Count;
			}

			///asumes = 9 is on the +x axis, and positive degrees in counterclockwise
			public static Vector2 DegreeToUnitVector2(float degrees)
			{
				float radians = degrees * Mathf.Deg2Rad; 
				return new Vector2(Mathf.Sin(radians), Mathf.Cos(radians));
			}

			public static void GetPointCloudAnisotropy(List<Vector3> points, float max_anisotropy, float radius, ref Vector3 hint_normal, ref Vector3 centroid, ref Quaternion orientation, ref Vector3 principal_radii)
			{
				int count = points.Count;
				if (count == 0 || radius <= 0 || max_anisotropy <= 0)
				{
					principal_radii = Vector3.one * radius;
					orientation = Quaternion.identity;
					return;
				}

				centroid = GetPointCloudCentroid(points);

				// three columns of a 3x3 anisotropy matrix:
				Vector4 c0 = Vector4.zero,
					c1 = Vector4.zero,
					c2 = Vector4.zero;

				Matrix4x4 anisotropy = Matrix4x4.zero;

				// multiply offset by offset transposed, add to matrix, and average.
				for (int i = 0; i < count; i++)
				{
					Vector4 offset = points[i] - centroid;
					c0 += offset * offset[0];
					c1 += offset * offset[1];
					c2 += offset * offset[2];
				}

				anisotropy.SetColumn(0, c0 / count);
				anisotropy.SetColumn(1, c1 / count);
				anisotropy.SetColumn(2, c2 / count);

				Matrix4x4 orientMat;
				EigenSolve(anisotropy, out principal_radii, out orientMat);

				// flip orientation if it is not in the same side as the hint normal:
				if (Vector3.Dot(orientMat.GetColumn(2), hint_normal) < 0)
				{
					orientMat.SetColumn(2, orientMat.GetColumn(2) * -1);
					orientMat.SetColumn(1, orientMat.GetColumn(1) * -1);
				}

				float max = principal_radii[0];
				principal_radii = Vector3.Max(principal_radii, Vector3.one * max / max_anisotropy) / max * radius;
				orientation = orientMat.rotation;
			}

			public static int AverageIntArray(int[] Ints)
			{ //Average an array of integers
				int total = 0;

				for (int i = 0; i < Ints.Length; i++)
				{
					total += Ints[i];
				}
				total = total / Ints.Length;

				return total;
			}

			public static int AddIntArray(int[] Ints)
			{
				int total = 0;

				for (int i = 0; i < Ints.Length; i++)
				{
					total += Ints[i];
				}

				return total;
			}

			public static float AverageFloatArray(float[] Ints)
			{ //Average an array of floats
				float total = 0;

				for (int i = 0; i < Ints.Length; i++)
				{
					total += Ints[i];
				}
				total = total / Ints.Length;

				return total;
			}

			public static float AddFloatArray(float[] floats)
			{
				float total = 0;

				for (int i = 0; i < floats.Length; i++)
				{
					total += floats[i];
				}

				return total;
			}

			public static string GetStringChars(string String, int startChar, int endChar)
			{
				string final = "";

				for (int i = startChar; i < endChar; i++)
				{
					final += String.Substring(i, 1);
				}

				return final;
			}

			public static Color RandomColor()
			{
				Color final = new Color(UnityEngine.Random.Range(0, 255), UnityEngine.Random.Range(0, 255), UnityEngine.Random.Range(0, 255));

				final.r = final.r / 255;
				final.g = final.g / 255;
				final.b = final.b / 255;

				return final;
			}

			public static Color RandomColorWithAlpha()
			{
				Color final = new Color(UnityEngine.Random.Range(0, 255), UnityEngine.Random.Range(0, 255), UnityEngine.Random.Range(0, 255), UnityEngine.Random.Range(0, 255));

				final.r = final.r / 255;
				final.g = final.g / 255;
				final.b = final.b / 255;
				final.a = final.a / 255;

				return final;
			}

			public static Color RandomSmartColor(int vividness, int damping)
			{
				Color final = new Color(0, 0, 0);

				int r = UnityEngine.Random.Range(0, 255);
				int g = UnityEngine.Random.Range(0, 255);
				int b = UnityEngine.Random.Range(0, 255);

				if (r > vividness)
				{
					g = g - damping;
					b = b - damping;
				}

				if (g > vividness)
				{
					r = r - damping;
					b = b - damping;
				}

				if (b > vividness)
				{
					r = r - damping;
					g = g - damping;
				}

				if (r < 0)
				{
					r = 0;
				}

				if (g < 0)
				{
					g = 0;
				}

				if (b < 0)
				{
					b = 0;
				}

				final.r = r;
				final.g = g;
				final.b = b;

				final.r = final.r / 255;
				final.g = final.g / 255;
				final.b = final.b / 255;

				return final;
			}

			public static int[] RandomIntArray(int min, int max, int count)
			{
				int[] final = new int[count];

				for (int i = 0; i < count; i++)
				{
					final[i] = UnityEngine.Random.Range(min, max);
				}

				return final;
			}

			public static float[] RandomFloatArray(float min, float max, int count)
			{
				float[] final = new float[count];

				for (int i = 0; i < count; i++)
				{
					final[i] = UnityEngine.Random.Range(min, max);
				}

				return final;
			}

			public static string RandomString(string[] Dictionary, int length)
			{
				string final = "";

				for (int i = 0; i < length; i++)
				{
					int num = UnityEngine.Random.Range(0, Dictionary.Length);
					final += Dictionary[num];
				}

				return final;
			}

			public static string RandomStringUsingAlphabet(int length)
			{
				string final = "";

				for (int i = 0; i < length; i++)
				{
					int num = UnityEngine.Random.Range(0, Constants.AlphabetDictionary.Length);
					final += Constants.AlphabetDictionary[num];
				}

				return final;
			}

			public static string RandomStringUsingNumberDictionary(int length)
			{
				string final = "";

				for (int i = 0; i < length; i++)
				{
					int num = UnityEngine.Random.Range(0, Constants.ZeroToNineDictionary.Length);
					final += Constants.ZeroToNineDictionary[num];
				}

				return final;
			}

			public static string[] RandomStringArray(string[] Dictionary, int length, int count)
			{
				string[] final = new string[count];

				for (int c = 0; c < count; c++)
				{

					string current = "";

					for (int i = 0; i < length; i++)
					{
						int num = UnityEngine.Random.Range(0, Dictionary.Length);
						current += Dictionary[num];
					}

					final[c] = current;
				}

				return final;
			}

			public static string[] RandomStringArrayUsingAlphebet(int length, int count)
			{
				string[] final = new string[count];

				for (int c = 0; c < count; c++)
				{

					string current = "";

					for (int i = 0; i < length; i++)
					{
						int num = UnityEngine.Random.Range(0, Constants.AlphabetDictionary.Length);
						current += Constants.AlphabetDictionary[num];
					}

					final[c] = current;
				}

				return final;
			}

			public static string[] RandomStringArrayNumberDictionary(int length, int count)
			{
				string[] final = new string[count];

				for (int c = 0; c < count; c++)
				{

					string current = "";

					for (int i = 0; i < length; i++)
					{
						int num = UnityEngine.Random.Range(0, Constants.ZeroToNineDictionary.Length);
						current += Constants.ZeroToNineDictionary[num];
					}

					final[c] = current;
				}

				return final;
			}

			public static string[] ReplaceInStringArray(string[] StringArray, string ToReplace, string Replacement)
			{
				string[] final = new string[StringArray.Length];

				for (int i = 0; i < StringArray.Length; i++)
				{
					string str = StringArray[i];

					str = str.Replace(ToReplace, Replacement);

					final[i] = str;
				}

				return final;
			}

			public static string RemoveCommasFromString(string String)
			{
				string final = String;
				final = String.Replace(",", "");
				return final;
			}

			public static string RemovePeriodsFromString(string String)
			{
				string final = String;
				final = String.Replace(".", "");
				return final;
			}

			public static int ParseIntSafe(string String)
			{
				int final = 0;

				String = RemoveCommasFromString(String);
				String = RemovePeriodsFromString(String);

				int.Parse(String);

				return final;
			}

			public static float TempretureConvertFtoC(float F)
			{
				float final = (F - 32) * 5 / 9;
				return final;
			}

			public static float TempretureConvertFtoK(float F)
			{
				float final = (F - 32) * 5 / 9 + 273.15f;
				return final;
			}

			public static float TempretureConvertCtoF(float C)
			{
				float final = (C * 9 / 5) + 32;
				return final;
			}

			public static float TempretureConvertCtoK(float C)
			{
				float final = C + 273.15f;
				return final;
			}

			public static float TempretureConvertKtoF(float K)
			{
				float final = (K - 273.15f) * 9 / 5 + 32;
				return final;
			}

			public static float TempretureConvertKtoC(float K)
			{
				float final = K - 273.15f;
				return final;
			}

			public static float PercentError(float a, float b)
			{
				float final = (a - b) / a;
				return final;
			}

			public static int[] CompareIntArrayDifference(int toCompare, int[] array)
			{
				int[] final = new int[array.Length];

				for (int i = 0; i < array.Length; i++)
				{
					final[i] = toCompare - array[i];
				}

				return final;
			}

			public static float[] CompareFloatArrayDifference(float toCompare, float[] array)
			{
				float[] final = new float[array.Length];

				for (int i = 0; i < array.Length; i++)
				{
					final[i] = toCompare - array[i];
				}

				return final;
			}

			public static Vector2 Vector2ParabolaPoint(Vector2 a, Vector2 b, Vector2 c, float t)
			{
				t = Mathf.Clamp01(t);

				Vector2 q = (1 - t) * a + t * b;
				Vector2 r = (1 - t) * b + t * c;
				Vector2 p = (1 - t) * q + t * r;

				return p;
			}

			public static Vector3 Vector3ParabolaPoint(Vector3 a, Vector3 b, Vector3 c, float t)
			{
				t = Mathf.Clamp01(t);

				Vector3 q = (1 - t) * a + t * b;
				Vector3 r = (1 - t) * b + t * c;
				Vector3 p = (1 - t) * q + t * r;

				return p;
			}

			public static Vector2[] Vector2Parabola(Vector2 a, Vector2 b, Vector2 c, int quality)
			{
				Vector2[] final = new Vector2[quality];

				float interval = 1 / (float)quality;
				float t = 0;

				for (int i = 0; i < quality; i++)
				{
					t += interval;

					Vector2 q = (1 - t) * a + t * b;
					Vector2 r = (1 - t) * b + t * c;
					Vector2 p = (1 - t) * q + t * r;
					final[i] = p;
				}

				return final;
			}

			public static Vector3[] Vector3Parabola(Vector3 a, Vector3 b, Vector3 c, int quality)
			{
				Vector3[] final = new Vector3[quality];

				Vector3 q;
				Vector3 r;
				Vector3 p;

				float interval = 1 / (float)quality;
				float t = 0;

				for (int i = 0; i < quality; i++)
				{
					t += interval;

					q = (1 - t) * a + t * b;
					r = (1 - t) * b + t * c;
					p = (1 - t) * q + t * r;
					final[i] = p;
				}

				return final;
			}

			public static int IntWeightedAverage(int a, int b, float t)
			{
				t = Mathf.Clamp01(t);

				int final = (int)((1 - t) * a + t * b);
				return final;
			}

			public static float FloatWeightedAverage(float a, float b, float t)
			{
				t = Mathf.Clamp01(t);

				float final = (1 - t) * a + t * b;
				return final;
			}

			public static float PythagoreanTheorem(float a, float b, float c)
			{ //Leave the one 0 to be calculated
				float final = 0;

				if (c == 0)
				{
					float A = a * a;
					float B = b * b;
					final = Mathf.Sqrt((A + B));
					return final;
				}
				if (a == 0)
				{
					float B = b * b;
					float C = c * c;
					final = Mathf.Sqrt((C - B));
					return final;
				}
				if (b == 0)
				{
					float A = a * a;
					float C = c * c;
					final = Mathf.Sqrt((C - A));
					return final;
				}
				return 0;
			}

			public static float AreaOfRectangle(float l, float w)
			{
				float final = l * w;
				return final;
			}

			public static float AreaOfTrapezoid(float a, float b, float h)
			{
				float final = ((a + b) / 2) * h;
				return final;
			}

			public static float AreaOfCircle(float r)
			{
				float final = Constants.PI * (r * r);
				return final;
			}

			public static float AreaOfTriangle(float b, float h)
			{
				float final = (1 / 2) * b * h;
				return final;
			}

			public static float VolumeOfRectangularprism(float l, float w, float h)
			{
				return l * w * h;
			}

			public static float VolumeOfSphere(float r)
			{
				return (4 / 3) * Constants.PI * (r * r);
			}

			public static float VolumeOfCone(float BaseArea, float h)
			{
				return (1 / 3) * BaseArea * h;
			}

			public static float VolumeOfCube(float l)
			{
				return Mathf.Pow(l, 3);
			}

			public static float VolumeOfCylinder(float BaseArea, float h)
			{
				return BaseArea * h;
			}

			public static float PerimeterOfSquare(float s)
			{
				return 4 * s;
			}

			public static float PerimeterOfRectangle(float l, float w)
			{
				return (2 * w) + (2 * l);
			}

			public static float PerimeterOfTriangle(float a, float b, float c)
			{
				float final = a + b + c;
				return final;
			}

			public static int[] SortIntArray(int[] Array)
			{
				int[] final = Array;
				for (int b = 0; b < final.Length; b++)
				{
					for (int i = 0; i < final.Length - 1; i++)
					{
						if (final[i] > final[i + 1])
						{
							int a = final[i];
							final[i] = final[i + 1];
							final[i + 1] = a;
						}
					}
				}
				return final;
			}

			public static float[] SortFloatArray(float[] Array)
			{
				float[] final = Array;
				for (int b = 0; b < final.Length; b++)
				{
					for (int i = 0; i < final.Length - 1; i++)
					{
						if (final[i] > final[i + 1])
						{
							float a = final[i];
							final[i] = final[i + 1];
							final[i + 1] = a;
						}
					}
				}
				return final;
			}

			public static int[] SortIntArrayReverse(int[] Array)
			{
				int[] final = Array;
				for (int b = 0; b < final.Length; b++)
				{
					for (int i = 0; i < final.Length - 1; i++)
					{
						if (final[i] < final[i + 1])
						{
							int a = final[i];
							final[i] = final[i + 1];
							final[i + 1] = a;
						}
					}
				}
				return final;
			}

			public static float[] SortFloatArrayReverse(float[] Array)
			{
				float[] final = Array;
				for (int b = 0; b < final.Length; b++)
				{
					for (int i = 0; i < final.Length - 1; i++)
					{
						if (final[i] < final[i + 1])
						{
							float a = final[i];
							final[i] = final[i + 1];
							final[i + 1] = a;
						}
					}
				}
				return final;
			}

			public static string[] SortStringArrayByLength(string[] String)
			{
				string[] final = String;
				for (int b = 0; b < String.Length; b++)
				{
					for (int i = 0; i < String.Length - 1; i++)
					{
						if (final[i].Length > final[i + 1].Length)
						{
							string a = final[i];
							final[i] = final[i + 1];
							final[i + 1] = a;
						}
					}
				}
				return final;
			}

			public static string[] SortStringArrayByLengthReverse(string[] String)
			{
				string[] final = String;
				for (int b = 0; b < String.Length; b++)
				{
					for (int i = 0; i < String.Length - 1; i++)
					{
						if (final[i].Length < final[i + 1].Length)
						{
							string a = final[i];
							final[i] = final[i + 1];
							final[i + 1] = a;
						}
					}
				}
				return final;
			}

			public static Vector3[] appendVector3Arrays(Vector3[] a, Vector3[] b)
			{
				Vector3[] final = new Vector3[a.Length + b.Length + 1];

				for (int i = 0; i < a.Length; i++)
				{
					final[i] = a[i];
				}

				for (int i = 0; i < b.Length; i++)
				{
					final[i + a.Length] = b[i];
				}

				return final;
			}

			public static Vector2[] appendVector2Arrays(Vector2[] a, Vector2[] b)
			{
				Vector2[] final = new Vector2[a.Length + b.Length + 1];

				for (int i = 0; i < a.Length; i++)
				{
					final[i] = a[i];
				}

				for (int i = 0; i < b.Length; i++)
				{
					final[i + a.Length] = b[i];
				}

				return final;
			}

			public static Vector3[] CreateCatmullSpline(Vector3[] points, int resolution, bool loop)
			{
				Vector3[] final = new Vector3[resolution * points.Length + 1];

				for (int i = 0; i < points.Length; i++)
				{
					if ((i == 0 || i == points.Length - 2 || i == points.Length - 1) && !loop)
					{
						continue;
					}
					Vector3[] a = DisplayCatmullRomSpline(i, points, resolution);
					final = processVector3array(final, a, Vector3.zero);
				}
				return final;
			}
			public static Vector3[] DisplayCatmullRomSpline(int pos, Vector3[] points, int res)
			{
				Vector3[] outPos = new Vector3[res + 5];

				Vector3 p0 = points[ClampListPos(pos - 1, points)];
				Vector3 p1 = points[pos];
				Vector3 p2 = points[ClampListPos(pos + 1, points)];
				Vector3 p3 = points[ClampListPos(pos + 2, points)];

				Vector3 lastPos = p1;
				p1 = lastPos;


				float resolution = (float)1 / res;

				int loops = Mathf.FloorToInt(1f / resolution);

				for (int i = 1; i <= loops; i++)
				{
					float t = i * resolution;
					Vector3 newPos = GetCatmullRomPosition(t, p0, p1, p2, p3);
					outPos[i] = newPos;
					lastPos = newPos;
				}
				return outPos;
			}
			public static int ClampListPos(int pos, Vector3[] points)
			{
				if (pos < 0)
				{
					pos = points.Length - 1;
				}
				if (pos > points.Length)
				{
					pos = 1;
				}
				else if (pos > points.Length - 1)
				{
					pos = 0;
				}
				return pos;
			}
			public static Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
			{
				Vector3 a = 2f * p1;
				Vector3 b = p2 - p0;
				Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
				Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;
				Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));
				return pos;
			}
			public static Vector3[] processVector3array(Vector3[] a, Vector3[] b, Vector3 cleanThis)
			{
				Vector3[] start = new Vector3[a.LongLength + b.LongLength];

				for (int i = 0; i < a.Length; i++)
				{
					start[i] = a[i];
				}
				for (int i = 0; i < b.Length; i++)
				{
					start[i + a.Length] = b[i];
				}

				Vector3[] removal = new Vector3[start.Length];
				int count = 0;

				for (int i = 0; i < start.Length; i++)
				{
					if (start[i] != cleanThis)
					{
						removal[count] = start[i];
						count++;
					}
				}
				Vector3[] output = new Vector3[count];
				for (int i = 0; i < count; i++)
				{
					output[i] = removal[i];
				}
				return output;
			}


		}

	}
}

