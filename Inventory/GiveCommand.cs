using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem;
using InventorySystem.Items;
using UnityEngine;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin.Inventory
{
	// Token: 0x02000A55 RID: 2645
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class GiveCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000D2A RID: 3370
		// (get) Token: 0x06003BC6 RID: 15302 RVA: 0x000C54E1 File Offset: 0x000C36E1
		public string Command { get; } = "give";

		// Token: 0x17000D2B RID: 3371
		// (get) Token: 0x06003BC7 RID: 15303 RVA: 0x000C54E9 File Offset: 0x000C36E9
		public string[] Aliases { get; }

		// Token: 0x17000D2C RID: 3372
		// (get) Token: 0x06003BC8 RID: 15304 RVA: 0x000C54F1 File Offset: 0x000C36F1
		public string Description { get; } = "Give player(s) a specified item.";

		// Token: 0x17000D2D RID: 3373
		// (get) Token: 0x06003BC9 RID: 15305 RVA: 0x000C54F9 File Offset: 0x000C36F9
		public string[] Usage { get; } = new string[]
		{
			"%player%",
			"%item%"
		};

		// Token: 0x06003BCA RID: 15306 RVA: 0x000C5504 File Offset: 0x000C3704
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
			ItemType[] array2 = GiveCommand.ParseItems(array[0]).ToArray<ItemType>();
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
							GiveCommand.AddItem(ply, sender, id);
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

		// Token: 0x06003BCB RID: 15307 RVA: 0x000C5654 File Offset: 0x000C3854
		private static IEnumerable<ItemType> ParseItems(string argument)
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

		// Token: 0x06003BCC RID: 15308 RVA: 0x000C5664 File Offset: 0x000C3864
		private static void AddItem(ReferenceHub ply, ICommandSender sender, ItemType id)
		{
			UnityEngine.Object x = ply.inventory.ServerAddItem(id, ItemAddReason.AdminCommand, 0, null);
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} gave {1} to {2}.", sender.LogName, id, ply.LoggedNameFromRefHub()), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			if (x == null)
			{
				throw new NullReferenceException(string.Format("Could not add {0}. Inventory is full or the item is not defined.", id));
			}
		}
	}
}
