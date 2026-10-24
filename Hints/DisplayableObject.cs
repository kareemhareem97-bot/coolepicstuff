using System;
using Mirror;

namespace Hints
{
	// Token: 0x020007F6 RID: 2038
	public abstract class DisplayableObject<TData> : NetworkObject<TData>
	{
		// Token: 0x170008D0 RID: 2256
		// (get) Token: 0x06002F52 RID: 12114 RVA: 0x0009FE99 File Offset: 0x0009E099
		// (set) Token: 0x06002F53 RID: 12115 RVA: 0x0009FEA1 File Offset: 0x0009E0A1
		public float DurationScalar { get; private set; }

		// Token: 0x06002F54 RID: 12116 RVA: 0x0009FEAA File Offset: 0x0009E0AA
		protected DisplayableObject(float durationScalar = 1f)
		{
			this.DurationScalar = durationScalar;
		}

		// Token: 0x06002F55 RID: 12117 RVA: 0x0009FEB9 File Offset: 0x0009E0B9
		public override void Deserialize(NetworkReader reader)
		{
			this.DurationScalar = reader.ReadFloat();
		}

		// Token: 0x06002F56 RID: 12118 RVA: 0x0009FEC7 File Offset: 0x0009E0C7
		public override void Serialize(NetworkWriter writer)
		{
			writer.WriteFloat(this.DurationScalar);
		}
	}
}
