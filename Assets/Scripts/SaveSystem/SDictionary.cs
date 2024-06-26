using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SDictionary<k, v> : Dictionary<k, v>, ISerializationCallbackReceiver
{
	[SerializeField]
	private List<k> m_keys = new List<k>();

	[SerializeField]
	private List<v> m_values = new List<v>();

	public void OnBeforeSerialize()
	{
		m_keys.Clear();
		m_values.Clear();
		using Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<k, v> current = enumerator.Current;
			m_keys.Add(current.Key);
			m_values.Add(current.Value);
		}
	}

	public void OnAfterDeserialize()
	{
		Clear();
		for (int i = 0; i < m_keys.Count; i++)
		{
			Add(m_keys[i], m_values[i]);
		}
		m_keys.Clear();
		m_values.Clear();
	}

	public k GetKeyAtIndex(int i)
	{
		if (i > Keys.Count)
			return default;

		int count = 0;
		var enumerator = Keys.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (count == i)
				return enumerator.Current;
			count++;
		}

		return default;
	}

	public v GetValueAtIndex(int i)
	{
		if (i > Values.Count)
			return default;

		int count = 0;
		var enumerator = Values.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (count == i)
				return enumerator.Current;
			count++;
		}

		return default;
	}
}
