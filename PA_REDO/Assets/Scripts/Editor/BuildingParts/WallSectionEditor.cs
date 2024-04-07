using BuildingParts;
using UnityEditor;
using UnityEngine;

namespace Editor.BuildingParts
{
	[CustomEditor(typeof(WallSection))]
	public class WallSectionEditor : UnityEditor.Editor
	{
		[SerializeField] private int a;
		public override void OnInspectorGUI()
		{
			WallSection wallSection = (WallSection) target;


			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("WallPart Count: ");
			wallSection.WallPartCount = EditorGUILayout.IntField(wallSection.WallPartCount);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("DecoPart Count: ");
			wallSection.DecoPartCount = EditorGUILayout.IntField(wallSection.DecoPartCount);
			EditorGUILayout.EndHorizontal();

			if (GUILayout.Button("Regenerate")) 
				wallSection.Generate(wallSection.WallPartCount, wallSection.DecoPartCount);
			
			if (GUILayout.Button("Clear"))
				wallSection.Clear();
			
			base.OnInspectorGUI();
		}
	}
}