using System;
using Mirror;

namespace Hints
{
	// Token: 0x02000813 RID: 2067
	public static class AnimationCurveHintParameterFunctions
	{
		// Token: 0x06002FC1 RID: 12225 RVA: 0x0009FF00 File Offset: 0x0009E100
		public static void Serialize(this NetworkWriter writer, AnimationCurveHintParameter value)
		{
			value.Serialize(writer);
		}

		// Token: 0x06002FC2 RID: 12226 RVA: 0x000A078C File Offset: 0x0009E98C
		public static AnimationCurveHintParameter Deserialize(this NetworkReader reader)
		{
			return AnimationCurveHintParameter.FromNetwork(reader);
		}
	}
}
