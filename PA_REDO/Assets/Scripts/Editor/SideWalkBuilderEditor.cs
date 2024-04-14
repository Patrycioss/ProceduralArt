using UnityEditor;
using UnityEngine;

namespace Editor
{
	[CustomEditor(typeof(SideWalkBuilder))]
	public class SideWalkBuilderEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			SideWalkBuilder sideWalkBuilder = (SideWalkBuilder) target;
			
			if (GUILayout.Button("Build SideWalk")) 
				sideWalkBuilder.Build();
			
			base.OnInspectorGUI();
		}
	}
}