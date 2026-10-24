using System;
using RoundRestarting;

namespace CommandSystem.Commands.RemoteAdmin.ServerEvent
{
	// Token: 0x02000A3F RID: 2623
	[CommandHandler(typeof(ServerEventCommand))]
	public class RoundRestartCommand : ICommand
	{
		// Token: 0x17000CDB RID: 3291
		// (get) Token: 0x06003B43 RID: 15171 RVA: 0x000C3538 File Offset: 0x000C1738
		public string Command { get; } = "ROUND_RESTART";

		// Token: 0x17000CDC RID: 3292
		// (get) Token: 0x06003B44 RID: 15172 RVA: 0x000C3540 File Offset: 0x000C1740
		public string[] Aliases { get; } = new string[]
		{
			"RR",
			"RESTART",
			"ROUNDRESTART"
		};

		// Token: 0x17000CDD RID: 3293
		// (get) Token: 0x06003B45 RID: 15173 RVA: 0x000C3548 File Offset: 0x000C1748
		public string Description { get; } = "Restarts the current round.";

		// Token: 0x06003B46 RID: 15174 RVA: 0x000C3550 File Offset: 0x000C1750
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RoundEvents, out response))
			{
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " forced round restart.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			RoundRestart.InitiateRoundRestart();
			response = "Round restart forced.";
			return true;
		}
	}
}
