using System;

namespace CommandSystem.Commands.RemoteAdmin.PermissionsManagement
{
	// Token: 0x02000A43 RID: 2627
	[CommandHandler(typeof(PermissionsManagementCommand))]
	public class SetGroupCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000CE8 RID: 3304
		// (get) Token: 0x06003B5A RID: 15194 RVA: 0x000C3848 File Offset: 0x000C1A48
		public string Command { get; } = "setgroup";

		// Token: 0x17000CE9 RID: 3305
		// (get) Token: 0x06003B5B RID: 15195 RVA: 0x000C3850 File Offset: 0x000C1A50
		public string[] Aliases { get; }

		// Token: 0x17000CEA RID: 3306
		// (get) Token: 0x06003B5C RID: 15196 RVA: 0x000C3858 File Offset: 0x000C1A58
		public string Description { get; } = "Sets a player's group. (Use \"-1\" as the group name to remove a group.)";

		// Token: 0x17000CEB RID: 3307
		// (get) Token: 0x06003B5D RID: 15197 RVA: 0x000C3860 File Offset: 0x000C1A60
		public string[] Usage { get; } = new string[]
		{
			"UserId",
			"Group (-1 to remove)"
		};

		// Token: 0x06003B5E RID: 15198 RVA: 0x000C3868 File Offset: 0x000C1A68
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PermissionsManagement, out response))
			{
				return false;
			}
			if (arguments.Count < 2)
			{
				response = "To execute this command provide at least 2 arguments!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			string text = arguments.At(1);
			if (text == "-1")
			{
				text = null;
			}
			else if (ServerStatic.PermissionsHandler.GetGroup(text) == null)
			{
				response = "Group can't be found.";
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Concat(new string[]
			{
				sender.LogName,
				" set server group of ",
				arguments.At(0),
				" to ",
				text,
				"."
			}), ServerLogs.ServerLogType.RemoteAdminActivity_Misc, false);
			ServerStatic.RolesConfig.SetStringDictionaryItem("Members", arguments.At(0), text);
			ServerStatic.PermissionsHandler = new PermissionsHandler(ref ServerStatic.RolesConfig, ref ServerStatic.SharedGroupsConfig, ref ServerStatic.SharedGroupsMembersConfig);
			response = "User permissions updated. If user is online, please use \"setgroup\" command to change it now (without this command, new role will be applied during next round).";
			return true;
		}
	}
}
