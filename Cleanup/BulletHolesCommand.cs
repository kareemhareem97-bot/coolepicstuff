using System;
using Decals;
using Utils.Networking;

namespace CommandSystem.Commands.RemoteAdmin.Cleanup
{
	// Token: 0x02000A78 RID: 2680
	[CommandHandler(typeof(CleanupCommand))]
	public class BulletHolesCommand : ICommand
	{
		// Token: 0x17000D8E RID: 3470
		// (get) Token: 0x06003C89 RID: 15497 RVA: 0x000C76B6 File Offset: 0x000C58B6
		public string Command { get; } = "bulletholes";

		// Token: 0x17000D8F RID: 3471
		// (get) Token: 0x06003C8A RID: 15498 RVA: 0x000C76BE File Offset: 0x000C58BE
		public string[] Aliases { get; } = new string[]
		{
			"bh",
			"bullets"
		};

		// Token: 0x17000D90 RID: 3472
		// (get) Token: 0x06003C8B RID: 15499 RVA: 0x000C76C6 File Offset: 0x000C58C6
		public string Description { get; } = "Cleans up bulled holes from the map.";

		// Token: 0x06003C8C RID: 15500 RVA: 0x000C76D0 File Offset: 0x000C58D0
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
			{
				return false;
			}
			string s;
			int maxValue;
			if (!arguments.Array.TryGet(2, out s) || !int.TryParse(s, out maxValue))
			{
				maxValue = int.MaxValue;
			}
			foreach (DecalPoolType decalPoolType in EnumUtils<DecalPoolType>.Values)
			{
				if (decalPoolType != DecalPoolType.None && decalPoolType != DecalPoolType.Blood)
				{
					new DecalCleanupMessage(decalPoolType, maxValue).SendToAuthenticated(0);
				}
			}
			response = "Cleaned up bullet holes!";
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " has force-cleaned up bullet holes.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			return true;
		}
	}
}
