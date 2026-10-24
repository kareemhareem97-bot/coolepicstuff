using System;
using System.Collections.Generic;
using RemoteAdmin;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009EA RID: 2538
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class ExecuteAsCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000BD1 RID: 3025
		// (get) Token: 0x0600394E RID: 14670 RVA: 0x000BB735 File Offset: 0x000B9935
		public string Command { get; } = "executeas";

		// Token: 0x17000BD2 RID: 3026
		// (get) Token: 0x0600394F RID: 14671 RVA: 0x000BB73D File Offset: 0x000B993D
		public string[] Aliases { get; } = new string[]
		{
			"runas"
		};

		// Token: 0x17000BD3 RID: 3027
		// (get) Token: 0x06003950 RID: 14672 RVA: 0x000BB745 File Offset: 0x000B9945
		public string Description { get; } = "Runs a command as another player.";

		// Token: 0x17000BD4 RID: 3028
		// (get) Token: 0x06003951 RID: 14673 RVA: 0x000BB74D File Offset: 0x000B994D
		public string[] Usage { get; } = new string[]
		{
			"%player%",
			"Command"
		};

		// Token: 0x06003952 RID: 14674 RVA: 0x000BB758 File Offset: 0x000B9958
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.ExecuteAs, out response))
			{
				return false;
			}
			if (arguments.Count < 1)
			{
				response = "You must specify the players and the command to run!\nUsage: " + this.Command + " " + this.DisplayCommandUsage();
				return false;
			}
			string[] value;
			List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out value, false);
			if (list == null)
			{
				response = "An unexpected problem has occurred during PlayerId or name array processing.";
				return false;
			}
			string text = string.Join(' ', value);
			int num = 0;
			foreach (ReferenceHub referenceHub in list)
			{
				PlayerCommandSender playerCommandSender;
				if (!(referenceHub == null) && referenceHub.queryProcessor.TryGetSender(out playerCommandSender))
				{
					ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Concat(new string[]
					{
						sender.LogName,
						" executed the command ",
						text,
						" as ",
						playerCommandSender.LogName
					}), ServerLogs.ServerLogType.RemoteAdminActivity_Misc, false);
					ulong permissions = referenceHub.serverRoles.Permissions;
					PlayerCommandSender playerCommandSender2 = sender as PlayerCommandSender;
					ulong num2 = (playerCommandSender2 != null) ? playerCommandSender2.Permissions : ServerStatic.PermissionsHandler.FullPerm;
					referenceHub.serverRoles.Permissions &= num2;
					try
					{
						CommandProcessor.ProcessQuery(text, playerCommandSender);
					}
					catch (Exception ex)
					{
						response = "An error occurred while executing the command: " + ex.Message;
						return false;
					}
					referenceHub.serverRoles.Permissions = permissions;
					num++;
				}
			}
			response = string.Format("Done! The request affected {0} player{1}!", num, (num == 1) ? "" : "s");
			return true;
		}
	}
}
