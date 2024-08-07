using UnityEngine;

public static class ExtensionsVector
{
    public static float GetMagnitude2D(this Vector3 a)
	{
		Vector2 a2 = new Vector2(a.x, a.y);
		return a2.magnitude;
	}
}
