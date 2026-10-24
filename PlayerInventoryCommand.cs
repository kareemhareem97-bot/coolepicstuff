using System;
using System.Collections.Generic;
using System.Text;
using InventorySystem.Items;
using NorthwoodLib.Pools;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009F6 RID: 2550
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class PlayerInventoryCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000BFA RID: 3066
		// (get) Token: 0x06003999 RID: 14745 RVA: 0x000BD269 File Offset: 0x000BB469
		public string Command { get; } = "playerinventory";

		// Token: 0x17000BFB RID: 3067
		// (get) Token: 0x0600399A RID: 14746 RVA: 0x000BD271 File Offset: 0x000BB471
		public string[] Aliases { get; } = new string[]
		{
			"playerinv",
			"pinv",
			"pinventory"
		};

		// Token: 0x17000BFC RID: 3068
		// (get) Token: 0x0600399B RID: 14747 RVA: 0x000BD279 File Offset: 0x000BB479
		public string Description { get; } = "Displays players inventory.";

		// Token: 0x17000BFD RID: 3069
		// (get) Token: 0x0600399C RID: 14748 RVA: 0x000BD281 File Offset: 0x000BB481
		public string[] Usage { get; } = new string[]
		{
			"%player%"
		};

		// Token: 0x0600399D RID: 14749 RVA: 0x000BD28C File Offset: 0x000BB48C
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
			{
				return false;
			}
			if (arguments.Count < 1)
			{
				response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			string[] array;
			List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out array, false);
			if (list == null || list.Count == 0)
			{
				response = "Cannot find player with id or name: " + arguments.At(0);
				return false;
			}
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			foreach (ReferenceHub referenceHub in list)
			{
				string text = referenceHub.LoggedNameFromRefHub();
				ServerLogs.AddLog(ServerLogs.Modules.DataAccess, sender.LogName + " displayed the inventory of player " + text + ".", ServerLogs.ServerLogType.RemoteAdminActivity_Misc, false);
				stringBuilder.AppendFormat("Inventory of {0}:", text);
				foreach (KeyValuePair<ushort, ItemBase> keyValuePair in referenceHub.inventory.UserInventory.Items)
				{
					stringBuilder.AppendFormat("\n - {0}", keyValuePair.Value.ItemTypeId);
				}
				stringBuilder.AppendLine();
			}
			response = stringBuilder.ToString().Trim();
			StringBuilderPool.Shared.Return(stringBuilder);
			return true;
		}
	}
}
