using UnityEditor;
using UnityEngine;

namespace Editor
{
	[CustomEditor(typeof(PlaneGenerator))]
	public class PlaneGeneratorEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			PlaneGenerator planeGenerator = (PlaneGenerator) target;

			if (GUILayout.Button("Generate")) 
				planeGenerator.Generate();
			
			base.OnInspectorGUI();
		}
	}
}