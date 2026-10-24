using System;
using System.Collections.Generic;
using UnityEngine;

namespace FastResources
{
	// Token: 0x0200086B RID: 2155
	public class FastResourcesDatabase : ScriptableObject
	{
		// Token: 0x06003130 RID: 12592 RVA: 0x000A537C File Offset: 0x000A357C
		public static T GetFirst<T>() where T : UnityEngine.Object
		{
			T result;
			if (!FastResourcesDatabase.TryGetFirst<T>(out result))
			{
				return default(T);
			}
			return result;
		}

		// Token: 0x06003131 RID: 12593 RVA: 0x000A53A0 File Offset: 0x000A35A0
		public unsafe static bool TryGetFirst<T>(out T result) where T : UnityEngine.Object
		{
			ReadOnlySpan<T> readOnlySpan;
			if (FastResourcesDatabase.TryGetAll<T>(out readOnlySpan))
			{
				result = *readOnlySpan[0];
				return true;
			}
			result = default(T);
			return false;
		}

		// Token: 0x06003132 RID: 12594 RVA: 0x000A53D4 File Offset: 0x000A35D4
		public static ReadOnlySpan<T> GetAll<T>() where T : UnityEngine.Object
		{
			ReadOnlySpan<T> result;
			if (!FastResourcesDatabase.TryGetAll<T>(out result))
			{
				return ReadOnlySpan<T>.Empty;
			}
			return result;
		}

		// Token: 0x06003133 RID: 12595 RVA: 0x000A53F4 File Offset: 0x000A35F4
		public static bool TryGetAll<T>(out ReadOnlySpan<T> result) where T : UnityEngine.Object
		{
			FastResourcesDatabase.ValidateDictionaryCache();
			Type typeFromHandle = typeof(T);
			object obj;
			if (FastResourcesDatabase._dictionarizedTyped.TryGetValue(typeFromHandle, out obj))
			{
				result = new ReadOnlySpan<T>(obj as T[]);
				return result.Length > 0;
			}
			List<UnityEngine.Object> list;
			if (!FastResourcesDatabase._dictionarizedRaw.TryGetValue(typeFromHandle, out list))
			{
				result = null;
				return false;
			}
			T[] array = new T[list.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (list[i] as T);
			}
			FastResourcesDatabase._dictionarizedTyped[typeFromHandle] = array;
			result = new ReadOnlySpan<T>(array);
			return true;
		}

		// Token: 0x06003134 RID: 12596 RVA: 0x000A54AC File Offset: 0x000A36AC
		private static void ValidateDictionaryCache()
		{
			if (FastResourcesDatabase._cacheSet)
			{
				return;
			}
			FastResourcesDatabase.ResourcesTypePair[] resources = Resources.Load<FastResourcesDatabase>("FastResources/FastResources")._resources;
			FastResourcesDatabase._dictionarizedRaw = new Dictionary<Type, List<UnityEngine.Object>>(resources.Length);
			FastResourcesDatabase._dictionarizedTyped = new Dictionary<Type, object>(resources.Length);
			foreach (FastResourcesDatabase.ResourcesTypePair resourcesTypePair in resources)
			{
				Type type = Type.GetType(resourcesTypePair.TypeName);
				if (!FastResourcesDatabase._dictionarizedRaw.TryAdd(type, resourcesTypePair.Resources))
				{
					Debug.LogError("Fast resource type " + resourcesTypePair.TypeName + " already existed in dictionary.");
				}
			}
			FastResourcesDatabase._cacheSet = true;
		}

		// Token: 0x0400264B RID: 9803
		public const string FullDatabasePath = "Assets/_Scripts/FastResources/Resources/FastResources/FastResources.asset";

		// Token: 0x0400264C RID: 9804
		public const string RelativeDatabasePath = "FastResources/FastResources";

		// Token: 0x0400264D RID: 9805
		private static Dictionary<Type, List<UnityEngine.Object>> _dictionarizedRaw;

		// Token: 0x0400264E RID: 9806
		private static Dictionary<Type, object> _dictionarizedTyped;

		// Token: 0x0400264F RID: 9807
		private static bool _cacheSet;

		// Token: 0x04002650 RID: 9808
		[SerializeField]
		private FastResourcesDatabase.ResourcesTypePair[] _resources;

		// Token: 0x0200086C RID: 2156
		[Serializable]
		private struct ResourcesTypePair
		{
			// Token: 0x04002651 RID: 9809
			public string TypeName;

			// Token: 0x04002652 RID: 9810
			public List<UnityEngine.Object> Resources;
		}
	}
}
