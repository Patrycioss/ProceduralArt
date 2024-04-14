using UnityEditor;
using UnityEngine;

namespace Editor
{
	[CustomEditor(typeof(CityGenerator))]
	public class CityGeneratorEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			CityGenerator cityGenerator = (CityGenerator) target;

			if (GUILayout.Button("Full Generate")) 
				cityGenerator.FullGenerate();
			
			if (GUILayout.Button("Full Clear"))
				cityGenerator.FullClear();
			
			base.OnInspectorGUI();
		}
	}
}