using _Scripts._MyStuff;
using OldStuff._MyStuff;
using UnityEditor;
using UnityEngine;

namespace Editor._MyStuff
{
	[CustomEditor(typeof(NeighbourhoodBuilder))]
	public class NeighbourhoodBuilderEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			NeighbourhoodBuilder neighbourhoodBuilder = (NeighbourhoodBuilder) target;
			
			if (GUILayout.Button("Load Saved Intersections")) 
				neighbourhoodBuilder.Load();

			if (GUILayout.Button("Generate")) 
				neighbourhoodBuilder.Generate();

			if (GUILayout.Button("Clear")) 
				neighbourhoodBuilder.Clear();

			base.OnInspectorGUI();
		}
	}
}