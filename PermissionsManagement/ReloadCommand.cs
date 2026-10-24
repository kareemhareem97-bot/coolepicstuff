using System;
using GameCore;

namespace CommandSystem.Commands.RemoteAdmin.PermissionsManagement
{
	// Token: 0x02000A42 RID: 2626
	[CommandHandler(typeof(PermissionsManagementCommand))]
	public class ReloadCommand : ICommand
	{
		// Token: 0x17000CE5 RID: 3301
		// (get) Token: 0x06003B55 RID: 15189 RVA: 0x000C3746 File Offset: 0x000C1946
		public string Command { get; } = "reload";

		// Token: 0x17000CE6 RID: 3302
		// (get) Token: 0x06003B56 RID: 15190 RVA: 0x000C374E File Offset: 0x000C194E
		public string[] Aliases { get; } = new string[]
		{
			"rl"
		};

		// Token: 0x17000CE7 RID: 3303
		// (get) Token: 0x06003B57 RID: 15191 RVA: 0x000C3756 File Offset: 0x000C1956
		public string Description { get; } = "Reloads the permissions file.";

		// Token: 0x06003B58 RID: 15192 RVA: 0x000C3760 File Offset: 0x000C1960
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PermissionsManagement, out response))
			{
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " reloaded permissions files.", ServerLogs.ServerLogType.RemoteAdminActivity_Misc, false);
			ConfigFile.ReloadGameConfigs(false);
			ServerStatic.RolesConfig.Reload();
			ServerStatic.SharedGroupsConfig = ((ConfigSharing.Paths[4] == null) ? null : new YamlConfig(ConfigSharing.Paths[4] + "shared_groups.txt"));
			ServerStatic.SharedGroupsMembersConfig = ((ConfigSharing.Paths[5] == null) ? null : new YamlConfig(ConfigSharing.Paths[5] + "shared_groups_members.txt"));
			ServerStatic.PermissionsHandler = new PermissionsHandler(ref ServerStatic.RolesConfig, ref ServerStatic.SharedGroupsConfig, ref ServerStatic.SharedGroupsMembersConfig);
			response = "Permissions file reloaded.";
			return true;
		}
	}
}
