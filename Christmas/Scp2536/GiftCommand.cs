using System;
using System.Collections.Generic;
using CommandSystem;
using Utils;

namespace Christmas.Scp2536
{
	// Token: 0x020010C6 RID: 4294
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class GiftCommand : ICommand, IUsageProvider
	{
		// Token: 0x170013CC RID: 5068
		// (get) Token: 0x06005F9F RID: 24479 RVA: 0x0013FD16 File Offset: 0x0013DF16
		public string Command { get; } = "gift";

		// Token: 0x170013CD RID: 5069
		// (get) Token: 0x06005FA0 RID: 24480 RVA: 0x0013FD1E File Offset: 0x0013DF1E
		public string[] Aliases { get; } = new string[]
		{
			"addgift"
		};

		// Token: 0x170013CE RID: 5070
		// (get) Token: 0x06005FA1 RID: 24481 RVA: 0x0013FD26 File Offset: 0x0013DF26
		public string Description { get; } = "Grant a gift to a player.";

		// Token: 0x170013CF RID: 5071
		// (get) Token: 0x06005FA2 RID: 24482 RVA: 0x0013FD2E File Offset: 0x0013DF2E
		public string[] Usage { get; } = new string[]
		{
			"%player%",
			"Gift name (Optional)"
		};

		// Token: 0x06005FA3 RID: 24483 RVA: 0x0013FD38 File Offset: 0x0013DF38
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.GivingItems, out response))
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
			if (list == null)
			{
				response = "Cannot find player! Try using the player ID!";
				return false;
			}
			Scp2536GiftBase scp2536GiftBase = null;
			bool flag = array.Length != 0 && this.TryFetchGift(array[0].ToUpper(), out scp2536GiftBase);
			int num = 0;
			bool flag2 = false;
			foreach (ReferenceHub hub in list)
			{
				num++;
				if (flag)
				{
					flag2 = scp2536GiftBase.CanBeGranted(hub);
					scp2536GiftBase.ServerGrant(hub);
				}
				else
				{
					Scp2536Controller.Singleton.GiftController.ServerGrantRandomGift(hub);
				}
			}
			response = "Gift granted!";
			if (flag2 && num > 1)
			{
				response += " Forcefully, as the target didn't meet the expected conditions.";
			}
			return true;
		}

		// Token: 0x06005FA4 RID: 24484 RVA: 0x0013FE40 File Offset: 0x0013E040
		private bool TryFetchGift(string input, out Scp2536GiftBase giftBase)
		{
			foreach (Scp2536GiftBase scp2536GiftBase in Scp2536GiftController.Gifts)
			{
				if (scp2536GiftBase.GetType().Name.ToUpper().Contains(input))
				{
					giftBase = scp2536GiftBase;
					return true;
				}
			}
			giftBase = null;
			return false;
		}
	}
}
