using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

public class RNG : MonoBehaviour
{
	[FormerlySerializedAs("_seed")] [SerializeField] private int seed;
		
	private Random rand;
	
	/// <summary>
	/// Random number between pMin (inclusive) and pMax (inclusive)
	/// </summary>
	public float Float(float pMin, float pMax)
	{
		double rDouble = rand.NextDouble();
		double rRangeDouble = rDouble * (pMax - pMin) + pMin;
		return (float) rRangeDouble;
	}
	
	/// <summary>
	/// Random number between two integers, min inclusive and max inclusive.
	/// </summary>
	public int Int(int pMin, int pMax)
	{
		return rand.Next(pMin, pMax + 1);
	}
	public void SetRandomSeed()
	{
		seed = new Random().Next();
		rand = new Random(seed);
	}

	private void OnValidate()
	{
		rand = new Random(seed);
	}
}