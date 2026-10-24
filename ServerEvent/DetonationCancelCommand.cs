using System;

namespace CommandSystem.Commands.RemoteAdmin.ServerEvent
{
	// Token: 0x02000A3C RID: 2620
	[CommandHandler(typeof(ServerEventCommand))]
	public class DetonationCancelCommand : ICommand
	{
		// Token: 0x17000CD2 RID: 3282
		// (get) Token: 0x06003B34 RID: 15156 RVA: 0x000C3360 File Offset: 0x000C1560
		public string Command { get; } = "DETONATION_CANCEL";

		// Token: 0x17000CD3 RID: 3283
		// (get) Token: 0x06003B35 RID: 15157 RVA: 0x000C3368 File Offset: 0x000C1568
		public string[] Aliases { get; }

		// Token: 0x17000CD4 RID: 3284
		// (get) Token: 0x06003B36 RID: 15158 RVA: 0x000C3370 File Offset: 0x000C1570
		public string Description { get; } = "Cancels the alpha warhead detonation sequence.";

		// Token: 0x06003B37 RID: 15159 RVA: 0x000C3378 File Offset: 0x000C1578
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.WarheadEvents, out response))
			{
				return false;
			}
			bool isLocked = AlphaWarheadController.Singleton.IsLocked;
			AlphaWarheadController.Singleton.IsLocked = false;
			DeadmanSwitch.Reset(true);
			AlphaWarheadController.Singleton.CancelDetonation();
			AlphaWarheadController.Singleton.IsLocked = isLocked;
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " cancelled warhead detonation.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			response = "Warhead detonation cancelled.";
			return true;
		}
	}
}
