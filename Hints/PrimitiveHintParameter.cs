using System;
using Mirror;

namespace Hints
{
	// Token: 0x0200082C RID: 2092
	public abstract class PrimitiveHintParameter<TValue> : HintParameter
	{
		// Token: 0x170008DA RID: 2266
		// (get) Token: 0x06002FF7 RID: 12279 RVA: 0x000A0D7D File Offset: 0x0009EF7D
		// (set) Token: 0x06002FF8 RID: 12280 RVA: 0x000A0D85 File Offset: 0x0009EF85
		private protected TValue Value { protected get; private set; }

		// Token: 0x06002FF9 RID: 12281 RVA: 0x000A0D8E File Offset: 0x0009EF8E
		protected PrimitiveHintParameter(Func<NetworkReader, TValue> deserializer, Action<NetworkWriter, TValue> serializer)
		{
			this._deserializer = deserializer;
			this._serializer = serializer;
		}

		// Token: 0x06002FFA RID: 12282 RVA: 0x000A0DA4 File Offset: 0x0009EFA4
		protected PrimitiveHintParameter(TValue value, Func<NetworkReader, TValue> deserializer, Action<NetworkWriter, TValue> serializer) : this(deserializer, serializer)
		{
			this.Value = value;
		}

		// Token: 0x06002FFB RID: 12283 RVA: 0x000A0DB5 File Offset: 0x0009EFB5
		public override void Deserialize(NetworkReader reader)
		{
			this.Value = this._deserializer(reader);
		}

		// Token: 0x06002FFC RID: 12284 RVA: 0x000A0DC9 File Offset: 0x0009EFC9
		public override void Serialize(NetworkWriter writer)
		{
			this._serializer(writer, this.Value);
		}

		// Token: 0x06002FFD RID: 12285 RVA: 0x000A0DDD File Offset: 0x0009EFDD
		protected override string UpdateState(float progress)
		{
			if (!this._stopFormatting)
			{
				return this.FormatValue(progress, out this._stopFormatting);
			}
			return null;
		}

		// Token: 0x06002FFE RID: 12286 RVA: 0x000A0DF8 File Offset: 0x0009EFF8
		protected virtual string FormatValue(float progress, out bool stopFormatting)
		{
			stopFormatting = true;
			TValue value = this.Value;
			return value.ToString();
		}

		// Token: 0x04002557 RID: 9559
		private readonly Func<NetworkReader, TValue> _deserializer;

		// Token: 0x04002558 RID: 9560
		private readonly Action<NetworkWriter, TValue> _serializer;

		// Token: 0x04002559 RID: 9561
		private bool _stopFormatting;
	}
}
