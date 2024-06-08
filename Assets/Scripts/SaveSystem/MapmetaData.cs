using System.Collections.Generic;
using UnityEngine;

// [TODO] Please try and make this generic!! Feels too hacky to have two types of StatsData

[System.Serializable]
public class MapmetaData<k, v> : Dictionary<k, v>, ISerializationCallbackReceiver
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
}
