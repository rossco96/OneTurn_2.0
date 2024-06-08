using UnityEngine;

public static class ExtensionsFloat
{
	/// <summary>
	/// IMPORTANT -- This does not fully work!!
	/// Will occasionally return e.g. 1.2300001
	/// </summary>
	public static float RoundDP(this float n, int decimalPlaces)
	{
		float y = n * Mathf.Pow(10, decimalPlaces);
		y = Mathf.Round(y);
		y /= Mathf.Pow(10, decimalPlaces);
		return y;
	}
}
