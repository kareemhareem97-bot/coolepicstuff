using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms.Ammo;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin.Inventory
{
	// Token: 0x02000A57 RID: 2647
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class RemoveItemCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000D30 RID: 3376
		// (get) Token: 0x06003BD6 RID: 15318 RVA: 0x000C5827 File Offset: 0x000C3A27
		public string Command { get; } = "removeitem";

		// Token: 0x17000D31 RID: 3377
		// (get) Token: 0x06003BD7 RID: 15319 RVA: 0x000C582F File Offset: 0x000C3A2F
		public string[] Aliases { get; }

		// Token: 0x17000D32 RID: 3378
		// (get) Token: 0x06003BD8 RID: 15320 RVA: 0x000C5837 File Offset: 0x000C3A37
		public string Description { get; } = "Remove the specified item from the player(s) inventory.";

		// Token: 0x17000D33 RID: 3379
		// (get) Token: 0x06003BD9 RID: 15321 RVA: 0x000C583F File Offset: 0x000C3A3F
		public string[] Usage { get; } = new string[]
		{
			"%player%",
			"%item%"
		};

		// Token: 0x06003BDA RID: 15322 RVA: 0x000C5848 File Offset: 0x000C3A48
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.GivingItems, out response))
			{
				return false;
			}
			if (arguments.Count < 2)
			{
				response = "To execute this command provide at least 2 arguments!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			string[] array;
			List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out array, false);
			if (array == null || array.Length == 0)
			{
				response = "You must specify item(s) to give.";
				return false;
			}
			ItemType[] array2 = this.ParseItems(array[0]).ToArray<ItemType>();
			if (array2.Length == 0)
			{
				response = "You didn't input any items.";
				return false;
			}
			int num = 0;
			int num2 = 0;
			string arg = string.Empty;
			if (list != null)
			{
				foreach (ReferenceHub ply in list)
				{
					try
					{
						foreach (ItemType id in array2)
						{
							this.RemoveItem(ply, sender, id);
						}
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
			response = ((num == 0) ? string.Format("Done! The request affected {0} player{1}", num2, (num2 == 1) ? "!" : "s!") : string.Format("Failed to execute the command! Failures: {0}\nLast error log:\n{1}", num, arg));
			return true;
		}

		// Token: 0x06003BDB RID: 15323 RVA: 0x000C599C File Offset: 0x000C3B9C
		private IEnumerable<ItemType> ParseItems(string argument)
		{
			string[] array = argument.Split('.', StringSplitOptions.None);
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				int num;
				if (int.TryParse(array2[i], out num) && Enum.IsDefined(typeof(ItemType), num))
				{
					yield return (ItemType)num;
				}
			}
			array2 = null;
			yield break;
		}

		// Token: 0x06003BDC RID: 15324 RVA: 0x000C59AC File Offset: 0x000C3BAC
		private void RemoveItem(ReferenceHub ply, ICommandSender sender, ItemType id)
		{
			ItemBase itemBase = InventoryItemLoader.AvailableItems[id];
			bool flag = itemBase.Category != ItemCategory.Ammo;
			KeyValuePair<ushort, ItemBase> keyValuePair = ply.inventory.UserInventory.Items.FirstOrDefault((KeyValuePair<ushort, ItemBase> i) => i.Value.ItemTypeId == id);
			if (keyValuePair.Value == null && flag)
			{
				return;
			}
			if (flag)
			{
				ply.inventory.ServerRemoveItem(keyValuePair.Key, null);
			}
			else
			{
				AmmoPickup ammoPickup = itemBase.PickupDropModel as AmmoPickup;
				int curAmmo = (int)ply.inventory.GetCurAmmo(itemBase.ItemTypeId);
				ply.inventory.ServerSetAmmo(itemBase.ItemTypeId, curAmmo - (int)ammoPickup.SavedAmmo);
				itemBase.OnRemoved(itemBase.PickupDropModel);
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} removed item {1} from {2}.", sender.LogName, id, ply.LoggedNameFromRefHub()), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
		}
	}
}
