using System;
using Mirror;
using UnityEngine;

namespace Hints
{
	// Token: 0x02000849 RID: 2121
	public static class HintMessageParameterFunctions
	{
		// Token: 0x06003045 RID: 12357 RVA: 0x000A1538 File Offset: 0x0009F738
		public static void Serialize(this NetworkWriter writer, HintMessage value)
		{
			Hint content = value.Content;
			if (!(content is TextHint))
			{
				if (!(content is TranslationHint))
				{
					Debug.LogError("Attempted to serialize an unknown type of HintMessage!");
					writer.WriteByte(0);
					return;
				}
				writer.WriteByte(2);
			}
			else
			{
				writer.WriteByte(1);
			}
			value.Content.Serialize(writer);
		}

		// Token: 0x06003046 RID: 12358 RVA: 0x000A1590 File Offset: 0x0009F790
		public static HintMessage Deserialize(this NetworkReader reader)
		{
			Hint content;
			switch (reader.ReadByte())
			{
			case 0:
				Debug.LogError("Unknown type of HintMessage has been received!");
				return default(HintMessage);
			case 1:
				content = TextHint.FromNetwork(reader);
				break;
			case 2:
				content = TranslationHint.FromNetwork(reader);
				break;
			default:
				Debug.LogError("Invalid type of HintMessage has been received!");
				return default(HintMessage);
			}
			return new HintMessage(content);
		}

		// Token: 0x0200084A RID: 2122
		private enum HintMessageTypes : byte
		{
			// Token: 0x04002577 RID: 9591
			Unknown,
			// Token: 0x04002578 RID: 9592
			TextHint,
			// Token: 0x04002579 RID: 9593
			TranslationHint
		}
	}
}
