using System;
using UnityEngine;

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
		T[] newArray = new T[array.Length + 1];

		for (int i = 0; i < array.Length; ++i)
		{
			newArray[i] = array[i];
		}
		newArray[array.Length] = item;
		
		return newArray;
	}

	public static T[] RemoveAt<T>(this T[] array, int index)
	{
		T[] newArray = new T[array.Length - 1];

		bool indexReached = false;
		for (int i = 0; i < newArray.Length; ++i)
		{
			if (i == index) indexReached = true;
			int oldIndex = (indexReached) ? i + 1 : i;
			newArray[i] = array[oldIndex];
		}

		return newArray;
	}

	public static int[] RetrieveListOrder(this long[] array)
	{
		int[] listOrder = new int[0];
		int recentIndex = 0;
		long previousRecentTime = long.MaxValue;

		for (int i = 0; i < array.Length; ++i)
		{
			long recentTime = 0;
			for (int j = 0; j < array.Length; ++j)
			{
				if (array[j] < previousRecentTime && array[j] > recentTime)
				{
					recentTime = array[j];
					recentIndex = j;
				}
			}
			previousRecentTime = recentTime;
			listOrder = listOrder.Add(recentIndex);
		}

		return listOrder;
	}
}
