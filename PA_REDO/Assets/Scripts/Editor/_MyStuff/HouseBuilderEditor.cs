using _Scripts.OldStuff._MyStuff;
using UnityEditor;
using UnityEngine;

namespace Editor._MyStuff
{
	[CustomEditor(typeof(HouseBuilder))]
	public class HouseBuilderEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			HouseBuilder houseBuilder = (HouseBuilder) target;
			
			if (GUILayout.Button("Build")) 
				houseBuilder.Build();

			base.OnInspectorGUI();
		}
	}
}