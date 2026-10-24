using System;
using PlayerRoles.PlayableScps.Scp3114;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A19 RID: 2585
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class Scp3114HistoryCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000C71 RID: 3185
		// (get) Token: 0x06003A6E RID: 14958 RVA: 0x000C0C45 File Offset: 0x000BEE45
		public string Command { get; } = "scp3114history";

		// Token: 0x17000C72 RID: 3186
		// (get) Token: 0x06003A6F RID: 14959 RVA: 0x000C0C4D File Offset: 0x000BEE4D
		public string[] Aliases { get; } = new string[]
		{
			"3114history",
			"history3114",
			"historyscp3114"
		};

		// Token: 0x17000C73 RID: 3187
		// (get) Token: 0x06003A70 RID: 14960 RVA: 0x000C0C55 File Offset: 0x000BEE55
		public string Description { get; } = "Prints identity history of SCP-3114.";

		// Token: 0x17000C74 RID: 3188
		// (get) Token: 0x06003A71 RID: 14961 RVA: 0x000C0C5D File Offset: 0x000BEE5D
		public string[] Usage { get; } = new string[]
		{
			"Instance ID (if multiple recorded)"
		};

		// Token: 0x06003A72 RID: 14962 RVA: 0x000C0C68 File Offset: 0x000BEE68
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
			{
				return false;
			}
			if (arguments.Count == 0)
			{
				response = Scp3114History.PrintHistory(null);
			}
			else
			{
				int value;
				if (!int.TryParse(arguments.At(0), out value))
				{
					value = -1;
				}
				response = Scp3114History.PrintHistory(new int?(value));
			}
			return true;
		}
	}
}
