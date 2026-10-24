using System;
using System.Collections.Generic;
using CommandSystem;
using UnityEngine;
using Utils;

namespace Christmas.Scp2536
{
	// Token: 0x020010C7 RID: 4295
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class TeleportTreeCommand : ICommand, IUsageProvider
	{
		// Token: 0x170013D0 RID: 5072
		// (get) Token: 0x06005FA6 RID: 24486 RVA: 0x0013FF09 File Offset: 0x0013E109
		public string Command { get; } = "tptree";

		// Token: 0x170013D1 RID: 5073
		// (get) Token: 0x06005FA7 RID: 24487 RVA: 0x0013FF11 File Offset: 0x0013E111
		public string[] Aliases { get; } = new string[]
		{
			"teleporttree"
		};

		// Token: 0x170013D2 RID: 5074
		// (get) Token: 0x06005FA8 RID: 24488 RVA: 0x0013FF19 File Offset: 0x0013E119
		public string Description { get; } = "Carpincho.";

		// Token: 0x170013D3 RID: 5075
		// (get) Token: 0x06005FA9 RID: 24489 RVA: 0x0013FF21 File Offset: 0x0013E121
		public string[] Usage { get; } = new string[]
		{
			"%player%"
		};

		// Token: 0x06005FAA RID: 24490 RVA: 0x0013FF2C File Offset: 0x0013E12C
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
			Vector3 position = list[0].transform.position;
			Scp2536Controller.Singleton.RpcMoveTree(position, Quaternion.identity, 0);
			Scp2536Controller.Singleton.GiftController.ServerPrepareGifts(true);
			return true;
		}
	}
}
