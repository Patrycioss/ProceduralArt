using UnityEditor;
using UnityEngine;

namespace Editor
{
	[CustomEditor(typeof(RNG)), CanEditMultipleObjects]
	public class RNGEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			RNG rng = (RNG) serializedObject.targetObject;

			if (GUILayout.Button("Random Seed")) 
				rng.SetRandomSeed();

			base.OnInspectorGUI();
		}
	}
}