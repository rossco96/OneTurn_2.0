/*
using System;
using UnityEngine;

[Serializable]
public class SerializableTuple<T1, T2, T3> : Tuple<T1, T2, T3>
//public class SerializableTuple<T1, T2, T3, T4> : Tuple<T1, T2, T3, T4>
{
	[SerializeField]
	private T1 value1;

	[SerializeField]
	private T2 value2;

	[SerializeField]
	private T3 value3;

	//[SerializeField]
	//private T4 value4;

	public SerializableTuple(T1 item1, T2 item2, T3 item3) : base(item1, item2, item3)
	//public SerializableTuple(T1 item1, T2 item2, T3 item3, T4 item4) : base(item1, item2, item3, item4)
	{
		value1 = item1;
		value2 = item2;
		value3 = item3;
		//value4 = item4;
	}

	public new T1 Item1 { get => value1; }
	public new T2 Item2 { get => value2; }
	public new T3 Item3 { get => value3; }
	//public new T4 Item4 { get => value4; }
}
//*/