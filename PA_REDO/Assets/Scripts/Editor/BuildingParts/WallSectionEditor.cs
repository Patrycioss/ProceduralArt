using BuildingParts;
using UnityEditor;
using UnityEngine;

namespace Editor.BuildingParts
{
	[CustomEditor(typeof(WallSection))]
	public class WallSectionEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			WallSection wallSection = (WallSection) target;
			
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("WallPart Count: ");
			wallSection.WallPartCount = EditorGUILayout.IntField(wallSection.WallPartCount);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Height: ");
			wallSection.Height = EditorGUILayout.IntField(wallSection.Height);
			EditorGUILayout.EndHorizontal();
			
			// EditorGUILayout.BeginHorizontal();
			// GUILayout.Label("Skip: ");
			// wallSection.Skip = EditorGUILayout.IntField(wallSection.Skip);
			// EditorGUILayout.EndHorizontal();
			

			if (GUILayout.Button("Regenerate")) 
				wallSection.Generate(wallSection.WallPartCount,  wallSection.Height, wallSection.Skip);
			
			if (GUILayout.Button("Clear"))
				wallSection.Clear();
			
			base.OnInspectorGUI();
		}
	}
}