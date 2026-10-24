using System;
using System.Linq;
using Mirror;
using PlayerRoles.Ragdolls;
using UnityEngine;

namespace CommandSystem.Commands.RemoteAdmin.Cleanup
{
	// Token: 0x02000A7A RID: 2682
	[CommandHandler(typeof(CleanupCommand))]
	public class CorpsesCommand : ICommand
	{
		// Token: 0x17000D95 RID: 3477
		// (get) Token: 0x06003C96 RID: 15510 RVA: 0x000C784E File Offset: 0x000C5A4E
		public string Command { get; } = "corpses";

		// Token: 0x17000D96 RID: 3478
		// (get) Token: 0x06003C97 RID: 15511 RVA: 0x000C7856 File Offset: 0x000C5A56
		public string[] Aliases { get; } = new string[]
		{
			"corpse",
			"ragdolls",
			"ragdoll",
			"r",
			"c",
			"0"
		};

		// Token: 0x17000D97 RID: 3479
		// (get) Token: 0x06003C98 RID: 15512 RVA: 0x000C785E File Offset: 0x000C5A5E
		public string Description { get; } = "Cleans up ragdolls from the map.";

		// Token: 0x06003C99 RID: 15513 RVA: 0x000C7868 File Offset: 0x000C5A68
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
			{
				return false;
			}
			BasicRagdoll[] array = (from r in UnityEngine.Object.FindObjectsOfType<BasicRagdoll>()
			orderby r.Info.CreationTime descending
			select r).ToArray<BasicRagdoll>();
			int num = array.Length;
			int num2;
			if (arguments.Count > 0 && int.TryParse(arguments.At(0), out num2) && num2 < array.Length)
			{
				num = num2;
			}
			for (int i = 0; i < num; i++)
			{
				NetworkServer.Destroy(array[i].gameObject);
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} has force-cleaned up {1} ragdolls.", sender.LogName, num), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			response = string.Format("{0} ragdolls have been deleted.", num);
			return true;
		}
	}
}
