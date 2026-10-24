using System;
using System.Collections.Generic;
using System.Text;
using NorthwoodLib.Pools;
using PlayerStatsSystem;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A1C RID: 2588
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class SetHealthCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000C7D RID: 3197
		// (get) Token: 0x06003A80 RID: 14976 RVA: 0x000C1131 File Offset: 0x000BF331
		public string Command { get; } = "hp";

		// Token: 0x17000C7E RID: 3198
		// (get) Token: 0x06003A81 RID: 14977 RVA: 0x000C1139 File Offset: 0x000BF339
		public string[] Aliases { get; } = new string[]
		{
			"sethealth",
			"sethp"
		};

		// Token: 0x17000C7F RID: 3199
		// (get) Token: 0x06003A82 RID: 14978 RVA: 0x000C1141 File Offset: 0x000BF341
		public string Description { get; } = "Sets the player(s) health to the specified amount.";

		// Token: 0x17000C80 RID: 3200
		// (get) Token: 0x06003A83 RID: 14979 RVA: 0x000C1149 File Offset: 0x000BF349
		public string[] Usage { get; } = new string[]
		{
			"%player%",
			"Amount"
		};

		// Token: 0x06003A84 RID: 14980 RVA: 0x000C1154 File Offset: 0x000BF354
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
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
			int num = int.TryParse(array[0], out num) ? num : 0;
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			int num2 = 0;
			if (list != null)
			{
				foreach (ReferenceHub referenceHub in list)
				{
					if (!(referenceHub == null))
					{
						HealthStat module = referenceHub.playerStats.GetModule<HealthStat>();
						module.CurValue = ((num > 0) ? ((float)num) : module.MaxValue);
						if (num2 != 0)
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.Append(referenceHub.LoggedNameFromRefHub());
						num2++;
					}
				}
			}
			if (num2 > 0)
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} set health of player{1}{2} to {3}.", new object[]
				{
					sender.LogName,
					(num2 == 1) ? " " : "s ",
					stringBuilder,
					num
				}), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			}
			StringBuilderPool.Shared.Return(stringBuilder);
			ListPool<ReferenceHub>.Shared.Return(list);
			response = string.Format("Done! The request affected {0} player{1}", num2, (num2 == 1) ? "!" : "s!");
			return true;
		}
	}
}
