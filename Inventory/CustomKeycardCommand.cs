using System;
using System.Collections.Generic;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Keycards;
using NorthwoodLib.Pools;
using UnityEngine;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin.Inventory
{
	// Token: 0x02000A53 RID: 2643
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class CustomKeycardCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000D22 RID: 3362
		// (get) Token: 0x06003BB6 RID: 15286 RVA: 0x000C4E66 File Offset: 0x000C3066
		public string Command { get; } = "customkeycard";

		// Token: 0x17000D23 RID: 3363
		// (get) Token: 0x06003BB7 RID: 15287 RVA: 0x000C4E6E File Offset: 0x000C306E
		public string[] Aliases { get; } = new string[]
		{
			"ckeycard"
		};

		// Token: 0x17000D24 RID: 3364
		// (get) Token: 0x06003BB8 RID: 15288 RVA: 0x000C4E76 File Offset: 0x000C3076
		public string Description { get; } = "Give player(s) a custom keycard.";

		// Token: 0x17000D25 RID: 3365
		// (get) Token: 0x06003BB9 RID: 15289 RVA: 0x000C4E7E File Offset: 0x000C307E
		public string[] Usage { get; } = new string[]
		{
			"%player%",
			"KeycardType"
		};

		// Token: 0x06003BBA RID: 15290 RVA: 0x000C4E88 File Offset: 0x000C3088
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.GivingItems, out response))
			{
				return false;
			}
			if (arguments.Count == 0)
			{
				response = "Please provide players to give the keycard to.";
				return false;
			}
			KeycardItem keycardItem;
			if (!CustomKeycardCommand.TryParseKeycard(arguments, out keycardItem))
			{
				response = "Please choose one of the following options as your second argument:";
				foreach (KeyValuePair<ItemType, ItemBase> keyValuePair in InventoryItemLoader.AvailableItems)
				{
					KeycardItem keycardItem2 = keyValuePair.Value as KeycardItem;
					if (keycardItem2 != null && keycardItem2.Customizable)
					{
						response = response + " " + keyValuePair.Key.ToString();
					}
				}
				response += ".";
				return false;
			}
			string[] array;
			List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out array, false);
			int num = 0;
			string text = "";
			DetailBase[] details = keycardItem.Details;
			for (int i = 0; i < details.Length; i++)
			{
				ICustomizableDetail customizableDetail = details[i] as ICustomizableDetail;
				if (customizableDetail != null)
				{
					foreach (string str in customizableDetail.CommandArguments)
					{
						text = text + "\n- " + str;
						num++;
					}
				}
			}
			if (array.Length + 1 < num)
			{
				ListPool<ReferenceHub>.Shared.Return(list);
				response = string.Format("{0} requires: {1}", keycardItem.ItemTypeId, text);
				return false;
			}
			int num2 = 0;
			foreach (DetailBase detailBase in keycardItem.Details)
			{
				ICustomizableDetail customizableDetail2 = detailBase as ICustomizableDetail;
				if (customizableDetail2 != null)
				{
					int num3 = customizableDetail2.CommandArguments.Length;
					try
					{
						ArraySegment<string> args = new ArraySegment<string>(array, num2 + 1, num3);
						customizableDetail2.ParseArguments(args);
					}
					catch (Exception ex)
					{
						ListPool<ReferenceHub>.Shared.Return(list);
						response = "Arguments failed to parse at: " + detailBase.GetType().Name + ".\n" + ex.Message;
						return false;
					}
					num2 += num3;
				}
			}
			int num4 = 0;
			int num5 = 0;
			string arg = string.Empty;
			foreach (ReferenceHub ply in list)
			{
				try
				{
					CustomKeycardCommand.AddItem(ply, sender, keycardItem.ItemTypeId);
					num5++;
				}
				catch (Exception ex2)
				{
					num4++;
					arg = ex2.Message;
				}
			}
			response = ((num4 == 0) ? string.Format("Done! The request affected {0} player{1}", num5, (num5 == 1) ? "!" : "s!") : string.Format("Failed to execute the command! Failures: {0}\nLast error log:\n{1}", num4, arg));
			ListPool<ReferenceHub>.Shared.Return(list);
			return true;
		}

		// Token: 0x06003BBB RID: 15291 RVA: 0x000C5164 File Offset: 0x000C3364
		private static bool TryParseKeycard(ArraySegment<string> args, out KeycardItem keycard)
		{
			if (args.Count < 2)
			{
				keycard = null;
				return false;
			}
			ItemType itemType;
			if (!Enum.TryParse<ItemType>(args.At(1), true, out itemType))
			{
				keycard = null;
				return false;
			}
			return itemType.TryGetTemplate(out keycard) && keycard.Customizable;
		}

		// Token: 0x06003BBC RID: 15292 RVA: 0x000C51A8 File Offset: 0x000C33A8
		private static void AddItem(ReferenceHub ply, ICommandSender sender, ItemType id)
		{
			UnityEngine.Object x = ply.inventory.ServerAddItem(id, ItemAddReason.AdminCommand, 0, null);
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} created {1} and gave it to {2}.", sender.LogName, id, ply.LoggedNameFromRefHub()), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			if (x == null)
			{
				throw new NullReferenceException(string.Format("Could not add {0}. Inventory is full or the item is not defined.", id));
			}
		}
	}
}
