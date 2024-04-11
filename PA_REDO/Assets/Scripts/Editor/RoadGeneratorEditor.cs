using UnityEditor;
using UnityEngine;

namespace Editor
{
	[CustomEditor(typeof(RoadGenerator))]
	public class RoadGeneratorEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			RoadGenerator roadGenerator = (RoadGenerator) target;

			if (GUILayout.Button("Generate")) 
				roadGenerator.Generate();
			
			if (GUILayout.Button("Clear"))
				roadGenerator.Clear();
			
			base.OnInspectorGUI();
		}
	}
}