using System;
using Mirror;

namespace Hints
{
	// Token: 0x0200081D RID: 2077
	public abstract class FormattablePrimitiveHintParameter<TValue> : PrimitiveHintParameter<TValue>
	{
		// Token: 0x170008D9 RID: 2265
		// (get) Token: 0x06002FD4 RID: 12244 RVA: 0x000A0A2A File Offset: 0x0009EC2A
		// (set) Token: 0x06002FD5 RID: 12245 RVA: 0x000A0A32 File Offset: 0x0009EC32
		private protected string Format { protected get; private set; }

		// Token: 0x06002FD6 RID: 12246 RVA: 0x000A0A3B File Offset: 0x0009EC3B
		protected FormattablePrimitiveHintParameter(TValue value, string format, Func<TValue, string, string> formatter, Func<NetworkReader, TValue> deserializer, Action<NetworkWriter, TValue> serializer) : base(value, deserializer, serializer)
		{
			this._formatter = formatter;
			this.Format = format;
		}

		// Token: 0x06002FD7 RID: 12247 RVA: 0x000A0A58 File Offset: 0x0009EC58
		protected FormattablePrimitiveHintParameter(Func<TValue, string, string> formatter, Func<NetworkReader, TValue> deserializer, Action<NetworkWriter, TValue> serializer) : this(default(TValue), string.Empty, formatter, deserializer, serializer)
		{
		}

		// Token: 0x06002FD8 RID: 12248 RVA: 0x000A0A7C File Offset: 0x0009EC7C
		public override void Deserialize(NetworkReader reader)
		{
			base.Deserialize(reader);
			this.Format = reader.ReadString();
		}

		// Token: 0x06002FD9 RID: 12249 RVA: 0x000A0A91 File Offset: 0x0009EC91
		public override void Serialize(NetworkWriter writer)
		{
			base.Serialize(writer);
			writer.WriteString(this.Format);
		}

		// Token: 0x06002FDA RID: 12250 RVA: 0x000A0AA6 File Offset: 0x0009ECA6
		protected override string FormatValue(float progress, out bool stopFormatting)
		{
			stopFormatting = true;
			return this._formatter(base.Value, this.Format);
		}

		// Token: 0x04002549 RID: 9545
		private readonly Func<TValue, string, string> _formatter;
	}
}
