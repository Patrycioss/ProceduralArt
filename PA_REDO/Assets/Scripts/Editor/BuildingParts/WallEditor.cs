using System.Linq;
using BuildingParts;
using UnityEditor;
using UnityEngine;

namespace Editor.BuildingParts
{
	[CustomPropertyDrawer(typeof(Wall))]
	public class WallEditor : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			Rect labelPosition = new Rect(position);

			WallSection wallSection = (WallSection) property.FindPropertyRelative("WallSection").objectReferenceValue;

			GUIContent newLabel = label;
			newLabel.text = label.text.Split("/").Last();
			position = EditorGUI.PrefixLabel(labelPosition, GUIUtility.GetControlID(FocusType.Passive), newLabel);

			int indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			SerializedProperty typeProperty = property.FindPropertyRelative("Type");
			SerializedProperty objectProperty = property.FindPropertyRelative("Object");

			Rect objectRect = new Rect(position.x, position.y, 170, position.height);
			// Rect buttonRect = new Rect(position.x + 70, position.y, 100, position.height);
			Rect typeRect = new Rect(position.x + 180, position.y, 100, position.height);

			// typeProperty.intValue = EditorGUI.IntField(typeRect, typeProperty.intValue);
			EditorGUI.ObjectField(objectRect, objectProperty.objectReferenceValue,
				typeof(GameObject), false);

			string[] options = wallSection.GetWallOptions();
			
			
			// int typeIndex = options.ToList().IndexOf(typeProperty.stringValue);

			// typeProperty.stringValue = options[EditorGUILayout.Popup(new GUIContent("Wall Options"), typeIndex, options)];

			GUIContent dropdownLabel = label;
			label.text = typeProperty.stringValue.Split("/".ToCharArray()).Last();
			
			if (EditorGUI.DropdownButton(typeRect, dropdownLabel, FocusType.Passive))
			{
				GenericMenu menu = new GenericMenu();
			
				foreach (string option in options)
				{
					string name = option.Split("/".ToCharArray()).Last();
					Debug.Log($"{name}");
					menu.AddItem(new GUIContent(name), false, _ =>
						{
							typeProperty.stringValue = option;
							wallSection.SetWall((Wall) property.boxedValue, typeProperty.stringValue);
						},
						option);
				}
				
				menu.DropDown(typeRect);
			}


			// if (GUI.Button(buttonRect, "Regen"))
			// {
			// 	wallSection.SetWall((Wall) property.boxedValue, typeProperty.stringValue);
			// }

			

			EditorGUI.indentLevel = indent;
			EditorGUI.EndProperty();
		}
	}
}