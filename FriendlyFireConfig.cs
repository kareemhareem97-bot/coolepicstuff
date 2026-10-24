using System;
using System.Globalization;

// Token: 0x02000125 RID: 293
internal static class FriendlyFireConfig
{
	// Token: 0x06000587 RID: 1415 RVA: 0x00014B60 File Offset: 0x00012D60
	internal static FriendlyFireAction ParseAction(string action)
	{
		string a = action.ToLower(CultureInfo.InvariantCulture);
		if (a == "kill")
		{
			return FriendlyFireAction.Kill;
		}
		if (a == "kick")
		{
			return FriendlyFireAction.Kick;
		}
		if (!(a == "ban"))
		{
			return FriendlyFireAction.Noop;
		}
		return FriendlyFireAction.Ban;
	}

	// Token: 0x040004A3 RID: 1187
	internal static bool RoundEnabled;

	// Token: 0x040004A4 RID: 1188
	internal static bool LifeEnabled;

	// Token: 0x040004A5 RID: 1189
	internal static bool WindowEnabled;

	// Token: 0x040004A6 RID: 1190
	internal static bool RespawnEnabled;

	// Token: 0x040004A7 RID: 1191
	internal static bool ExplosionAfterDisconnecting;

	// Token: 0x040004A8 RID: 1192
	internal static FriendlyFireAction RoundAction;

	// Token: 0x040004A9 RID: 1193
	internal static FriendlyFireAction LifeAction;

	// Token: 0x040004AA RID: 1194
	internal static FriendlyFireAction WindowAction;

	// Token: 0x040004AB RID: 1195
	internal static FriendlyFireAction RespawnAction;

	// Token: 0x040004AC RID: 1196
	internal static FriendlyFireAction ExplosionAfterDisconnectingAction;

	// Token: 0x040004AD RID: 1197
	internal static uint RoundKillThreshold;

	// Token: 0x040004AE RID: 1198
	internal static uint LifeKillThreshold;

	// Token: 0x040004AF RID: 1199
	internal static uint WindowKillThreshold;

	// Token: 0x040004B0 RID: 1200
	internal static uint RespawnKillThreshold;

	// Token: 0x040004B1 RID: 1201
	internal static uint RoundDamageThreshold;

	// Token: 0x040004B2 RID: 1202
	internal static uint LifeDamageThreshold;

	// Token: 0x040004B3 RID: 1203
	internal static uint WindowDamageThreshold;

	// Token: 0x040004B4 RID: 1204
	internal static uint RespawnDamageThreshold;

	// Token: 0x040004B5 RID: 1205
	internal static long RoundBanTime;

	// Token: 0x040004B6 RID: 1206
	internal static long LifeBanTime;

	// Token: 0x040004B7 RID: 1207
	internal static long WindowBanTime;

	// Token: 0x040004B8 RID: 1208
	internal static long RespawnBanTime;

	// Token: 0x040004B9 RID: 1209
	internal static long ExplosionAfterDisconnectingTime;

	// Token: 0x040004BA RID: 1210
	internal static string RoundBanReason;

	// Token: 0x040004BB RID: 1211
	internal static string LifeBanReason;

	// Token: 0x040004BC RID: 1212
	internal static string WindowBanReason;

	// Token: 0x040004BD RID: 1213
	internal static string RespawnBanReason;

	// Token: 0x040004BE RID: 1214
	internal static string ExplosionAfterDisconnectingBanReason;

	// Token: 0x040004BF RID: 1215
	internal static string RoundKillReason;

	// Token: 0x040004C0 RID: 1216
	internal static string LifeKillReason;

	// Token: 0x040004C1 RID: 1217
	internal static string WindowKillReason;

	// Token: 0x040004C2 RID: 1218
	internal static string RespawnKillReason;

	// Token: 0x040004C3 RID: 1219
	internal static string RoundAdminMessage;

	// Token: 0x040004C4 RID: 1220
	internal static string LifeAdminMessage;

	// Token: 0x040004C5 RID: 1221
	internal static string WindowAdminMessage;

	// Token: 0x040004C6 RID: 1222
	internal static string RespawnAdminMessage;

	// Token: 0x040004C7 RID: 1223
	internal static string ExplosionAfterDisconnectingAdminMessage;

	// Token: 0x040004C8 RID: 1224
	internal static string RoundBroadcastMessage;

	// Token: 0x040004C9 RID: 1225
	internal static string LifeBroadcastMessage;

	// Token: 0x040004CA RID: 1226
	internal static string WindowBroadcastMessage;

	// Token: 0x040004CB RID: 1227
	internal static string RespawnBroadcastMessage;

	// Token: 0x040004CC RID: 1228
	internal static string ExplosionAfterDisconnectingBroadcastMessage;

	// Token: 0x040004CD RID: 1229
	internal static bool RoundWebhook;

	// Token: 0x040004CE RID: 1230
	internal static bool LifeWebhook;

	// Token: 0x040004CF RID: 1231
	internal static bool WindowWebhook;

	// Token: 0x040004D0 RID: 1232
	internal static bool RespawnWebhook;

	// Token: 0x040004D1 RID: 1233
	internal static bool ExplosionAfterDisconnectingWebhook;

	// Token: 0x040004D2 RID: 1234
	internal static uint Window;

	// Token: 0x040004D3 RID: 1235
	internal static uint RespawnWindow;

	// Token: 0x040004D4 RID: 1236
	internal static ushort BroadcastTime;

	// Token: 0x040004D5 RID: 1237
	internal static ushort AdminChatTime;

	// Token: 0x040004D6 RID: 1238
	internal static bool IgnoreClassDTeamkills;

	// Token: 0x040004D7 RID: 1239
	internal static string WebhookUrl;

	// Token: 0x040004D8 RID: 1240
	internal static bool PauseDetector;
}
