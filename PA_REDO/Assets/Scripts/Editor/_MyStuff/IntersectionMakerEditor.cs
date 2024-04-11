using UnityEditor;
using UnityEngine;

namespace Editor._MyStuff
{
	[CustomEditor(typeof(IntersectionMaker))]
	public class IntersectionMakerEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			IntersectionMaker intersectionMaker = (IntersectionMaker) target;
			
			if (GUILayout.Button("Load Saved Intersections")) 
				intersectionMaker.Load();

			if (GUILayout.Button("Generate")) 
				intersectionMaker.Generate();
			
			if (GUILayout.Button("Clear"))
				intersectionMaker.Clear();

			base.OnInspectorGUI();
		}
	}
}