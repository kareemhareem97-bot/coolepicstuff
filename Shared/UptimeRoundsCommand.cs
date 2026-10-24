using System;
using RoundRestarting;
using UnityEngine;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009CA RID: 2506
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class UptimeRoundsCommand : ICommand
	{
		// Token: 0x17000B71 RID: 2929
		// (get) Token: 0x0600389F RID: 14495 RVA: 0x000B821D File Offset: 0x000B641D
		public string Command { get; } = "uptime";

		// Token: 0x17000B72 RID: 2930
		// (get) Token: 0x060038A0 RID: 14496 RVA: 0x000B8225 File Offset: 0x000B6425
		public string[] Aliases { get; } = new string[]
		{
			"rounds"
		};

		// Token: 0x17000B73 RID: 2931
		// (get) Token: 0x060038A1 RID: 14497 RVA: 0x000B822D File Offset: 0x000B642D
		public string Description { get; } = "Displays the uptime of the game.";

		// Token: 0x060038A2 RID: 14498 RVA: 0x000B8235 File Offset: 0x000B6435
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = string.Format("Server uptime: {0} seconds, {1} rounds.", Time.unscaledTime, RoundRestart.UptimeRounds);
			return true;
		}
	}
}
