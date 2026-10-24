using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000221 RID: 545
public static class RandomElement
{
	// Token: 0x06000BEC RID: 3052 RVA: 0x0002EEA0 File Offset: 0x0002D0A0
	public static T RandomItem<T>(this T[] array)
	{
		return array[UnityEngine.Random.Range(0, array.Length)];
	}

	// Token: 0x06000BED RID: 3053 RVA: 0x0002EEB1 File Offset: 0x0002D0B1
	public static T RandomItem<T>(this List<T> list)
	{
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	// Token: 0x06000BEE RID: 3054 RVA: 0x0002EEC5 File Offset: 0x0002D0C5
	public static bool TryGetRandomItem<T>(this List<T> list, out T random)
	{
		if (list.Count > 0)
		{
			random = list.RandomItem<T>();
			return true;
		}
		random = default(T);
		return false;
	}

	// Token: 0x06000BEF RID: 3055 RVA: 0x0002EEE8 File Offset: 0x0002D0E8
	public static T PullRandomItem<T>(this List<T> list)
	{
		int index = UnityEngine.Random.Range(0, list.Count);
		T result = list[index];
		list.RemoveAt(index);
		return result;
	}
}
