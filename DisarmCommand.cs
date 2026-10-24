using System;
using System.Collections.Generic;
using System.Text;
using InventorySystem;
using InventorySystem.Disarming;
using NorthwoodLib.Pools;
using Utils;
using Utils.Networking;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009DF RID: 2527
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class DisarmCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000BB2 RID: 2994
		// (get) Token: 0x0600390E RID: 14606 RVA: 0x000BA1C9 File Offset: 0x000B83C9
		public string Command { get; } = "disarm";

		// Token: 0x17000BB3 RID: 2995
		// (get) Token: 0x0600390F RID: 14607 RVA: 0x000BA1D1 File Offset: 0x000B83D1
		public string[] Aliases { get; } = new string[]
		{
			"da"
		};

		// Token: 0x17000BB4 RID: 2996
		// (get) Token: 0x06003910 RID: 14608 RVA: 0x000BA1D9 File Offset: 0x000B83D9
		public string Description { get; } = "Force disarming player(s), no matter if they have been cuffed before. They can be released only with RELEASE command.";

		// Token: 0x17000BB5 RID: 2997
		// (get) Token: 0x06003911 RID: 14609 RVA: 0x000BA1E1 File Offset: 0x000B83E1
		public string[] Usage { get; } = new string[]
		{
			"%player%"
		};

		// Token: 0x06003912 RID: 14610 RVA: 0x000BA1EC File Offset: 0x000B83EC
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
			if (list == null)
			{
				response = "An unexpected problem has occurred during PlayerId or name array processing.";
				return false;
			}
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			int num = 0;
			foreach (ReferenceHub referenceHub in list)
			{
				if (!(referenceHub == null))
				{
					DisarmingHandlers.InvokeOnPlayerDisarmed(null, referenceHub);
					referenceHub.inventory.SetDisarmedStatus(null);
					referenceHub.inventory.ServerDropEverything();
					DisarmedPlayers.Entries.Add(new DisarmedPlayers.DisarmedEntry(referenceHub.networkIdentity.netId, 0U));
					new DisarmedPlayersListMessage(DisarmedPlayers.Entries).SendToAuthenticated(0);
					if (num != 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(referenceHub.LoggedNameFromRefHub());
					num++;
				}
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} administratively disarmed {1}.", sender.LogName, stringBuilder), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			StringBuilderPool.Shared.Return(stringBuilder);
			response = string.Format("Done! The request affected {0} player{1}", num, (num == 1) ? "!" : "s!");
			return true;
		}
	}
}
