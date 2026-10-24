using System;
using Mirror;
using UnityEngine;

namespace Hints
{
	// Token: 0x020007FC RID: 2044
	public class OutlineEffect : HintEffect
	{
		// Token: 0x06002F6D RID: 12141 RVA: 0x000A0061 File Offset: 0x0009E261
		public static OutlineEffect FromNetwork(NetworkReader reader)
		{
			OutlineEffect outlineEffect = new OutlineEffect();
			outlineEffect.Deserialize(reader);
			return outlineEffect;
		}

		// Token: 0x170008D4 RID: 2260
		// (get) Token: 0x06002F6E RID: 12142 RVA: 0x000A006F File Offset: 0x0009E26F
		// (set) Token: 0x06002F6F RID: 12143 RVA: 0x000A0077 File Offset: 0x0009E277
		private protected Color32 OutlineColor { protected get; private set; }

		// Token: 0x170008D5 RID: 2261
		// (get) Token: 0x06002F70 RID: 12144 RVA: 0x000A0080 File Offset: 0x0009E280
		// (set) Token: 0x06002F71 RID: 12145 RVA: 0x000A0088 File Offset: 0x0009E288
		private protected float OutlineWidth { protected get; private set; }

		// Token: 0x06002F72 RID: 12146 RVA: 0x0009FF30 File Offset: 0x0009E130
		private OutlineEffect() : base(0f, 1f)
		{
		}

		// Token: 0x06002F73 RID: 12147 RVA: 0x000A0091 File Offset: 0x0009E291
		public OutlineEffect(Color32 outlineColor, float outlineWidth, float startScalar = 0f, float durationScalar = 1f) : base(startScalar, durationScalar)
		{
			this.OutlineColor = outlineColor;
			this.OutlineWidth = outlineWidth;
		}

		// Token: 0x06002F74 RID: 12148 RVA: 0x000A00AA File Offset: 0x0009E2AA
		public override void Deserialize(NetworkReader reader)
		{
			base.Deserialize(reader);
			this.OutlineColor = reader.ReadColor32();
			this.OutlineWidth = reader.ReadFloat();
		}

		// Token: 0x06002F75 RID: 12149 RVA: 0x000A00CB File Offset: 0x0009E2CB
		public override void Serialize(NetworkWriter writer)
		{
			base.Serialize(writer);
			writer.WriteColor32(this.OutlineColor);
			writer.WriteFloat(this.OutlineWidth);
		}
	}
}
