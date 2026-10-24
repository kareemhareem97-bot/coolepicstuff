using System;

namespace CommandSystem.Commands.RemoteAdmin.ServerEvent
{
	// Token: 0x02000A3E RID: 2622
	[CommandHandler(typeof(ServerEventCommand))]
	public class DetonationStartCommand : ICommand
	{
		// Token: 0x17000CD8 RID: 3288
		// (get) Token: 0x06003B3E RID: 15166 RVA: 0x000C348B File Offset: 0x000C168B
		public string Command { get; } = "DETONATION_START";

		// Token: 0x17000CD9 RID: 3289
		// (get) Token: 0x06003B3F RID: 15167 RVA: 0x000C3493 File Offset: 0x000C1693
		public string[] Aliases { get; }

		// Token: 0x17000CDA RID: 3290
		// (get) Token: 0x06003B40 RID: 15168 RVA: 0x000C349B File Offset: 0x000C169B
		public string Description { get; } = "Force the alpha warhead detonation to start.";

		// Token: 0x06003B41 RID: 15169 RVA: 0x000C34A4 File Offset: 0x000C16A4
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.WarheadEvents, out response))
			{
				return false;
			}
			AlphaWarheadController.Singleton.InstantPrepare();
			bool isLocked = AlphaWarheadController.Singleton.IsLocked;
			AlphaWarheadController.Singleton.IsLocked = false;
			AlphaWarheadController.Singleton.StartDetonation(false, false, null);
			AlphaWarheadController.Singleton.IsLocked = isLocked;
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " started warhead detonation.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			response = "Warhead detonation started.";
			return true;
		}
	}
}
