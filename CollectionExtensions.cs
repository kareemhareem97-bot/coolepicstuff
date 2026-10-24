using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Mirror;
using NorthwoodLib.Pools;
using UnityEngine;

// Token: 0x020001D8 RID: 472
public static class CollectionExtensions
{
	// Token: 0x060009B8 RID: 2488 RVA: 0x00027044 File Offset: 0x00025244
	public static void ShuffleQueue<T>(this Queue<T> queue, System.Random rng = null)
	{
		if (rng == null)
		{
			rng = new System.Random();
		}
		List<T> list = ListPool<T>.Shared.Rent(queue.Count);
		while (queue.Count > 0)
		{
			list.Add(queue.Dequeue());
		}
		queue.Clear();
		while (list.Count > 0)
		{
			int index = rng.Next(list.Count);
			queue.Enqueue(list[index]);
			list.RemoveAt(index);
		}
		ListPool<T>.Shared.Return(list);
	}

	// Token: 0x060009B9 RID: 2489 RVA: 0x000270C0 File Offset: 0x000252C0
	public static void ShuffleList<T>(this IList<T> list, System.Random rng = null)
	{
		if (rng == null)
		{
			rng = new System.Random();
		}
		int i = list.Count;
		while (i > 1)
		{
			i--;
			int index = rng.Next(i + 1);
			T value = list[index];
			list[index] = list[i];
			list[i] = value;
		}
	}

	// Token: 0x060009BA RID: 2490 RVA: 0x00027110 File Offset: 0x00025310
	public static void ShuffleListSecure<T>(this IList<T> list)
	{
		using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
		{
			int i = list.Count;
			while (i > 1)
			{
				byte[] array = new byte[1];
				do
				{
					rngcryptoServiceProvider.GetBytes(array);
				}
				while ((int)array[0] >= i * (255 / i));
				int index = (int)array[0] % i;
				i--;
				T value = list[index];
				list[index] = list[i];
				list[i] = value;
			}
		}
	}

	// Token: 0x060009BB RID: 2491 RVA: 0x00027194 File Offset: 0x00025394
	public static TReturn[] OfType<TOriginal, TReturn>(this TOriginal[] original)
	{
		List<TReturn> list = ListPool<TReturn>.Shared.Rent();
		foreach (TOriginal toriginal in original)
		{
			if (toriginal is TReturn)
			{
				TReturn item = toriginal as TReturn;
				list.Add(item);
			}
		}
		TReturn[] result = list.ToArray();
		ListPool<TReturn>.Shared.Return(list);
		return result;
	}

	// Token: 0x060009BC RID: 2492 RVA: 0x000271F8 File Offset: 0x000253F8
	public static bool IsEmpty(this Array array)
	{
		return array.Length == 0;
	}

	// Token: 0x060009BD RID: 2493 RVA: 0x00027203 File Offset: 0x00025403
	public static bool IsEmpty<T>(this T[] array)
	{
		return array.Length == 0;
	}

	// Token: 0x060009BE RID: 2494 RVA: 0x0002720A File Offset: 0x0002540A
	public static bool IsEmpty<T>(this ArraySegment<T> array)
	{
		return array.Count == 0;
	}

	// Token: 0x060009BF RID: 2495 RVA: 0x00027216 File Offset: 0x00025416
	public static bool IsEmpty<T>(this List<T> list)
	{
		return list.Count == 0;
	}

	// Token: 0x060009C0 RID: 2496 RVA: 0x00027221 File Offset: 0x00025421
	public static bool IsEmpty<T>(this Queue<T> queue)
	{
		return queue.Count == 0;
	}

	// Token: 0x060009C1 RID: 2497 RVA: 0x0002722C File Offset: 0x0002542C
	public static bool IsEmpty<T>(this Stack<T> stack)
	{
		return stack.Count == 0;
	}

	// Token: 0x060009C2 RID: 2498 RVA: 0x00027237 File Offset: 0x00025437
	public static bool IsEmpty<T>(this HashSet<T> set)
	{
		return set.Count == 0;
	}

	// Token: 0x060009C3 RID: 2499 RVA: 0x00027242 File Offset: 0x00025442
	public static bool IsEmpty<T>(this SortedSet<T> set)
	{
		return set.Count == 0;
	}

	// Token: 0x060009C4 RID: 2500 RVA: 0x0002724D File Offset: 0x0002544D
	public static bool IsEmpty<T>(this SyncList<T> list)
	{
		return list.Count == 0;
	}

	// Token: 0x060009C5 RID: 2501 RVA: 0x00027258 File Offset: 0x00025458
	public static bool IsEmpty<T>(this SyncSet<T> set)
	{
		return set.Count == 0;
	}

