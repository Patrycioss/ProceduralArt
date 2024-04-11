using _Scripts._MyStuff;
using UnityEditor;
using UnityEngine;

namespace Editor._MyStuff
{
	[CustomEditor(typeof(SaveData)), CanEditMultipleObjects]
	public class SaveDataEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			SaveData saveData = (SaveData) serializedObject.targetObject;

			if (GUILayout.Button("Save")) 
				saveData.Save();

			if (GUILayout.Button("Load")) 
				saveData.Load();

			base.OnInspectorGUI();
		}
	}
}