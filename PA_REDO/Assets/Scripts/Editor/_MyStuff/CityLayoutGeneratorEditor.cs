using OldStuff._MyStuff;
using UnityEditor;
using UnityEngine;

namespace Editor._MyStuff
{
	[CustomEditor(typeof(CityLayoutGenerator))]
	public class CityLayoutGeneratorEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			CityLayoutGenerator cityLayoutGenerator = (CityLayoutGenerator) target;

			if (GUILayout.Button("Generate")) 
				cityLayoutGenerator.Generate();

			if (GUILayout.Button("Save Intersections")) 
				cityLayoutGenerator.Save();

			if (GUILayout.Button("Load Intersections")) 
				cityLayoutGenerator.Load();

			if (GUILayout.Button("Clear")) 
				cityLayoutGenerator.Clear();

			base.OnInspectorGUI();
		}
	}
}