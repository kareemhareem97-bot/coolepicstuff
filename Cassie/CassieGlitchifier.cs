using System;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace Cassie
{
	// Token: 0x02000AB4 RID: 2740
	public static class CassieGlitchifier
	{
		// Token: 0x06003DD7 RID: 15831 RVA: 0x000CC15C File Offset: 0x000CA35C
		public static string Glitchify(string original, float intensityScale)
		{
			if (intensityScale <= 0f)
			{
				return original;
			}
			float glitchChance = UnityEngine.Random.Range(0.1f, 0.14f) * intensityScale;
			float stutterChance = UnityEngine.Random.Range(0.07f, 0.09f) * intensityScale;
			return CassieGlitchifier.Glitchify(original, glitchChance, stutterChance);
		}

		// Token: 0x06003DD8 RID: 15832 RVA: 0x000CC1A0 File Offset: 0x000CA3A0
		public static string Glitchify(string original, float glitchChance, float stutterChance)
		{
			if (glitchChance <= 0f)
			{
				return original;
			}
			if (stutterChance <= 0f)
			{
				return original;
			}
			CassieGlitchifier.Sb.Clear();
			string[] array = original.Split(' ', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				CassieGlitchifier.Sb.Append(array[i]);
				CassieGlitchifier.Sb.Append(' ');
				if (i < array.Length - 1)
				{
					if (UnityEngine.Random.value < glitchChance)
					{
						CassieGlitchifier.Sb.Append(".G");
						CassieGlitchifier.Sb.Append(UnityEngine.Random.Range(1, 7));
						CassieGlitchifier.Sb.Append(' ');
					}
					if (UnityEngine.Random.value < stutterChance)
					{
						CassieGlitchifier.Sb.Append("$STUTTER_");
						CassieGlitchifier.Sb.Append((UnityEngine.Random.value * 0.7f).ToString(CultureInfo.InvariantCulture));
						CassieGlitchifier.Sb.Append("_0.13_");
						CassieGlitchifier.Sb.Append(UnityEngine.Random.Range(2, 6));
						CassieGlitchifier.Sb.Append(' ');
					}
				}
			}
			return CassieGlitchifier.Sb.ToString();
		}

		// Token: 0x04002D3A RID: 11578
		private static readonly StringBuilder Sb = new StringBuilder();
	}
}
