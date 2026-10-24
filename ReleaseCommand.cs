using System;
using System.Collections.Generic;
using System.Text;
using InventorySystem.Disarming;
using NorthwoodLib.Pools;
using Utils;
using Utils.Networking;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A06 RID: 2566
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class ReleaseCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000C35 RID: 3125
		// (get) Token: 0x060039F8 RID: 14840 RVA: 0x000BF125 File Offset: 0x000BD325
		public string Command { get; } = "release";

		// Token: 0x17000C36 RID: 3126
		// (get) Token: 0x060039F9 RID: 14841 RVA: 0x000BF12D File Offset: 0x000BD32D
		public string[] Aliases { get; } = new string[]
		{
			"free"
		};

		// Token: 0x17000C37 RID: 3127
		// (get) Token: 0x060039FA RID: 14842 RVA: 0x000BF135 File Offset: 0x000BD335
		public string Description { get; } = "Releases the specified disarmed player(s).";

		// Token: 0x17000C38 RID: 3128
		// (get) Token: 0x060039FB RID: 14843 RVA: 0x000BF13D File Offset: 0x000BD33D
		public string[] Usage { get; } = new string[]
		{
			"%player%"
		};

		// Token: 0x060039FC RID: 14844 RVA: 0x000BF148 File Offset: 0x000BD348
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
				response = "An unexpected problem has occurred during PlayerId array processing.";
				return false;
			}
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			int num = 0;
			foreach (ReferenceHub referenceHub in list)
			{
				if (!(referenceHub == null))
				{
					num++;
					referenceHub.inventory.SetDisarmedStatus(null);
					new DisarmedPlayersListMessage(DisarmedPlayers.Entries).SendToAuthenticated(0);
					if (num != 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(referenceHub.LoggedNameFromRefHub());
				}
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} administratively released {1}.", sender.LogName, stringBuilder), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			StringBuilderPool.Shared.Return(stringBuilder);
			response = string.Format("Done! The request affected {0} player{1}", num, (num == 1) ? "!" : "s!");
			return true;
		}
	}
}
