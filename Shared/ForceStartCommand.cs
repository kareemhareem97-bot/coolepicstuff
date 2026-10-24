using System;
using GameCore;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009B1 RID: 2481
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class ForceStartCommand : ICommand
	{
		// Token: 0x17000B28 RID: 2856
		// (get) Token: 0x0600381E RID: 14366 RVA: 0x000B6591 File Offset: 0x000B4791
		public string Command { get; } = "forcestart";

		// Token: 0x17000B29 RID: 2857
		// (get) Token: 0x0600381F RID: 14367 RVA: 0x000B6599 File Offset: 0x000B4799
		public string[] Aliases { get; } = new string[]
		{
			"fs",
			"rs",
			"start",
			"roundstart"
		};

		// Token: 0x17000B2A RID: 2858
		// (get) Token: 0x06003820 RID: 14368 RVA: 0x000B65A1 File Offset: 0x000B47A1
		public string Description { get; } = "Forces the round to start.";

		// Token: 0x06003821 RID: 14369 RVA: 0x000B65AC File Offset: 0x000B47AC
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RoundEvents, out response))
			{
				return false;
			}
			if (RoundStart.RoundStarted)
			{
				response = "This command can only be ran while in the lobby.";
				return false;
			}
			bool flag = CharacterClassManager.ForceRoundStart();
			if (flag)
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " forced round start.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			}
			response = (flag ? "Done! Forced round start." : "Failed to force start.");
			return flag;
		}
	}
}
