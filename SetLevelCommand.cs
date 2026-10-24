using System;
using System.Collections.Generic;
using System.Text;
using NorthwoodLib.Pools;
using PlayerRoles.PlayableScps.Scp079;
using UnityEngine;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A18 RID: 2584
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class SetLevelCommand : Scp079CommandBase
	{
		// Token: 0x17000C6D RID: 3181
		// (get) Token: 0x06003A67 RID: 14951 RVA: 0x000C0A49 File Offset: 0x000BEC49
		public override string Command { get; } = "setlevel";

		// Token: 0x17000C6E RID: 3182
		// (get) Token: 0x06003A68 RID: 14952 RVA: 0x000C0A51 File Offset: 0x000BEC51
		public override string[] Aliases { get; } = new string[]
		{
			"settier",
			"level",
			"lvl"
		};

		// Token: 0x17000C6F RID: 3183
		// (get) Token: 0x06003A69 RID: 14953 RVA: 0x000C0A59 File Offset: 0x000BEC59
		public override string Description { get; } = "Sets the level of the player playing as SCP-079.";

		// Token: 0x17000C70 RID: 3184
		// (get) Token: 0x06003A6A RID: 14954 RVA: 0x000C0A61 File Offset: 0x000BEC61
		public override string[] Usage { get; } = new string[]
		{
			"%player%",
			"New Level"
		};

		// Token: 0x06003A6B RID: 14955 RVA: 0x000C0A6C File Offset: 0x000BEC6C
		public override bool OnExecute(ArraySegment<string> arguments, ICommandSender sender, int input, out string response)
		{
			string[] array;
			List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out array, false);
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			int input2 = 0;
			int num = 0;
			bool flag = input-- <= 1;
			input--;
			foreach (ReferenceHub referenceHub in list)
			{
				Scp079Role scp079Role = referenceHub.roleManager.CurrentRole as Scp079Role;
				Scp079TierManager scp079TierManager;
				if (scp079Role != null && scp079Role.SubroutineModule.TryGetSubroutine<Scp079TierManager>(out scp079TierManager))
				{
					if (!flag)
					{
						input2 = scp079TierManager.AbsoluteThresholds[Mathf.Clamp(input, 0, scp079TierManager.AbsoluteThresholds.Length - 1)];
					}
					this.ApplyChanges(scp079TierManager, input2);
					num++;
					stringBuilder.Append(", " + referenceHub.LoggedNameFromRefHub());
				}
			}
			if (num > 0)
			{
				string text = StringBuilderPool.Shared.ToStringReturn(stringBuilder).Substring(2);
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} used \"{1} ({2})\" command on player{3}{4}.", new object[]
				{
					sender.LogName,
					this.Command,
					input,
					(num == 1) ? " " : "s ",
					text
				}), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			}
			else
			{
				StringBuilderPool.Shared.Return(stringBuilder);
			}
			response = string.Format("Done! The request affected {0} player{1}", num, (num == 1) ? "!" : "s!");
			return true;
		}

		// Token: 0x06003A6C RID: 14956 RVA: 0x000C09CC File Offset: 0x000BEBCC
		public override void ApplyChanges(Scp079TierManager manager, int exp)
		{
			manager.TotalExp = exp;
		}
	}
}
