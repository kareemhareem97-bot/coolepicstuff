using System;
using System.Collections.Generic;
using System.Text;
using NorthwoodLib.Pools;
using PlayerStatsSystem;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009F9 RID: 2553
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class KillCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000C06 RID: 3078
		// (get) Token: 0x060039AB RID: 14763 RVA: 0x000BD86E File Offset: 0x000BBA6E
		public string Command { get; } = "kill";

		// Token: 0x17000C07 RID: 3079
		// (get) Token: 0x060039AC RID: 14764 RVA: 0x000BD876 File Offset: 0x000BBA76
		public string[] Aliases { get; }

		// Token: 0x17000C08 RID: 3080
		// (get) Token: 0x060039AD RID: 14765 RVA: 0x000BD87E File Offset: 0x000BBA7E
		public string Description { get; } = "Kills the specified player(s).";

		// Token: 0x17000C09 RID: 3081
		// (get) Token: 0x060039AE RID: 14766 RVA: 0x000BD886 File Offset: 0x000BBA86
		public string[] Usage { get; } = new string[]
		{
			"PlayerId"
		};

		// Token: 0x060039AF RID: 14767 RVA: 0x000BD890 File Offset: 0x000BBA90
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
			{
				return false;
			}
			if (arguments.Count < 1)
			{
				response = string.Format("To execute this command provide at least 1 argument!\nUsage: {0} {1}", arguments.Array[0], this.Usage);
				return false;
			}
			string[] array;
			List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out array, false);
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			int num = 0;
			if (list != null)
			{
				foreach (ReferenceHub referenceHub in list)
				{
					if (referenceHub != null && referenceHub.playerStats.DealDamage(new UniversalDamageHandler(-1f, DeathTranslations.Unknown, null)))
					{
						if (num != 0)
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.Append(referenceHub.LoggedNameFromRefHub());
						num++;
					}
				}
			}
			if (num > 0)
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} administratively killed player{1}{2}.", sender.LogName, (num == 1) ? " " : "s ", stringBuilder), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			}
			StringBuilderPool.Shared.Return(stringBuilder);
			response = string.Format("Done! The request affected {0} player{1}", num, (num == 1) ? "!" : "s!");
			return true;
		}
	}
}
