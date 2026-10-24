using System;
using GameCore;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009FA RID: 2554
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class LobbyLockCommand : ICommand
	{
		// Token: 0x17000C0A RID: 3082
		// (get) Token: 0x060039B1 RID: 14769 RVA: 0x000BDA02 File Offset: 0x000BBC02
		public string Command { get; } = "lobbylock";

		// Token: 0x17000C0B RID: 3083
		// (get) Token: 0x060039B2 RID: 14770 RVA: 0x000BDA0A File Offset: 0x000BBC0A
		public string[] Aliases { get; } = new string[]
		{
			"ll",
			"llock"
		};

		// Token: 0x17000C0C RID: 3084
		// (get) Token: 0x060039B3 RID: 14771 RVA: 0x000BDA12 File Offset: 0x000BBC12
		public string Description { get; } = "Locks or unlocks the lobby (prevents the round from starting).";

		// Token: 0x060039B4 RID: 14772 RVA: 0x000BDA1C File Offset: 0x000BBC1C
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
			RoundStart.LobbyLock = !RoundStart.LobbyLock;
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + (RoundStart.LobbyLock ? " enabled " : " disabled ") + "lobby lock.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			response = "Lobby lock " + (RoundStart.LobbyLock ? "enabled!" : "disabled!");
			return true;
		}
	}
}
