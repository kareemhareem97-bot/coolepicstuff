using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem;
using InventorySystem.Items;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009F7 RID: 2551
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class StripCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000BFE RID: 3070
		// (get) Token: 0x0600399F RID: 14751 RVA: 0x000BD465 File Offset: 0x000BB665
		public string Command { get; } = "strip";

		// Token: 0x17000BFF RID: 3071
		// (get) Token: 0x060039A0 RID: 14752 RVA: 0x000BD46D File Offset: 0x000BB66D
		public string[] Aliases { get; } = new string[]
		{
			"clear"
		};

		// Token: 0x17000C00 RID: 3072
		// (get) Token: 0x060039A1 RID: 14753 RVA: 0x000BD475 File Offset: 0x000BB675
		public string Description { get; } = "Clears the specified player(s) inventory.";

		// Token: 0x17000C01 RID: 3073
		// (get) Token: 0x060039A2 RID: 14754 RVA: 0x000BD47D File Offset: 0x000BB67D
		public string[] Usage { get; } = new string[]
		{
			"%player%"
		};

		// Token: 0x060039A3 RID: 14755 RVA: 0x000BD488 File Offset: 0x000BB688
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
			{
				return false;
			}
			if (arguments.Count >= 1)
			{
				string[] array;
				List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out array, false);
				int num = 0;
				int num2 = 0;
				string arg = "";
				if (list != null)
				{
					foreach (ReferenceHub referenceHub in list)
					{
						try
						{
							InventoryInfo userInventory = referenceHub.inventory.UserInventory;
							while (userInventory.Items.Count > 0)
							{
								referenceHub.inventory.ServerRemoveItem(userInventory.Items.ElementAt(0).Key, null);
							}
							userInventory.ReserveAmmo.Clear();
							referenceHub.inventory.SendAmmoNextFrame = true;
							ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " cleared " + referenceHub.LoggedNameFromRefHub() + "'s inventory.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
						}
						catch (Exception ex)
						{
							num++;
							arg = ex.Message;
							continue;
						}
						num2++;
					}
				}
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} ran the STRIP command on {1} players.", sender.LogName, list.Count), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
				response = ((num == 0) ? string.Format("Done! The request affected {0} player{1}", num2, (num2 == 1) ? "!" : "s!") : string.Format("Failed to execute the command! Failures: {0}\nLast error log:\n {1}", num, arg));
				return true;
			}
			response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
			return false;
		}
	}
}
