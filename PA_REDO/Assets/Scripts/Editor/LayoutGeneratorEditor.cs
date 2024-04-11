using UnityEditor;
using UnityEngine;

namespace Editor
{
	[CustomEditor(typeof(LayoutGenerator))]
	public class LayoutGeneratorEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			LayoutGenerator terrainGenerator = (LayoutGenerator) target;

			if (GUILayout.Button("Generate")) 
				terrainGenerator.Generate();
			
			if (GUILayout.Button("Clear"))
				terrainGenerator.Clear();
			
			base.OnInspectorGUI();
		}
	}
}