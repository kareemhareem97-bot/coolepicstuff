using System;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009AC RID: 2476
	[CommandHandler(typeof(ConfigCommand))]
	public class PathCommand : ICommand
	{
		// Token: 0x17000B18 RID: 2840
		// (get) Token: 0x06003804 RID: 14340 RVA: 0x000B618D File Offset: 0x000B438D
		public string Command { get; } = "path";

		// Token: 0x17000B19 RID: 2841
		// (get) Token: 0x06003805 RID: 14341 RVA: 0x000B6195 File Offset: 0x000B4395
		public string[] Aliases { get; }

		// Token: 0x17000B1A RID: 2842
		// (get) Token: 0x06003806 RID: 14342 RVA: 0x000B619D File Offset: 0x000B439D
		public string Description { get; } = "Returns the path to the config file";

		// Token: 0x06003807 RID: 14343 RVA: 0x000B61A5 File Offset: 0x000B43A5
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.ServerConfigs, out response))
			{
				return false;
			}
			response = "Configuration file path: <i>" + FileManager.GetAppFolder(true, true, "") + "</i>";
			return true;
		}
	}
}
