using System;
using Mirror;

namespace Hints
{
	// Token: 0x020007F9 RID: 2041
	public class AlphaEffect : HintEffect
	{
		// Token: 0x06002F5C RID: 12124 RVA: 0x0009FF11 File Offset: 0x0009E111
		public static AlphaEffect FromNetwork(NetworkReader reader)
		{
			AlphaEffect alphaEffect = new AlphaEffect();
			alphaEffect.Deserialize(reader);
			return alphaEffect;
		}

		// Token: 0x170008D1 RID: 2257
		// (get) Token: 0x06002F5D RID: 12125 RVA: 0x0009FF1F File Offset: 0x0009E11F
		// (set) Token: 0x06002F5E RID: 12126 RVA: 0x0009FF27 File Offset: 0x0009E127
		private protected float Alpha { protected get; private set; }

		// Token: 0x06002F5F RID: 12127 RVA: 0x0009FF30 File Offset: 0x0009E130
		private AlphaEffect() : base(0f, 1f)
		{
		}

		// Token: 0x06002F60 RID: 12128 RVA: 0x0009FF42 File Offset: 0x0009E142
		public AlphaEffect(float alpha, float startScalar = 0f, float durationScalar = 1f) : base(startScalar, durationScalar)
		{
			this.Alpha = alpha;
		}

		// Token: 0x06002F61 RID: 12129 RVA: 0x0009FF53 File Offset: 0x0009E153
		public override void Deserialize(NetworkReader reader)
		{
			base.Deserialize(reader);
			this.Alpha = reader.ReadFloat();
		}

		// Token: 0x06002F62 RID: 12130 RVA: 0x0009FF68 File Offset: 0x0009E168
		public override void Serialize(NetworkWriter writer)
		{
			base.Serialize(writer);
			writer.WriteFloat(this.Alpha);
		}
	}
}
