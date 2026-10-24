using System;
using Decals;
using Utils.Networking;

namespace CommandSystem.Commands.RemoteAdmin.Cleanup
{
	// Token: 0x02000A77 RID: 2679
	[CommandHandler(typeof(CleanupCommand))]
	public class BloodCommand : ICommand
	{
		// Token: 0x17000D8B RID: 3467
		// (get) Token: 0x06003C84 RID: 15492 RVA: 0x000C75FD File Offset: 0x000C57FD
		public string Command { get; } = "blood";

		// Token: 0x17000D8C RID: 3468
		// (get) Token: 0x06003C85 RID: 15493 RVA: 0x000C7605 File Offset: 0x000C5805
		public string[] Aliases { get; } = new string[]
		{
			"bl"
		};

		// Token: 0x17000D8D RID: 3469
		// (get) Token: 0x06003C86 RID: 15494 RVA: 0x000C760D File Offset: 0x000C580D
		public string Description { get; } = "Cleans up blood from the map.";

		// Token: 0x06003C87 RID: 15495 RVA: 0x000C7618 File Offset: 0x000C5818
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
			new DecalCleanupMessage(DecalPoolType.Blood, maxValue).SendToAuthenticated(0);
			response = "Cleaned up blood!";
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " has force-cleaned up blood.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			return true;
		}
	}
}
