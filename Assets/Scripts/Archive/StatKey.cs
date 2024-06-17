/*
using UnityEngine;

[System.Serializable]
public class StatKey<T1, T2, T3> : System.Tuple<T1, T2, T3>
{
	[SerializeField]
	private T1 value1;

	[SerializeField]
	private T2 value2;

	[SerializeField]
	private T3 value3;

	public StatKey(T1 item1, T2 item2, T3 item3) : base(item1, item2, item3)
	{
		value1 = item1;
		value2 = item2;
		value3 = item3;
	}

	public new T1 Item1 { get => value1; }
	public new T2 Item2 { get => value2; }
	public new T3 Item3 { get => value3; }
}
//*/