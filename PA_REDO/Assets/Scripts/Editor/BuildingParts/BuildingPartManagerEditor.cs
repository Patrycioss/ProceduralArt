using BuildingParts;
using UnityEditor;
using UnityEngine;

namespace Editor.BuildingParts
{
	[CustomEditor(typeof(BuildingPartManager))]
	public class BuildingPartMangerEditor : UnityEditor.Editor
	{
		[SerializeField] private int a;
		public override void OnInspectorGUI()
		{
			BuildingPartManager buildingPartManager = (BuildingPartManager) target;
			//
			// if (GUILayout.Button("Fetch Parts"))
			// 	buildingPartManager.Fetch();
			//
			// if (GUILayout.Button("Clear Parts"))
			// 	buildingPartManager.Clear();
			
			base.OnInspectorGUI();
		}
	}
}