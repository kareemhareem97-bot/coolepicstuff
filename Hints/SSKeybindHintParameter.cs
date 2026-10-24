using System;
using System.Runtime.CompilerServices;
using Mirror;
using UnityEngine;
using UserSettings.ServerSpecific;

namespace Hints
{
	// Token: 0x02000833 RID: 2099
	public class SSKeybindHintParameter : FormattablePrimitiveHintParameter<int>
	{
		// Token: 0x06003009 RID: 12297 RVA: 0x000A0F42 File Offset: 0x0009F142
		public static SSKeybindHintParameter FromNetwork(NetworkReader reader)
		{
			SSKeybindHintParameter sskeybindHintParameter = new SSKeybindHintParameter();
			sskeybindHintParameter.Deserialize(reader);
			return sskeybindHintParameter;
		}

		// Token: 0x0600300A RID: 12298 RVA: 0x000A0F50 File Offset: 0x0009F150
		private static string GetAndFormatKeybind(int value, string format)
		{
			ServerSpecificSettingBase[] definedSettings = ServerSpecificSettingsSync.DefinedSettings;
			int i = 0;
			while (i < definedSettings.Length)
			{
				SSKeybindSetting sskeybindSetting = definedSettings[i] as SSKeybindSetting;
				if (sskeybindSetting != null && sskeybindSetting.SettingId == value)
				{
					if (sskeybindSetting.AssignedKeyCode == KeyCode.None)
					{
						return string.Format(format, TranslationReader.Get("KeyCodes", 1, "KEY NOT ASSIGNED"));
					}
					string normalVersion = new ReadableKeyCode(sskeybindSetting.AssignedKeyCode).NormalVersion;
					return string.Format(format, normalVersion);
				}
				else
				{
					i++;
				}
			}
			return string.Format(format, TranslationReader.Get("KeyCodes", 0, "SERVER SETTING NOT FOUND"));
		}

		// Token: 0x0600300B RID: 12299 RVA: 0x000A0FD4 File Offset: 0x0009F1D4
		protected SSKeybindHintParameter()
		{
			Func<int, string, string> formatter;
			if ((formatter = SSKeybindHintParameter.<>O.<0>__GetAndFormatKeybind) == null)
			{
				formatter = (SSKeybindHintParameter.<>O.<0>__GetAndFormatKeybind = new Func<int, string, string>(SSKeybindHintParameter.GetAndFormatKeybind));
			}
			Func<NetworkReader, int> deserializer;
			if ((deserializer = SSKeybindHintParameter.<>O.<1>__ReadInt) == null)
			{
				deserializer = (SSKeybindHintParameter.<>O.<1>__ReadInt = new Func<NetworkReader, int>(NetworkReaderExtensions.ReadInt));
			}
			Action<NetworkWriter, int> serializer;
			if ((serializer = SSKeybindHintParameter.<>O.<2>__WriteInt) == null)
			{
				serializer = (SSKeybindHintParameter.<>O.<2>__WriteInt = new Action<NetworkWriter, int>(NetworkWriterExtensions.WriteInt));
			}
			base..ctor(formatter, deserializer, serializer);
		}

		// Token: 0x0600300C RID: 12300 RVA: 0x000A1038 File Offset: 0x0009F238
		public SSKeybindHintParameter(int value, string format)
		{
			Func<int, string, string> formatter;
			if ((formatter = SSKeybindHintParameter.<>O.<0>__GetAndFormatKeybind) == null)
			{
				formatter = (SSKeybindHintParameter.<>O.<0>__GetAndFormatKeybind = new Func<int, string, string>(SSKeybindHintParameter.GetAndFormatKeybind));
			}
			Func<NetworkReader, int> deserializer;
			if ((deserializer = SSKeybindHintParameter.<>O.<1>__ReadInt) == null)
			{
				deserializer = (SSKeybindHintParameter.<>O.<1>__ReadInt = new Func<NetworkReader, int>(NetworkReaderExtensions.ReadInt));
			}
			Action<NetworkWriter, int> serializer;
			if ((serializer = SSKeybindHintParameter.<>O.<2>__WriteInt) == null)
			{
				serializer = (SSKeybindHintParameter.<>O.<2>__WriteInt = new Action<NetworkWriter, int>(NetworkWriterExtensions.WriteInt));
			}
			base..ctor(value, format, formatter, deserializer, serializer);
		}

		// Token: 0x0600300D RID: 12301 RVA: 0x000A109E File Offset: 0x0009F29E
		public SSKeybindHintParameter(int value) : this(value, "[{0}]")
		{
		}

		// Token: 0x0400255F RID: 9567
		public const string SettingNotFound = "SERVER SETTING NOT FOUND";

		// Token: 0x04002560 RID: 9568
		public const string KeyNotAssigned = "KEY NOT ASSIGNED";

		// Token: 0x04002561 RID: 9569
		public const string DefaultKeybindFormat = "[{0}]";

		// Token: 0x02000834 RID: 2100
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002562 RID: 9570
			public static Func<int, string, string> <0>__GetAndFormatKeybind;

			// Token: 0x04002563 RID: 9571
			public static Func<NetworkReader, int> <1>__ReadInt;

			// Token: 0x04002564 RID: 9572
			public static Action<NetworkWriter, int> <2>__WriteInt;
		}
	}
}
