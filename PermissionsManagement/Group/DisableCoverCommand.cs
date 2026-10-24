using System;

namespace CommandSystem.Commands.RemoteAdmin.PermissionsManagement.Group
{
	// Token: 0x02000A45 RID: 2629
	[CommandHandler(typeof(GroupCommand))]
	public class DisableCoverCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000CEF RID: 3311
		// (get) Token: 0x06003B65 RID: 15205 RVA: 0x000C3B0E File Offset: 0x000C1D0E
		public string Command { get; } = "disablecover";

		// Token: 0x17000CF0 RID: 3312
		// (get) Token: 0x06003B66 RID: 15206 RVA: 0x000C3B16 File Offset: 0x000C1D16
		public string[] Aliases { get; }

		// Token: 0x17000CF1 RID: 3313
		// (get) Token: 0x06003B67 RID: 15207 RVA: 0x000C3B1E File Offset: 0x000C1D1E
		public string Description { get; } = "Disables badge cover for a group.";

		// Token: 0x17000CF2 RID: 3314
		// (get) Token: 0x06003B68 RID: 15208 RVA: 0x000C3B26 File Offset: 0x000C1D26
		public string[] Usage { get; } = new string[]
		{
			"Group Name"
		};

		// Token: 0x06003B69 RID: 15209 RVA: 0x000C3B30 File Offset: 0x000C1D30
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
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " disabled cover for group " + text + ".", ServerLogs.ServerLogType.RemoteAdminActivity_Misc, false);
			ServerStatic.RolesConfig.SetString(text + "_cover", "false");
			ServerStatic.PermissionsHandler = new PermissionsHandler(ref ServerStatic.RolesConfig, ref ServerStatic.SharedGroupsConfig, ref ServerStatic.SharedGroupsMembersConfig);
			response = "Disabled cover for group " + text + ".";
			return true;
		}
	}
}
