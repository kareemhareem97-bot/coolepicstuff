using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Mirror;

namespace Hints
{
	// Token: 0x0200081A RID: 2074
	public class FloatHintParameter : FormattablePrimitiveHintParameter<float>
	{
		// Token: 0x06002FCE RID: 12238 RVA: 0x000A0928 File Offset: 0x0009EB28
		public static FloatHintParameter FromNetwork(NetworkReader reader)
		{
			FloatHintParameter floatHintParameter = new FloatHintParameter();
			floatHintParameter.Deserialize(reader);
			return floatHintParameter;
		}

		// Token: 0x06002FCF RID: 12239 RVA: 0x000A0936 File Offset: 0x0009EB36
		private static string FormatFloat(float value, string format)
		{
			if (format != null)
			{
				return value.ToString(format, CultureInfo.CurrentCulture);
			}
			return value.ToString(CultureInfo.CurrentCulture);
		}

		// Token: 0x06002FD0 RID: 12240 RVA: 0x000A0958 File Offset: 0x0009EB58
		protected FloatHintParameter()
		{
			Func<float, string, string> formatter;
			if ((formatter = FloatHintParameter.<>O.<0>__FormatFloat) == null)
			{
				formatter = (FloatHintParameter.<>O.<0>__FormatFloat = new Func<float, string, string>(FloatHintParameter.FormatFloat));
			}
			Func<NetworkReader, float> deserializer;
			if ((deserializer = FloatHintParameter.<>O.<1>__ReadFloat) == null)
			{
				deserializer = (FloatHintParameter.<>O.<1>__ReadFloat = new Func<NetworkReader, float>(NetworkReaderExtensions.ReadFloat));
			}
			Action<NetworkWriter, float> serializer;
			if ((serializer = FloatHintParameter.<>O.<2>__WriteFloat) == null)
			{
				serializer = (FloatHintParameter.<>O.<2>__WriteFloat = new Action<NetworkWriter, float>(NetworkWriterExtensions.WriteFloat));
			}
			base..ctor(formatter, deserializer, serializer);
		}

		// Token: 0x06002FD1 RID: 12241 RVA: 0x000A09BC File Offset: 0x0009EBBC
		public FloatHintParameter(float value, string format)
		{
			Func<float, string, string> formatter;
			if ((formatter = FloatHintParameter.<>O.<0>__FormatFloat) == null)
			{
				formatter = (FloatHintParameter.<>O.<0>__FormatFloat = new Func<float, string, string>(FloatHintParameter.FormatFloat));
			}
			Func<NetworkReader, float> deserializer;
			if ((deserializer = FloatHintParameter.<>O.<1>__ReadFloat) == null)
			{
				deserializer = (FloatHintParameter.<>O.<1>__ReadFloat = new Func<NetworkReader, float>(NetworkReaderExtensions.ReadFloat));
			}
			Action<NetworkWriter, float> serializer;
			if ((serializer = FloatHintParameter.<>O.<2>__WriteFloat) == null)
			{
				serializer = (FloatHintParameter.<>O.<2>__WriteFloat = new Action<NetworkWriter, float>(NetworkWriterExtensions.WriteFloat));
			}
			base..ctor(value, format, formatter, deserializer, serializer);
		}

		// Token: 0x0200081B RID: 2075
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002546 RID: 9542
			public static Func<float, string, string> <0>__FormatFloat;

			// Token: 0x04002547 RID: 9543
			public static Func<NetworkReader, float> <1>__ReadFloat;

			// Token: 0x04002548 RID: 9544
			public static Action<NetworkWriter, float> <2>__WriteFloat;
		}
	}
}
