using System;
using System.Collections.Generic;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using RemoteAdmin;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009F3 RID: 2547
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class GotoCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000BEE RID: 3054
		// (get) Token: 0x06003987 RID: 14727 RVA: 0x000BCCF6 File Offset: 0x000BAEF6
		public string Command { get; } = "goto";

		// Token: 0x17000BEF RID: 3055
		// (get) Token: 0x06003988 RID: 14728 RVA: 0x000BCCFE File Offset: 0x000BAEFE
		public string[] Aliases { get; }

		// Token: 0x17000BF0 RID: 3056
		// (get) Token: 0x06003989 RID: 14729 RVA: 0x000BCD06 File Offset: 0x000BAF06
		public string Description { get; } = "Teleports you to the specified player.";

		// Token: 0x17000BF1 RID: 3057
		// (get) Token: 0x0600398A RID: 14730 RVA: 0x000BCD0E File Offset: 0x000BAF0E
		public string[] Usage { get; } = new string[]
		{
			"%player%"
		};

		// Token: 0x0600398B RID: 14731 RVA: 0x000BCD18 File Offset: 0x000BAF18
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
			{
				return false;
			}
			PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
			if (playerCommandSender == null)
			{
				response = "You must be in-game to use this command!";
				return false;
			}
			if (arguments.Count < 1)
			{
				response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			if (!playerCommandSender.ReferenceHub.IsAlive())
			{
				response = "Command disabled when you are not alive!";
				return false;
			}
			string[] array;
			List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out array, false);
			if (list == null || list.Count != 1)
			{
				response = "This commands you to specify exactly one specific player!";
				return false;
			}
			if (playerCommandSender.ReferenceHub.TryOverridePosition(list[0].transform.position))
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " teleported themself to player " + list[0].LoggedNameFromRefHub() + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
				response = "Done!";
				return true;
			}
			response = "Your current role does not support this operation.";
			return false;
		}
	}
}
