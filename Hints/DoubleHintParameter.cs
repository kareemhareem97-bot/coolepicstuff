using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Mirror;

namespace Hints
{
	// Token: 0x02000817 RID: 2071
	public class DoubleHintParameter : FormattablePrimitiveHintParameter<double>
	{
		// Token: 0x06002FC8 RID: 12232 RVA: 0x000A0827 File Offset: 0x0009EA27
		public static DoubleHintParameter FromNetwork(NetworkReader reader)
		{
			DoubleHintParameter doubleHintParameter = new DoubleHintParameter();
			doubleHintParameter.Deserialize(reader);
			return doubleHintParameter;
		}

		// Token: 0x06002FC9 RID: 12233 RVA: 0x000A0835 File Offset: 0x0009EA35
		private static string FormatDouble(double value, string format)
		{
			if (format != null)
			{
				return value.ToString(format, CultureInfo.CurrentCulture);
			}
			return value.ToString(CultureInfo.CurrentCulture);
		}

		// Token: 0x06002FCA RID: 12234 RVA: 0x000A0854 File Offset: 0x0009EA54
		public DoubleHintParameter(double value, string format)
		{
			Func<double, string, string> formatter;
			if ((formatter = DoubleHintParameter.<>O.<0>__FormatDouble) == null)
			{
				formatter = (DoubleHintParameter.<>O.<0>__FormatDouble = new Func<double, string, string>(DoubleHintParameter.FormatDouble));
			}
			Func<NetworkReader, double> deserializer;
			if ((deserializer = DoubleHintParameter.<>O.<1>__ReadDouble) == null)
			{
				deserializer = (DoubleHintParameter.<>O.<1>__ReadDouble = new Func<NetworkReader, double>(NetworkReaderExtensions.ReadDouble));
			}
			Action<NetworkWriter, double> serializer;
			if ((serializer = DoubleHintParameter.<>O.<2>__WriteDouble) == null)
			{
				serializer = (DoubleHintParameter.<>O.<2>__WriteDouble = new Action<NetworkWriter, double>(NetworkWriterExtensions.WriteDouble));
			}
			base..ctor(value, format, formatter, deserializer, serializer);
		}

		// Token: 0x06002FCB RID: 12235 RVA: 0x000A08BC File Offset: 0x0009EABC
		protected DoubleHintParameter()
		{
			Func<double, string, string> formatter;
			if ((formatter = DoubleHintParameter.<>O.<0>__FormatDouble) == null)
			{
				formatter = (DoubleHintParameter.<>O.<0>__FormatDouble = new Func<double, string, string>(DoubleHintParameter.FormatDouble));
			}
			Func<NetworkReader, double> deserializer;
			if ((deserializer = DoubleHintParameter.<>O.<1>__ReadDouble) == null)
			{
				deserializer = (DoubleHintParameter.<>O.<1>__ReadDouble = new Func<NetworkReader, double>(NetworkReaderExtensions.ReadDouble));
			}
			Action<NetworkWriter, double> serializer;
			if ((serializer = DoubleHintParameter.<>O.<2>__WriteDouble) == null)
			{
				serializer = (DoubleHintParameter.<>O.<2>__WriteDouble = new Action<NetworkWriter, double>(NetworkWriterExtensions.WriteDouble));
			}
			base..ctor(formatter, deserializer, serializer);
		}

		// Token: 0x02000818 RID: 2072
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002543 RID: 9539
			public static Func<double, string, string> <0>__FormatDouble;

			// Token: 0x04002544 RID: 9540
			public static Func<NetworkReader, double> <1>__ReadDouble;

			// Token: 0x04002545 RID: 9541
			public static Action<NetworkWriter, double> <2>__WriteDouble;
		}
	}
}
