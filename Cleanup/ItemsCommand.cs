using System;
using InventorySystem.Items.Pickups;
using Mirror;
using UnityEngine;

namespace CommandSystem.Commands.RemoteAdmin.Cleanup
{
	// Token: 0x02000A80 RID: 2688
	[CommandHandler(typeof(CleanupCommand))]
	public class ItemsCommand : ICommand
	{
		// Token: 0x17000D98 RID: 3480
		// (get) Token: 0x06003CA8 RID: 15528 RVA: 0x000C7A65 File Offset: 0x000C5C65
		public string Command { get; } = "items";

		// Token: 0x17000D99 RID: 3481
		// (get) Token: 0x06003CA9 RID: 15529 RVA: 0x000C7A6D File Offset: 0x000C5C6D
		public string[] Aliases { get; } = new string[]
		{
			"item",
			"i",
			"1"
		};

		// Token: 0x17000D9A RID: 3482
		// (get) Token: 0x06003CAA RID: 15530 RVA: 0x000C7A75 File Offset: 0x000C5C75
		public string Description { get; } = "Cleans up items from the map.";

		// Token: 0x06003CAB RID: 15531 RVA: 0x000C7A80 File Offset: 0x000C5C80
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
			{
				return false;
			}
			ItemPickupBase[] array = UnityEngine.Object.FindObjectsOfType<ItemPickupBase>();
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
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} has force-cleaned up {1} items.", sender.LogName, num), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			response = string.Format("{0} items have been deleted.", num);
			return true;
		}
	}
}
