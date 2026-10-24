using System;

namespace CommandSystem.Commands.RemoteAdmin.Warhead
{
	// Token: 0x02000A2C RID: 2604
	[CommandHandler(typeof(WarheadCommand))]
	public class LockCommand : ICommand
	{
		// Token: 0x17000CB4 RID: 3252
		// (get) Token: 0x06003AE8 RID: 15080 RVA: 0x000C2555 File Offset: 0x000C0755
		public string Command { get; } = "lock";

		// Token: 0x17000CB5 RID: 3253
		// (get) Token: 0x06003AE9 RID: 15081 RVA: 0x000C255D File Offset: 0x000C075D
		public string[] Aliases { get; } = new string[]
		{
			"l",
			"lck"
		};

		// Token: 0x17000CB6 RID: 3254
		// (get) Token: 0x06003AEA RID: 15082 RVA: 0x000C2565 File Offset: 0x000C0765
		public string Description { get; } = "Locks the alpha warhead detonation so it cannot be activated.";

		// Token: 0x06003AEB RID: 15083 RVA: 0x000C2570 File Offset: 0x000C0770
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.WarheadEvents, out response))
			{
				return false;
			}
			if (AlphaWarheadController.Singleton == null)
			{
				response = "Warhead not found";
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " locked the alpha warhead detonation.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			AlphaWarheadController.Singleton.IsLocked = !AlphaWarheadController.Singleton.IsLocked;
			response = "Alpha Warhead Lock " + (AlphaWarheadController.Singleton.IsLocked ? "enabled!" : "disabled!");
			return true;
		}
	}
}
