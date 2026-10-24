using System;
using System.Collections.Generic;
using InventorySystem.Items;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin.Inventory
{
	// Token: 0x02000A54 RID: 2644
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class ForceEquipCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000D26 RID: 3366
		// (get) Token: 0x06003BBE RID: 15294 RVA: 0x000C5261 File Offset: 0x000C3461
		public string Command { get; } = "forceequip";

		// Token: 0x17000D27 RID: 3367
		// (get) Token: 0x06003BBF RID: 15295 RVA: 0x000C5269 File Offset: 0x000C3469
		public string[] Aliases { get; } = new string[]
		{
			"fequip",
			"forceeq"
		};

		// Token: 0x17000D28 RID: 3368
		// (get) Token: 0x06003BC0 RID: 15296 RVA: 0x000C5271 File Offset: 0x000C3471
		public string Description { get; } = "Forces player to equip item of provided type (if any exists in the inventory).";

		// Token: 0x17000D29 RID: 3369
		// (get) Token: 0x06003BC1 RID: 15297 RVA: 0x000C5279 File Offset: 0x000C3479
		public string[] Usage { get; } = new string[]
		{
			"%player%",
			"%item%"
		};

		// Token: 0x06003BC2 RID: 15298 RVA: 0x000C5284 File Offset: 0x000C3484
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.GivingItems, out response))
			{
				return false;
			}
			if (arguments.Count == 0)
			{
				response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			string[] array;
			List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out array, false);
			ItemType itemType = ItemType.None;
			if (array != null && array.Length != 0)
			{
				string text = array[0];
				if (text.Contains('.'))
				{
					text = text.Split('.', StringSplitOptions.None)[0];
				}
				int num;
				if (int.TryParse(text, out num) && Enum.IsDefined(typeof(ItemType), num))
				{
					itemType = (ItemType)num;
				}
			}
			int num2 = 0;
			int num3 = 0;
			string empty = string.Empty;
			if (list != null)
			{
				Func<ReferenceHub, ItemType, bool> func = (itemType == ItemType.None) ? new Func<ReferenceHub, ItemType, bool>(this.TryHolster) : new Func<ReferenceHub, ItemType, bool>(this.TryEquip);
				foreach (ReferenceHub arg in list)
				{
					if (func(arg, itemType))
					{
						num3++;
					}
				}
			}
			response = ((num2 == 0) ? string.Format("Done! The request affected {0} player{1}", num3, (num3 == 1) ? "!" : "s!") : string.Format("Failed to execute the command! Failures: {0}\nLast error log:\n{1}", num2, empty));
			return true;
		}

		// Token: 0x06003BC3 RID: 15299 RVA: 0x000C53DC File Offset: 0x000C35DC
		private bool TryEquip(ReferenceHub hub, ItemType itemToEquip)
		{
			foreach (KeyValuePair<ushort, ItemBase> keyValuePair in hub.inventory.UserInventory.Items)
			{
				if (keyValuePair.Value.ItemTypeId == itemToEquip)
				{
					hub.inventory.ServerSelectItem(keyValuePair.Key);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003BC4 RID: 15300 RVA: 0x000C545C File Offset: 0x000C365C
		private bool TryHolster(ReferenceHub hub, ItemType parsedItem)
		{
			if (hub.inventory.CurItem.SerialNumber == 0)
			{
				return false;
			}
			hub.inventory.ServerSelectItem(0);
			return false;
		}
	}
}
