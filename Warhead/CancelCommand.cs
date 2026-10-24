using System;

namespace CommandSystem.Commands.RemoteAdmin.Warhead
{
	// Token: 0x02000A27 RID: 2599
	[CommandHandler(typeof(WarheadCommand))]
	public class CancelCommand : ICommand
	{
		// Token: 0x17000CA5 RID: 3237
		// (get) Token: 0x06003ACF RID: 15055 RVA: 0x000C226B File Offset: 0x000C046B
		public string Command { get; } = "cancel";

		// Token: 0x17000CA6 RID: 3238
		// (get) Token: 0x06003AD0 RID: 15056 RVA: 0x000C2273 File Offset: 0x000C0473
		public string[] Aliases { get; } = new string[]
		{
			"stop",
			"c"
		};

		// Token: 0x17000CA7 RID: 3239
		// (get) Token: 0x06003AD1 RID: 15057 RVA: 0x000C227B File Offset: 0x000C047B
		public string Description { get; } = "Stops the alpha warhead detonation sequence.";

		// Token: 0x06003AD2 RID: 15058 RVA: 0x000C2283 File Offset: 0x000C0483
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.WarheadEvents, out response))
			{
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " cancelled warhead detonation.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			AlphaWarheadController.Singleton.CancelDetonation(null);
			response = "Detonation has been canceled.";
			return true;
		}
	}
}
