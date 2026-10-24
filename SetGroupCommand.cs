using System;
using System.Collections.Generic;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A1B RID: 2587
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class SetGroupCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000C79 RID: 3193
		// (get) Token: 0x06003A7A RID: 14970 RVA: 0x000C0EE8 File Offset: 0x000BF0E8
		public string Command { get; } = "setgroup";

		// Token: 0x17000C7A RID: 3194
		// (get) Token: 0x06003A7B RID: 14971 RVA: 0x000C0EF0 File Offset: 0x000BF0F0
		public string[] Aliases { get; } = new string[]
		{
			"sg",
			"setrole"
		};

		// Token: 0x17000C7B RID: 3195
		// (get) Token: 0x06003A7C RID: 14972 RVA: 0x000C0EF8 File Offset: 0x000BF0F8
		public string Description { get; } = "Temporarily assigns the specified player(s) to a specified group.";

		// Token: 0x17000C7C RID: 3196
		// (get) Token: 0x06003A7D RID: 14973 RVA: 0x000C0F00 File Offset: 0x000BF100
		public string[] Usage { get; } = new string[]
		{
			"%player%",
			"Group (-1 Removes)"
		};

		// Token: 0x06003A7E RID: 14974 RVA: 0x000C0F08 File Offset: 0x000BF108
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.SetGroup, out response))
			{
				return false;
			}
			if (arguments.Count < 2)
			{
				response = "To execute this command provide at least 2 arguments!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			string[] array;
			List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out array, false);
			UserGroup userGroup = null;
			if (array[0] != "-1")
			{
				userGroup = ServerStatic.PermissionsHandler.GetGroup(array[0]);
				if (userGroup == null)
				{
					response = "Requested group doesn't exist! Use group \"-1\" to remove user group.";
					return false;
				}
			}
			int num = 0;
			int num2 = 0;
			string text = string.Empty;
			foreach (ReferenceHub referenceHub in list)
			{
				if (referenceHub.encryptedChannelManager.EncryptionKey == null)
				{
					text = text + "Empty encryption key of player " + referenceHub.nicknameSync.MyNick + ". Make sure to use online mode or ask that player to use RA password before changing the role.\n";
					num2++;
				}
				else
				{
					if (userGroup == null)
					{
						ServerLogs.AddLog(ServerLogs.Modules.Permissions, sender.LogName + " removed local group from player " + referenceHub.LoggedNameFromRefHub() + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
					}
					else
					{
						ServerLogs.AddLog(ServerLogs.Modules.Permissions, string.Concat(new string[]
						{
							sender.LogName,
							" set local group of player ",
							referenceHub.LoggedNameFromRefHub(),
							" to ",
							array[0],
							"."
						}), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
					}
					referenceHub.serverRoles.SetGroup(userGroup, true, false);
					num++;
				}
			}
			if (num2 == 0)
			{
				response = string.Format("Done! The request affected {0} player{1}", num, (num == 1) ? "!" : "s!");
				return true;
			}
			response = string.Format("Failed to execute the command! Failures: {0}\nError log:\n{1}", num2, text);
			return false;
		}
	}
}
