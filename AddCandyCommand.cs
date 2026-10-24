using System;
using System.Collections.Generic;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Usables.Scp330;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009F5 RID: 2549
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class AddCandyCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000BF6 RID: 3062
		// (get) Token: 0x06003993 RID: 14739 RVA: 0x000BD032 File Offset: 0x000BB232
		public string Command { get; } = "addcandy";

		// Token: 0x17000BF7 RID: 3063
		// (get) Token: 0x06003994 RID: 14740 RVA: 0x000BD03A File Offset: 0x000BB23A
		public string[] Aliases { get; } = new string[]
		{
			"candy",
			"trickortreat"
		};

		// Token: 0x17000BF8 RID: 3064
		// (get) Token: 0x06003995 RID: 14741 RVA: 0x000BD042 File Offset: 0x000BB242
		public string Description { get; } = "Adds specific candies to player(s) bags. If a player does not have a bag, it will create a new one with a random candy.";

		// Token: 0x17000BF9 RID: 3065
		// (get) Token: 0x06003996 RID: 14742 RVA: 0x000BD04A File Offset: 0x000BB24A
		public string[] Usage { get; } = new string[]
		{
			"%player%",
			"%item%"
		};

		// Token: 0x06003997 RID: 14743 RVA: 0x000BD054 File Offset: 0x000BB254
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
				response = "Could not process provided arguments.";
				return false;
			}
			string text = (array[0].ToUpper()[0].ToString() + array[0].ToLower()).Remove(1, 1);
			CandyKindID candyKindID;
			if (!Enum.TryParse<CandyKindID>(text, out candyKindID))
			{
				response = array[0] + " could not be parsed as candy type";
				return false;
			}
			if (!Enum.IsDefined(typeof(CandyKindID), candyKindID) || candyKindID == CandyKindID.None)
			{
				response = text + " isn't a valid candy type!";
				return false;
			}
			int num = 0;
			int num2 = 0;
			string arg = string.Empty;
			if (list != null)
			{
				foreach (ReferenceHub referenceHub in list)
				{
					try
					{
						ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} gave {1} candy to {2}.", sender.LogName, candyKindID, referenceHub.LoggedNameFromRefHub()), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
						referenceHub.GrantCandy(candyKindID, ItemAddReason.AdminCommand);
						num2++;
					}
					catch (Exception ex)
					{
						num++;
						arg = ex.Message;
					}
				}
			}
			response = ((num == 0) ? string.Format("Done! The request affected {0} player{1}", num2, (num2 == 1) ? "!" : "s!") : string.Format("Failed to execute the command! Failures: {0}\nLast error log:\n{1}", num, arg));
			return true;
		}
	}
}
