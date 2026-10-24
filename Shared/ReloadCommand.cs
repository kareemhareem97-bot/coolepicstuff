using System;
using GameCore;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009AD RID: 2477
	[CommandHandler(typeof(ConfigCommand))]
	public class ReloadCommand : ICommand
	{
		// Token: 0x17000B1B RID: 2843
		// (get) Token: 0x06003809 RID: 14345 RVA: 0x000B61F4 File Offset: 0x000B43F4
		public string Command { get; } = "reload";

		// Token: 0x17000B1C RID: 2844
		// (get) Token: 0x0600380A RID: 14346 RVA: 0x000B61FC File Offset: 0x000B43FC
		public string[] Aliases { get; } = new string[]
		{
			"r",
			"rld"
		};

		// Token: 0x17000B1D RID: 2845
		// (get) Token: 0x0600380B RID: 14347 RVA: 0x000B6204 File Offset: 0x000B4404
		public string Description { get; } = "Reloads the games config";

		// Token: 0x0600380C RID: 14348 RVA: 0x000B620C File Offset: 0x000B440C
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.ServerConfigs, out response))
			{
				return false;
			}
			try
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " reloaded configuration and permissions files.", ServerLogs.ServerLogType.RemoteAdminActivity_Misc, false);
				ConfigFile.ReloadGameConfigs(false);
				ServerStatic.RolesConfig = new YamlConfig(ServerStatic.RolesConfigPath ?? (FileManager.GetAppFolder(true, true, "") + "config_remoteadmin.txt"));
				ServerStatic.SharedGroupsConfig = ((ConfigSharing.Paths[4] == null) ? null : new YamlConfig(ConfigSharing.Paths[4] + "shared_groups.txt"));
				ServerStatic.SharedGroupsMembersConfig = ((ConfigSharing.Paths[5] == null) ? null : new YamlConfig(ConfigSharing.Paths[5] + "shared_groups_members.txt"));
				ServerStatic.PermissionsHandler = new PermissionsHandler(ref ServerStatic.RolesConfig, ref ServerStatic.SharedGroupsConfig, ref ServerStatic.SharedGroupsMembersConfig);
			}
			catch (Exception arg)
			{
				response = string.Format("Failed to reload the configuration file. Error: {0}", arg);
				return false;
			}
			response = "Configuration file successfully reloaded. Some of the changes will be applied in the next round.";
			return true;
		}
	}
}
