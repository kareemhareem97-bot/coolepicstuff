using System;
using System.Collections.Generic;
using System.Text;
using CustomPlayerEffects;
using CustomPlayerEffects.Danger;
using NorthwoodLib.Pools;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009DD RID: 2525
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class DangerCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000BAA RID: 2986
		// (get) Token: 0x06003902 RID: 14594 RVA: 0x000B9EB5 File Offset: 0x000B80B5
		public string Command { get; } = "danger";

		// Token: 0x17000BAB RID: 2987
		// (get) Token: 0x06003903 RID: 14595 RVA: 0x000B9EBD File Offset: 0x000B80BD
		public string[] Aliases { get; }

		// Token: 0x17000BAC RID: 2988
		// (get) Token: 0x06003904 RID: 14596 RVA: 0x000B9EC5 File Offset: 0x000B80C5
		public string Description { get; } = "Outputs the currently active dangers of the specified player.";

		// Token: 0x17000BAD RID: 2989
		// (get) Token: 0x06003905 RID: 14597 RVA: 0x000B9ECD File Offset: 0x000B80CD
		public string[] Usage { get; } = new string[]
		{
			"%player%"
		};

		// Token: 0x06003906 RID: 14598 RVA: 0x000B9ED8 File Offset: 0x000B80D8
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.Effects, out response))
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
			if (list == null || list.Count != 1)
			{
				response = "This commands requires you to specify exactly one specific player!";
				return false;
			}
			ReferenceHub referenceHub = list[0];
			Scp1853 effect = referenceHub.playerEffectsController.GetEffect<Scp1853>();
			if (!effect.IsEnabled)
			{
				response = "The specified player does not have SCP-1853 active.";
				return false;
			}
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent(referenceHub.nicknameSync.DisplayName);
			stringBuilder.Append(" has <color=#65dbeb>").Append(effect.CurrentDanger).Append("</color> danger stacks. <color=#80eb65>x").Append(effect.StatMultiplier).Append("</color> stat boost.");
			foreach (DangerStackBase dangerStackBase in effect.Dangers)
			{
				if (dangerStackBase.IsActive)
				{
					int value = (int)((double)dangerStackBase.Duration - dangerStackBase.TimeTracker.Elapsed.TotalSeconds);
					stringBuilder.Append("\n- <color=#f59e42>").Append(dangerStackBase.GetType().Name).Append(":</color> ").Append(dangerStackBase.DangerValue).Append(" danger stacks").Append(" <color=#65aeeb>(").Append(value).Append("s left)</color>");
				}
			}
			response = StringBuilderPool.Shared.ToStringReturn(stringBuilder);
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " requested the SCP-1853 danger information of " + referenceHub.LoggedNameFromRefHub() + ".", ServerLogs.ServerLogType.RemoteAdminActivity_Misc, false);
			return true;
		}
	}
}
