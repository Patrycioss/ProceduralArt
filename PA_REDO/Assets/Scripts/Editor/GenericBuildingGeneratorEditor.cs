using UnityEditor;
using UnityEngine;

namespace Editor
{
	[CustomEditor(typeof(GenericBuildingGenerator))]
	public class GenericBuildingGeneratorEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			GenericBuildingGenerator terrainGenerator = (GenericBuildingGenerator) target;

			if (GUILayout.Button("Generate")) 
				terrainGenerator.Generate();
			
			if (GUILayout.Button("Clear"))
				terrainGenerator.Clear();
			
			base.OnInspectorGUI();
		}
	}
}