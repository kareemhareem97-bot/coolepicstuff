using System;
using System.Collections.Generic;
using System.Text;
using NorthwoodLib.Pools;
using PlayerStatsSystem;
using UnityEngine;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009F4 RID: 2548
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class HealCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000BF2 RID: 3058
		// (get) Token: 0x0600398D RID: 14733 RVA: 0x000BCE35 File Offset: 0x000BB035
		public string Command { get; } = "heal";

		// Token: 0x17000BF3 RID: 3059
		// (get) Token: 0x0600398E RID: 14734 RVA: 0x000BCE3D File Offset: 0x000BB03D
		public string[] Aliases { get; }

		// Token: 0x17000BF4 RID: 3060
		// (get) Token: 0x0600398F RID: 14735 RVA: 0x000BCE45 File Offset: 0x000BB045
		public string Description { get; } = "Heals player(s) a specified amount.";

		// Token: 0x17000BF5 RID: 3061
		// (get) Token: 0x06003990 RID: 14736 RVA: 0x000BCE4D File Offset: 0x000BB04D
		public string[] Usage { get; } = new string[]
		{
			"%player%",
			"Amount (0 = full)"
		};

		// Token: 0x06003991 RID: 14737 RVA: 0x000BCE58 File Offset: 0x000BB058
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
			int num = (array != null && array.Length != 0 && int.TryParse(array[0], out num)) ? num : 0;
			float num2 = (array != null && array.Length > 1 && float.TryParse(array[1], out num2)) ? num2 : 1f;
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			int num3 = 0;
			if (list != null)
			{
				foreach (ReferenceHub referenceHub in list)
				{
					if (referenceHub != null)
					{
						HealthStat module = referenceHub.playerStats.GetModule<HealthStat>();
						if (num > 0)
						{
							module.AddAmount((float)Mathf.Abs(num), num2);
						}
						else
						{
							module.CurValue = module.MaxValue;
						}
						if (num3 != 0)
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.Append(referenceHub.LoggedNameFromRefHub());
						num3++;
					}
				}
			}
			if (num3 > 0)
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} healed player{1}{2}.", sender.LogName, (num3 == 1) ? " " : "s ", stringBuilder), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			}
			StringBuilderPool.Shared.Return(stringBuilder);
			response = string.Format("Done! The request affected {0} player{1}", num3, (num3 == 1) ? "!" : "s!");
			return true;
		}
	}
}
