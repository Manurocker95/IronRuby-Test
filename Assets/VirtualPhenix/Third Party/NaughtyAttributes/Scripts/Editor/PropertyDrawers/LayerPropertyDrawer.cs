using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace NaughtyAttributes.Editor
{
	[CustomPropertyDrawer(typeof(LayerAttribute))]
	public class LayerPropertyDrawer : PropertyDrawerBase
	{
		protected override float GetPropertyHeight_Internal(SerializedProperty property, GUIContent label)
		{
			return (property.propertyType == SerializedPropertyType.String)
				? GetPropertyHeight(property)
				: GetPropertyHeight(property) + GetHelpBoxHeight();
		}

		protected override void OnGUI_Internal(Rect rect, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType == SerializedPropertyType.String)
			{
				// generate the taglist + custom tags
				List<string> layerList = new List<string>();
				layerList.AddRange(UnityEditorInternal.InternalEditorUtility.layers);

				string propertyString = property.stringValue;
				int index = 0;
				// check if there is an entry that matches the entry and get the index
				// we skip index 0 as that is a special custom case
				for (int i = 0; i < layerList.Count; i++)
				{
					if (layerList[i] == propertyString)
					{
						index = i;
						break;
					}
				}

				// Draw the popup box with the current selected index
				index = EditorGUI.Popup(rect, label.text, index, layerList.ToArray());

				// Adjust the actual string value of the property based on the selection
				if (index > 0)
				{
					property.stringValue = layerList[index];
				}
				else
				{
					property.stringValue = string.Empty;
				}
			}
			else if (property.propertyType == SerializedPropertyType.Integer)
			{
				// generate the taglist + custom tags
				Dictionary<int,string> layers = new Dictionary<int, string>();
				var list = UnityEditorInternal.InternalEditorUtility.layers;
				
				for (int i = 0; i < list.Length; i++)
				{
					layers.Add(i, list[i]);
				}
		

				int propertyString = property.intValue;
				int index = 0;
				// check if there is an entry that matches the entry and get the index
				// we skip index 0 as that is a special custom case
				for (int i = 0; i < layers.Keys.Count; i++)
				{
					if (layers.Keys.ElementAt(i) == propertyString)
					{
						index = i;
						break;
					}
				}

				// Draw the popup box with the current selected index
				index = EditorGUI.Popup(rect, label.text, index, list);

				// Adjust the actual string value of the property based on the selection
				if (index > 0)
				{
					property.intValue = layers.Keys.ElementAt(index);
				}
				else
				{
					property.intValue = 0;
				}
			}
			else
			{
				string message = string.Format("{0} supports only string and int fields", typeof(LayerAttribute).Name);
				DrawDefaultPropertyAndHelpBox(rect, property, message, MessageType.Warning);
			}
		}
	}
}
