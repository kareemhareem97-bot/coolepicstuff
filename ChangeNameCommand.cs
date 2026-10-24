using System;
using System.Collections.Generic;
using System.Text;
using NorthwoodLib.Pools;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009DB RID: 2523
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class ChangeNameCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000BA2 RID: 2978
		// (get) Token: 0x060038F6 RID: 14582 RVA: 0x000B9B05 File Offset: 0x000B7D05
		public string Command { get; } = "setname";

		// Token: 0x17000BA3 RID: 2979
		// (get) Token: 0x060038F7 RID: 14583 RVA: 0x000B9B0D File Offset: 0x000B7D0D
		public string[] Aliases { get; } = new string[]
		{
			"setnickname",
			"setnick"
		};

		// Token: 0x17000BA4 RID: 2980
		// (get) Token: 0x060038F8 RID: 14584 RVA: 0x000B9B15 File Offset: 0x000B7D15
		public string Description { get; } = "Changes or resets player's name.";

		// Token: 0x17000BA5 RID: 2981
		// (get) Token: 0x060038F9 RID: 14585 RVA: 0x000B9B1D File Offset: 0x000B7D1D
		public string[] Usage { get; } = new string[]
		{
			"%player%",
			"new name (optional)"
		};

		// Token: 0x060038FA RID: 14586 RVA: 0x000B9B28 File Offset: 0x000B7D28
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
			if (list == null || list.Count == 0)
			{
				response = "Cannot find player(s) using the arguments provided!";
				return false;
			}
			string text = (array == null) ? null : string.Join(" ", array);
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			foreach (ReferenceHub referenceHub in list)
			{
				if (text == null)
				{
					ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} cleared nickname of player {1} ({2}).", sender.LogName, referenceHub.PlayerId, referenceHub.nicknameSync.MyNick), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
					stringBuilder.AppendFormat("Reset {0}'s display name.\n", referenceHub.LoggedNameFromRefHub());
					referenceHub.nicknameSync.DisplayName = null;
				}
				else
				{
					ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} set nickname of player {1} ({2}) to \"{3}\".", new object[]
					{
						sender.LogName,
						referenceHub.PlayerId,
						referenceHub.nicknameSync.MyNick,
						text
					}), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
					stringBuilder.AppendFormat("Set {0}'s display name to: {1}\n", referenceHub.LoggedNameFromRefHub(), text);
					referenceHub.nicknameSync.DisplayName = text;
				}
			}
			response = stringBuilder.ToString().Trim();
			StringBuilderPool.Shared.Return(stringBuilder);
			return true;
		}
	}
}
