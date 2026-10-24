using System;
using Mirror;

namespace Hints
{
	// Token: 0x02000847 RID: 2119
	public abstract class HintEffect : DisplayableObject<SharedHintData>
	{
		// Token: 0x170008DE RID: 2270
		// (get) Token: 0x0600303F RID: 12351 RVA: 0x000A14E4 File Offset: 0x0009F6E4
		// (set) Token: 0x06003040 RID: 12352 RVA: 0x000A14EC File Offset: 0x0009F6EC
		public float StartScalar { get; private set; }

		// Token: 0x06003041 RID: 12353 RVA: 0x000A14F5 File Offset: 0x0009F6F5
		protected HintEffect(float startScalar = 0f, float durationScalar = 1f) : base(durationScalar)
		{
			this.StartScalar = startScalar;
		}

		// Token: 0x06003042 RID: 12354 RVA: 0x000A1505 File Offset: 0x0009F705
		public override void Deserialize(NetworkReader reader)
		{
			base.Deserialize(reader);
			this.StartScalar = reader.ReadFloat();
		}

		// Token: 0x06003043 RID: 12355 RVA: 0x000A151A File Offset: 0x0009F71A
		public override void Serialize(NetworkWriter writer)
		{
			base.Serialize(writer);
			writer.WriteFloat(this.StartScalar);
		}
	}
}
