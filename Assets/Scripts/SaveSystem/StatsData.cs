using System.Collections.Generic;
using UnityEngine;

// [TODO] Please try and make this generic!! Feels too hacky to have two types of StatsData

[System.Serializable]
public class StatsData<k, v> : Dictionary<k, v>, ISerializationCallbackReceiver where k : StatKey<string, string, string>
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

	public v GetValue(k key)
	{
		for (int i = 0; i < m_keys.Count; i++)
		{
			if (m_keys[i].Item1 == key.Item1 && m_keys[i].Item2 == key.Item2 && m_keys[i].Item3 == key.Item3)
				return m_values[i];
		}
		return default;
	}
}
