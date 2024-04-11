using UnityEngine;

namespace _Scripts._MyStuff
{
	public class RNG : MonoBehaviour
	{
		[SerializeField] private SaveData _saveData;
		
		[SerializeField] private int _seed;
		
		private System.Random _rand;

		private void OnValidate()
		{
			UpdateRandToSeed();
		}

		public void SaveSeed()
		{
			_saveData.data.seed = _seed;
		}

		public void LoadSeed()
		{
			_seed = _saveData.data.seed;
		}

		/// <summary>
		/// Random number between pMin (inclusive) and pMax (inclusive)
		/// </summary>
		public float Float(float pMin, float pMax)
		{
			double rDouble = _rand.NextDouble();
			double rRangeDouble = rDouble * (pMax - pMin) + pMin;
			return (float) rRangeDouble;
		}
		
		public int Int(int pMin, int pMax)
		{
			return _rand.Next(pMin, pMax + 1);
		}

		public void UpdateRandToSeed()
		{
			_rand = new System.Random(_seed);
		}

		public void SetRandomSeed()
		{
			_seed = new System.Random().Next();
		}
	}
}