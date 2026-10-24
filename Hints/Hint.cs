using System;
using Mirror;
using Utils.Networking;

namespace Hints
{
	// Token: 0x02000846 RID: 2118
	public abstract class Hint : DisplayableObject<SharedHintData>
	{
		// Token: 0x170008DD RID: 2269
		// (get) Token: 0x0600303A RID: 12346 RVA: 0x000A147A File Offset: 0x0009F67A
		// (set) Token: 0x0600303B RID: 12347 RVA: 0x000A1482 File Offset: 0x0009F682
		private protected HintParameter[] Parameters { protected get; private set; }

		// Token: 0x0600303C RID: 12348 RVA: 0x000A148B File Offset: 0x0009F68B
		protected Hint(HintParameter[] parameters, HintEffect[] effects, float durationScalar = 1f) : base(durationScalar)
		{
			this._effects = effects;
			this.Parameters = parameters;
		}

		// Token: 0x0600303D RID: 12349 RVA: 0x000A14A2 File Offset: 0x0009F6A2
		public override void Deserialize(NetworkReader reader)
		{
			base.Deserialize(reader);
			this._effects = reader.ReadHintEffectArray();
			this.Parameters = reader.ReadHintParameterArray();
		}

		// Token: 0x0600303E RID: 12350 RVA: 0x000A14C3 File Offset: 0x0009F6C3
		public override void Serialize(NetworkWriter writer)
		{
			base.Serialize(writer);
			writer.WriteHintEffectArray(this._effects);
			writer.WriteHintParameterArray(this.Parameters);
		}

		// Token: 0x04002572 RID: 9586
		private HintEffect[] _effects;
	}
}
