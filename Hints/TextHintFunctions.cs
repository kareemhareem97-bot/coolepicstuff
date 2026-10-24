using System;
using Mirror;

namespace Hints
{
	// Token: 0x02000803 RID: 2051
	public static class TextHintFunctions
	{
		// Token: 0x06002F81 RID: 12161 RVA: 0x0009FF00 File Offset: 0x0009E100
		public static void Serialize(this NetworkWriter writer, TextHint value)
		{
			value.Serialize(writer);
		}

		// Token: 0x06002F82 RID: 12162 RVA: 0x000A026E File Offset: 0x0009E46E
		public static TextHint Deserialize(this NetworkReader reader)
		{
			return TextHint.FromNetwork(reader);
		}
	}
}
