using System;

namespace CommandSystem.Commands.RemoteAdmin.PermissionsManagement.Group
{
	// Token: 0x02000A46 RID: 2630
	[CommandHandler(typeof(GroupCommand))]
	public class EnableCoverCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000CF3 RID: 3315
		// (get) Token: 0x06003B6B RID: 15211 RVA: 0x000C3C2E File Offset: 0x000C1E2E
		public string Command { get; } = "enablecover";

		// Token: 0x17000CF4 RID: 3316
		// (get) Token: 0x06003B6C RID: 15212 RVA: 0x000C3C36 File Offset: 0x000C1E36
		public string[] Aliases { get; }

		// Token: 0x17000CF5 RID: 3317
		// (get) Token: 0x06003B6D RID: 15213 RVA: 0x000C3C3E File Offset: 0x000C1E3E
		public string Description { get; } = "Enables badge cover for a group.";

		// Token: 0x17000CF6 RID: 3318
		// (get) Token: 0x06003B6E RID: 15214 RVA: 0x000C3C46 File Offset: 0x000C1E46
		public string[] Usage { get; } = new string[]
		{
			"Group Name"
		};

		// Token: 0x06003B6F RID: 15215 RVA: 0x000C3C50 File Offset: 0x000C1E50
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PermissionsManagement, out response))
			{
				return false;
			}
			if (arguments.Count < 1)
			{
				response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			string text = arguments.At(0);
			if (ServerStatic.PermissionsHandler.GetGroup(text) == null)
			{
				response = "Group can't be found.";
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " enabled cover for group " + text + ".", ServerLogs.ServerLogType.RemoteAdminActivity_Misc, false);
			ServerStatic.RolesConfig.SetString(text + "_cover", "true");
			ServerStatic.PermissionsHandler = new PermissionsHandler(ref ServerStatic.RolesConfig, ref ServerStatic.SharedGroupsConfig, ref ServerStatic.SharedGroupsMembersConfig);
			response = "Enabled cover for group " + text + ".";
			return true;
		}
	}
}
