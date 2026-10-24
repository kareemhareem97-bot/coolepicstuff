using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cassie
{
	// Token: 0x02000AB5 RID: 2741
	[CreateAssetMenu(fileName = "New CASSIE Clip Database", menuName = "ScriptableObject/CASSIE Clip Database")]
	public class CassieLineDatabase : ScriptableObject
	{
		// Token: 0x17000E12 RID: 3602
		// (get) Token: 0x06003DDA RID: 15834 RVA: 0x000CC2C7 File Offset: 0x000CA4C7
		// (set) Token: 0x06003DDB RID: 15835 RVA: 0x000CC2CF File Offset: 0x000CA4CF
		public CassieLine[] AllLines { get; private set; }

		// Token: 0x06003DDC RID: 15836 RVA: 0x000CC2D8 File Offset: 0x000CA4D8
		public void ValidateCache(bool forceReload = false)
		{
			if (CassieLineDatabase._cacheSet && !forceReload)
			{
				return;
			}
			foreach (KeyValuePair<char, List<CassieLine>> keyValuePair in CassieLineDatabase.LinesByFirstCharCache)
			{
				keyValuePair.Value.Clear();
			}
			foreach (CassieLine cassieLine in this.AllLines)
			{
				char key = cassieLine.ApiName[0];
				CassieLineDatabase.LinesByFirstCharCache.GetOrAddNew(key).Add(cassieLine);
			}
			CassieLineDatabase._cacheSet = true;
		}

		// Token: 0x06003DDD RID: 15837 RVA: 0x000CC37C File Offset: 0x000CA57C
		public unsafe bool TryGetLine(ReadOnlySpan<char> word, out CassieLine line)
		{
			this.ValidateCache(false);
			List<CassieLine> list;
			if (word.Length == 0 || !CassieLineDatabase.LinesByFirstCharCache.TryGetValue((char)(*word[0]), out list))
			{
				line = null;
				return false;
			}
			foreach (CassieLine cassieLine in list)
			{
				if (word.SequenceEqual(cassieLine.ApiName.AsSpan()))
				{
					line = cassieLine;
					return true;
				}
			}
			line = null;
			return false;
		}

		// Token: 0x04002D3B RID: 11579
		private static readonly Dictionary<char, List<CassieLine>> LinesByFirstCharCache = new Dictionary<char, List<CassieLine>>();

		// Token: 0x04002D3C RID: 11580
		private static bool _cacheSet;
	}
}
