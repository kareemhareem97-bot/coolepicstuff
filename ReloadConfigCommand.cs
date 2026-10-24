using System;
using GameCore;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A07 RID: 2567
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class ReloadConfigCommand : ICommand
	{
		// Token: 0x17000C39 RID: 3129
		// (get) Token: 0x060039FE RID: 14846 RVA: 0x000BF2D5 File Offset: 0x000BD4D5
		public string Command { get; } = "reloadconfig";

		// Token: 0x17000C3A RID: 3130
		// (get) Token: 0x060039FF RID: 14847 RVA: 0x000BF2DD File Offset: 0x000BD4DD
		public string[] Aliases { get; } = new string[]
		{
			"rc"
		};

		// Token: 0x17000C3B RID: 3131
		// (get) Token: 0x06003A00 RID: 14848 RVA: 0x000BF2E5 File Offset: 0x000BD4E5
		public string Description { get; } = "Reloads all configs.";

		// Token: 0x06003A01 RID: 14849 RVA: 0x000BF2F0 File Offset: 0x000BD4F0
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.ServerConfigs, out response))
			{
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " reloaded configuration files.", ServerLogs.ServerLogType.RemoteAdminActivity_Misc, false);
			bool result;
			try
			{
				ConfigFile.ReloadGameConfigs(false);
				response = "Reloaded all configs!";
				result = true;
			}
			catch (Exception arg)
			{
				response = string.Format("Reloading configs failed: {0}", arg);
				result = false;
			}
			return result;
		}
	}
}
