using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using _Scripts._MyStuff.CityLayout;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts._MyStuff
{
	[Serializable]
	public class Data
	{
		public List<Node> intersections;
		public int seed;
	}

	[Serializable]
	[CreateAssetMenu(fileName = "SaveData", menuName = "SaveData", order = 0)]
	public class SaveData : ScriptableObject
	{
		public Data data = new();

		public void Save()
		{
			BinaryFormatter bf = new BinaryFormatter(); 
			FileStream file = File.Create(Application.persistentDataPath 
			                              + "/SaveData.dat");
			bf.Serialize(file, data);
			file.Close();
			Debug.Log("Game data saved!");
		}

		public void Load()
		{
			if (File.Exists(Application.persistentDataPath 
			                + "/SaveData.dat"))
			{
				BinaryFormatter bf = new();
				FileStream file = 
					File.Open(Application.persistentDataPath 
					          + "/SaveData.dat", FileMode.Open);
				Data data = (Data)bf.Deserialize(file);
				file.Close();
				this.data = data;
				Debug.Log("Game data loaded!");
			}
			else
				Debug.LogError("There is no save data!");
		}
	}
}