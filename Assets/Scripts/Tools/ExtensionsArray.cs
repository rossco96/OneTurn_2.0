public static class ExtensionsArray
{
	public static bool Contains<T>(this T[] array, T item)
	{
		if (array == null || array.Length == 0)	return false;
		
		for (int i = 0; i < array.Length; ++i)
		{
			if (array[i].Equals(item))			return true;
		}

		return false;
	}

	public static T[] Add<T>(this T[] array, T item)
	{
		if (array.Length == 0) return new T[] { item };

		T[] newArray = new T[array.Length + 1];
		for (int i = 0; i < array.Length; ++i)
		{
			newArray[i] = array[i];
		}
		newArray[array.Length - 1] = item;
		
		return newArray;
	}
}
