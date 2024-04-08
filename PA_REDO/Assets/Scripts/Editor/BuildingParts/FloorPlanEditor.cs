using BuildingParts;
using UnityEditor;
using UnityEngine;

namespace Editor.BuildingParts
{
	[CustomPropertyDrawer(typeof(FloorPlan))]
	public class FloorPlanEditor : PropertyDrawer
	{
		private int currentSize;
		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			SerializedProperty sizeProperty = property.FindPropertyRelative("Size");
			SerializedProperty planProperty = property.FindPropertyRelative("Plan");
			
			EditorGUI.PropertyField(position, property, label, true);

			if (GUILayout.Button("Reset Floor Plan"))
			{
				for (int i = 0; i < planProperty.arraySize; i++)
				{
					SerializedProperty dataProperty = planProperty.GetArrayElementAtIndex(i).FindPropertyRelative("Data");
					
					for (int j = 0; j < dataProperty.arraySize; j++)
					{
						dataProperty.GetArrayElementAtIndex(j).intValue = 0;
					}
				}
			}

			if (currentSize != sizeProperty.intValue)
			{
				currentSize = sizeProperty.intValue;
			
				planProperty.arraySize = currentSize;

				for (int i = 0; i < planProperty.arraySize; i++)
				{
					SerializedProperty dataProperty = planProperty.GetArrayElementAtIndex(i).FindPropertyRelative("Data");
					dataProperty.arraySize = currentSize;
				}
			}
		}
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			return EditorGUI.GetPropertyHeight(property);
		}

	}
}