	// Token: 0x060009C6 RID: 2502 RVA: 0x00027263 File Offset: 0x00025463
	public static bool IsEmpty<TKey, TValue>(this SyncDictionary<TKey, TValue> dictionary)
	{
		return dictionary.Count == 0;
	}

	// Token: 0x060009C7 RID: 2503 RVA: 0x0002726E File Offset: 0x0002546E
	public static bool IsEmpty<T>(this ICollection<T> collection)
	{
		return collection.Count == 0;
	}

	// Token: 0x060009C8 RID: 2504 RVA: 0x00027279 File Offset: 0x00025479
	public static bool IsEmpty<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
	{
		return dictionary.Count == 0;
	}

	// Token: 0x060009C9 RID: 2505 RVA: 0x00027284 File Offset: 0x00025484
	public static bool IsEmpty<T>(this IEnumerable<T> iEnumerable)
	{
		return !iEnumerable.Any<T>();
	}

	// Token: 0x060009CA RID: 2506 RVA: 0x0002728F File Offset: 0x0002548F
	public static void EnsureCapacity<T>(this List<T> list, int capacity)
	{
		if (list.Capacity < capacity)
		{
			list.Capacity = capacity;
		}
	}

	// Token: 0x060009CB RID: 2507 RVA: 0x000272A4 File Offset: 0x000254A4
	public static int IndexOf<T>(this T[] array, T obj)
	{
		for (int i = 0; i < array.Length; i++)
		{
			if (EqualityComparer<T>.Default.Equals(array[i], obj))
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060009CC RID: 2508 RVA: 0x000272D8 File Offset: 0x000254D8
	public static int LastIndexOf<T>(this T[] array, T obj)
	{
		for (int i = array.Length - 1; i >= 0; i--)
		{
			if (EqualityComparer<T>.Default.Equals(array[i], obj))
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060009CD RID: 2509 RVA: 0x0002730C File Offset: 0x0002550C
	public static bool Contains<T>(this T[] array, T obj)
	{
		for (int i = 0; i < array.Length; i++)
		{
			if (EqualityComparer<T>.Default.Equals(array[i], obj))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060009CE RID: 2510 RVA: 0x00027340 File Offset: 0x00025540
	public static void ForEach<T>(this T[] array, Action<T> obj)
	{
		for (int i = 0; i < array.Length; i++)
		{
			if (obj != null)
			{
				obj(array[i]);
			}
		}
	}

	// Token: 0x060009CF RID: 2511 RVA: 0x0002736C File Offset: 0x0002556C
	public static void Reverse<T>(this T[] array)
	{
		int i = 0;
		int num = array.Length;
		while (i < num)
		{
			T t = array[i];
			array[i] = array[num];
			array[num] = t;
			i++;
			num--;
		}
	}

	// Token: 0x060009D0 RID: 2512 RVA: 0x000273AC File Offset: 0x000255AC
	public static bool CheckIdentical<T>(ICollection<T> left, ICollection<T> right, Func<T, T, bool> isEqual)
	{
		bool flag = left == null;
		bool flag2 = right == null;
		if (flag != flag2)
		{
			return false;
		}
		if (flag && flag2)
		{
			return true;
		}
		int count = left.Count;
		int count2 = right.Count;
		if (count != count2)
		{
			return false;
		}
		for (int i = 0; i < count; i++)
		{
			if (!isEqual(left.ElementAt(i), right.ElementAt(i)))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060009D1 RID: 2513 RVA: 0x00027410 File Offset: 0x00025610
	public static bool Contains(this string[] array, string str, StringComparison comparison = StringComparison.Ordinal)
	{
		for (int i = 0; i < array.Length; i++)
		{
			if (string.Equals(array[i], str, comparison))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060009D2 RID: 2514 RVA: 0x0002743C File Offset: 0x0002563C
	public static bool Contains(this List<string> list, string str, StringComparison comparison = StringComparison.Ordinal)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (string.Equals(list[i], str, comparison))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060009D3 RID: 2515 RVA: 0x0002746D File Offset: 0x0002566D
	public static bool TryGet<T>(this T[] array, int index, out T element)
	{
		if (index > -1 && index < array.Length)
		{
			element = array[index];
			return true;
		}
		element = default(T);
		return false;
	}

	// Token: 0x060009D4 RID: 2516 RVA: 0x00027490 File Offset: 0x00025690
	public static bool TryGet<T>(this List<T> list, int index, out T element)
	{
		if (index > -1 && index < list.Count)
		{
			element = list[index];
			return true;
		}
		element = default(T);
		return false;
	}

	// Token: 0x060009D5 RID: 2517 RVA: 0x000274B6 File Offset: 0x000256B6
	public static bool TryDequeue<T>(this Queue<T> queue, out T element)
	{
		if (queue.Count > 0)
		{
			element = queue.Dequeue();
			return true;
		}
		element = default(T);
		return false;
	}

	// Token: 0x060009D6 RID: 2518 RVA: 0x000274D8 File Offset: 0x000256D8
	public static T[] ToArray<T>(this Array array)
	{
		T[] array2 = new T[array.Length];
		array.CopyTo(array2, 0);
		return array2;
	}

	// Token: 0x060009D7 RID: 2519 RVA: 0x000274FC File Offset: 0x000256FC
	public static int IndexOf(this GameObject[] array, GameObject obj)
	{
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == obj)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060009D8 RID: 2520 RVA: 0x00027528 File Offset: 0x00025728
	public static int IndexOf(this List<GameObject> list, GameObject obj)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] == obj)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060009D9 RID: 2521 RVA: 0x00027558 File Offset: 0x00025758
	public static bool Contains(this GameObject[] array, GameObject obj)
	{
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == obj)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060009DA RID: 2522 RVA: 0x00027584 File Offset: 0x00025784
	public static bool Contains(this List<GameObject> list, GameObject obj)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] == obj)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060009DB RID: 2523 RVA: 0x000275B4 File Offset: 0x000257B4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T FirstElement<T>(this ArraySegment<T> segment)
	{
		return segment.Array[segment.Offset];
	}

	// Token: 0x060009DC RID: 2524 RVA: 0x000275C9 File Offset: 0x000257C9
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T At<T>(this ArraySegment<T> segment, int index)
	{
		return segment.Array[segment.Offset + index];
	}

	// Token: 0x060009DD RID: 2525 RVA: 0x000275E0 File Offset: 0x000257E0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ArraySegment<T> Segment<T>(this ArraySegment<T> segment, int offset)
	{
		return new ArraySegment<T>(segment.Array, segment.Offset + offset, segment.Count - offset);
	}

	// Token: 0x060009DE RID: 2526 RVA: 0x00027600 File Offset: 0x00025800
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ArraySegment<T> Segment<T>(this ArraySegment<T> segment, int offset, int length)
	{
		return new ArraySegment<T>(segment.Array, segment.Offset + offset, length);
	}

	// Token: 0x060009DF RID: 2527 RVA: 0x00027618 File Offset: 0x00025818
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ArraySegment<T> Segment<T>(this T[] array, int offset)
	{
		return new ArraySegment<T>(array, offset, array.Length - offset);
	}

	// Token: 0x060009E0 RID: 2528 RVA: 0x00027626 File Offset: 0x00025826
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ArraySegment<T> Segment<T>(this T[] array, int offset, int length)
	{
		return new ArraySegment<T>(array, offset, length);
	}

	// Token: 0x060009E1 RID: 2529 RVA: 0x00027630 File Offset: 0x00025830
	public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> factory) where TValue : class
	{
		TValue result;
		if (!dictionary.TryGetValue(key, out result))
		{
			return dictionary[key] = factory();
		}
		return result;
	}

	// Token: 0x060009E2 RID: 2530 RVA: 0x0002765C File Offset: 0x0002585C
	public static HashSet<TValue> GetOrAddNew<TKey, TValue>(this Dictionary<TKey, HashSet<TValue>> dictionary, TKey key)
	{
		HashSet<TValue> result;
		if (!dictionary.TryGetValue(key, out result))
		{
			return dictionary[key] = new HashSet<TValue>();
		}
		return result;
	}

	// Token: 0x060009E3 RID: 2531 RVA: 0x00027688 File Offset: 0x00025888
	public static List<TValue> GetOrAddNew<TKey, TValue>(this Dictionary<TKey, List<TValue>> dictionary, TKey key)
	{
		List<TValue> result;
		if (!dictionary.TryGetValue(key, out result))
		{
			return dictionary[key] = new List<TValue>();
		}
		return result;
	}

	// Token: 0x060009E4 RID: 2532 RVA: 0x000276B4 File Offset: 0x000258B4
	public static Queue<TValue> GetOrAddNew<TKey, TValue>(this Dictionary<TKey, Queue<TValue>> dictionary, TKey key)
	{
		Queue<TValue> result;
		if (!dictionary.TryGetValue(key, out result))
		{
			return dictionary[key] = new Queue<TValue>();
		}
		return result;
	}

	// Token: 0x060009E5 RID: 2533 RVA: 0x000276E0 File Offset: 0x000258E0
	public static Stack<TValue> GetOrAddNew<TKey, TValue>(this Dictionary<TKey, Stack<TValue>> dictionary, TKey key)
	{
		Stack<TValue> result;
		if (!dictionary.TryGetValue(key, out result))
		{
			return dictionary[key] = new Stack<TValue>();
		}
		return result;
	}
}
