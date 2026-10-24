using System;
using System.Collections.Generic;
using System.Text;
using NorthwoodLib.Pools;
using PlayerRoles;
using PlayerStatsSystem;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A1E RID: 2590
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class SetMaxHealthCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000C85 RID: 3205
		// (get) Token: 0x06003A8C RID: 14988 RVA: 0x000C1464 File Offset: 0x000BF664
		public string Command { get; } = "maxhp";

		// Token: 0x17000C86 RID: 3206
		// (get) Token: 0x06003A8D RID: 14989 RVA: 0x000C146C File Offset: 0x000BF66C
		public string[] Aliases { get; } = new string[]
		{
			"setmaxhealth",
			"setmaxhp"
		};

		// Token: 0x17000C87 RID: 3207
		// (get) Token: 0x06003A8E RID: 14990 RVA: 0x000C1474 File Offset: 0x000BF674
		public string Description { get; } = "Sets the player(s) maximum health to the specified amount.";

		// Token: 0x17000C88 RID: 3208
		// (get) Token: 0x06003A8F RID: 14991 RVA: 0x000C147C File Offset: 0x000BF67C
		public string[] Usage { get; } = new string[]
		{
			"%player%",
			"Amount"
		};

		// Token: 0x06003A90 RID: 14992 RVA: 0x000C1484 File Offset: 0x000BF684
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
			float num = float.TryParse(array[0], out num) ? num : 0f;
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			int num2 = 0;
			if (list != null)
			{
				foreach (ReferenceHub referenceHub in list)
				{
					if (!(referenceHub == null))
					{
						HealthStat module = referenceHub.playerStats.GetModule<HealthStat>();
						if (num >= 0f)
						{
							goto IL_CB;
						}
						IHealthbarRole healthbarRole = referenceHub.roleManager.CurrentRole as IHealthbarRole;
						if (healthbarRole == null)
						{
							goto IL_CB;
						}
						module.MaxValue = healthbarRole.MaxHealth;
						IL_D3:
						if (num2 != 0)
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.Append(referenceHub.LoggedNameFromRefHub());
						num2++;
						continue;
						IL_CB:
						module.MaxValue = num;
						goto IL_D3;
					}
				}
			}
			if (num2 > 0)
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} set maximum health of player{1}{2} to {3}.", new object[]
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
