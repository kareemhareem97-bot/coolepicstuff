using System;
using Mirror;

namespace Hints
{
	// Token: 0x02000839 RID: 2105
	public class TimespanHintParameter : DoubleHintParameter
	{
		// Token: 0x06003015 RID: 12309 RVA: 0x000A1147 File Offset: 0x0009F347
		public new static TimespanHintParameter FromNetwork(NetworkReader reader)
		{
			TimespanHintParameter timespanHintParameter = new TimespanHintParameter();
			timespanHintParameter.Deserialize(reader);
			return timespanHintParameter;
		}

		// Token: 0x06003016 RID: 12310 RVA: 0x000A1155 File Offset: 0x0009F355
		public static TimespanHintParameter FromOffset(double offset, string format, bool negate)
		{
			return new TimespanHintParameter(NetworkTime.time + offset, format, negate);
		}

		// Token: 0x06003017 RID: 12311 RVA: 0x000A1165 File Offset: 0x0009F365
		public static TimespanHintParameter FromOffset(TimeSpan offset, string format, bool negate)
		{
			return TimespanHintParameter.FromOffset(offset.TotalSeconds, format, negate);
		}

		// Token: 0x170008DB RID: 2267
		// (get) Token: 0x06003018 RID: 12312 RVA: 0x000A1175 File Offset: 0x0009F375
		// (set) Token: 0x06003019 RID: 12313 RVA: 0x000A117D File Offset: 0x0009F37D
		private protected bool Negate { protected get; private set; }

		// Token: 0x170008DC RID: 2268
		// (get) Token: 0x0600301A RID: 12314 RVA: 0x000A1188 File Offset: 0x0009F388
		protected TimeSpan OffsetTime
		{
			get
			{
				TimeSpan result = TimeSpan.FromSeconds(base.Value - NetworkTime.time);
				if (!this.Negate)
				{
					return result;
				}
				return result.Negate();
			}
		}

		// Token: 0x0600301B RID: 12315 RVA: 0x000A11B8 File Offset: 0x0009F3B8
		protected TimespanHintParameter()
		{
		}

		// Token: 0x0600301C RID: 12316 RVA: 0x000A11C0 File Offset: 0x0009F3C0
		public TimespanHintParameter(double sourceTime, string format, bool negate) : base(sourceTime, format)
		{
			this.Negate = negate;
		}

		// Token: 0x0600301D RID: 12317 RVA: 0x000A11D4 File Offset: 0x0009F3D4
		public TimespanHintParameter(DateTimeOffset sourceTime, string format, bool negate) : this((sourceTime - DateTimeOffset.UtcNow).TotalSeconds, format, negate)
		{
		}

		// Token: 0x0600301E RID: 12318 RVA: 0x000A11FC File Offset: 0x0009F3FC
		public override void Deserialize(NetworkReader reader)
		{
			base.Deserialize(reader);
			this.Negate = reader.ReadBool();
		}

		// Token: 0x0600301F RID: 12319 RVA: 0x000A1211 File Offset: 0x0009F411
		public override void Serialize(NetworkWriter writer)
		{
			base.Serialize(writer);
			writer.WriteBool(this.Negate);
		}

		// Token: 0x06003020 RID: 12320 RVA: 0x000A1228 File Offset: 0x0009F428
		protected override string UpdateState(float progress)
		{
			return this.OffsetTime.ToString(base.Format);
		}
	}
}
