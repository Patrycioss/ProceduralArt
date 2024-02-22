using UnityEditor;
using UnityEngine;

namespace Editor
{
	[CustomEditor(typeof(TerrainGenerator))]
	public class TerrainGeneratorEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			TerrainGenerator terrainGenerator = (TerrainGenerator) target;

			if (GUILayout.Button("Generate")) 
				terrainGenerator.Generate();
			
			if (GUILayout.Button("Clear"))
				terrainGenerator.Clear();
			
			base.OnInspectorGUI();
		}
	}
}