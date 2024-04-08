using BuildingParts;
using UnityEditor;
using UnityEngine;

namespace Editor.BuildingParts
{
	[CustomPropertyDrawer(typeof(Row))]
	public class RowEditor : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			SerializedProperty dataProperty = property.FindPropertyRelative("Data");

			for (int index = 0; index < dataProperty.arraySize; index++)
			{
				SerializedProperty intProperty = dataProperty.GetArrayElementAtIndex(index);
				Rect rect = new Rect(position.x + index * 20, position.y, 20, 20);
				int value = EditorGUI.IntField(rect, intProperty.intValue);
				
				if (value < 0)
				{
					value = 0;
				}

				intProperty.intValue = value;
			}

			EditorGUI.EndProperty();
		}
	}
}