using System;
using System.Collections.Generic;
using RemoteAdmin;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009CD RID: 2509
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class GbanKickCommand : ICommand, IHiddenCommand
	{
		// Token: 0x17000B78 RID: 2936
		// (get) Token: 0x060038AD RID: 14509 RVA: 0x000B8745 File Offset: 0x000B6945
		public string Command { get; } = "gban-kick";

		// Token: 0x17000B79 RID: 2937
		// (get) Token: 0x060038AE RID: 14510 RVA: 0x000B874D File Offset: 0x000B694D
		public string[] Aliases { get; }

		// Token: 0x17000B7A RID: 2938
		// (get) Token: 0x060038AF RID: 14511 RVA: 0x000B8755 File Offset: 0x000B6955
		public string Description { get; } = "Internal global banning use only.";

		// Token: 0x060038B0 RID: 14512 RVA: 0x000B8760 File Offset: 0x000B6960
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
			if (playerCommandSender == null)
			{
				response = "This command can only be executed by players in-game.";
				return false;
			}
			if (!playerCommandSender.ReferenceHub.authManager.RemoteAdminGlobalAccess)
			{
				response = "You do not have permission to run this command. Did you mean \"ban\"?";
				return false;
			}
			if (arguments.Count != 2)
			{
				response = "To run this program, type exactly 2 argument!.";
				return false;
			}
			List<int> list = Misc.ProcessRaPlayersList(arguments.At(0));
			if (list == null || list.Count != 1)
			{
				response = "An unexpected problem has occurred during PlayerId processing. (This command only accepts one ID).";
				return false;
			}
			ReferenceHub target;
			if (ReferenceHub.TryGetHub(list[0], out target))
			{
				BanPlayer.GlobalBanUser(target, playerCommandSender, arguments.At(1));
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " globally banned and kicked " + arguments.At(0) + " player.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
				response = "Done! Globally banned and kicked " + arguments.At(0) + ".";
				return true;
			}
			response = "Error finding player with that ID.";
			return false;
		}
	}
}
