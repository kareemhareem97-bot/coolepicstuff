using System;
using Mirror;

namespace Hints
{
	// Token: 0x020007F8 RID: 2040
	public static class AlphaCurveHintEffectFunctions
	{
		// Token: 0x06002F5A RID: 12122 RVA: 0x0009FF00 File Offset: 0x0009E100
		public static void Serialize(this NetworkWriter writer, AlphaCurveHintEffect value)
		{
			value.Serialize(writer);
		}

		// Token: 0x06002F5B RID: 12123 RVA: 0x0009FF09 File Offset: 0x0009E109
		public static AlphaCurveHintEffect Deserialize(this NetworkReader reader)
		{
			return AlphaCurveHintEffect.FromNetwork(reader);
		}
	}
}
