using System;
using System.Collections.Generic;
using System.Text;
using NorthwoodLib.Pools;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009DA RID: 2522
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class ChangeCustomPlayerInfoCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000B9E RID: 2974
		// (get) Token: 0x060038F0 RID: 14576 RVA: 0x000B98ED File Offset: 0x000B7AED
		public string Command { get; } = "custominfo";

		// Token: 0x17000B9F RID: 2975
		// (get) Token: 0x060038F1 RID: 14577 RVA: 0x000B98F5 File Offset: 0x000B7AF5
		public string[] Aliases { get; } = new string[]
		{
			"customi",
			"cinfo"
		};

		// Token: 0x17000BA0 RID: 2976
		// (get) Token: 0x060038F2 RID: 14578 RVA: 0x000B98FD File Offset: 0x000B7AFD
		public string Description { get; } = "Allows to quickly change the custom info string of a player";

		// Token: 0x17000BA1 RID: 2977
		// (get) Token: 0x060038F3 RID: 14579 RVA: 0x000B9905 File Offset: 0x000B7B05
		public string[] Usage { get; } = new string[]
		{
			"%player%",
			"text (optional)"
		};

		// Token: 0x060038F4 RID: 14580 RVA: 0x000B9910 File Offset: 0x000B7B10
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
				response = "Cannot find player! Try using the player ID!";
				return false;
			}
			string text = (array == null) ? null : string.Join(" ", array);
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			foreach (ReferenceHub referenceHub in list)
			{
				if (text == null)
				{
					ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} cleared custom info of player {1} ({2}).", sender.LogName, referenceHub.PlayerId, referenceHub.nicknameSync.MyNick), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
					stringBuilder.AppendFormat("Reset {0}'s custom info.\n", referenceHub.LoggedNameFromRefHub());
					referenceHub.nicknameSync.CustomPlayerInfo = null;
				}
				else
				{
					ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} set custom info of player {1} ({2}) to \"{3}\".", new object[]
					{
						sender.LogName,
						referenceHub.PlayerId,
						referenceHub.nicknameSync.MyNick,
						text
					}), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
					stringBuilder.AppendFormat("Set {0}'s custom info to: {1}\n", referenceHub.LoggedNameFromRefHub(), text);
					referenceHub.nicknameSync.CustomPlayerInfo = text;
				}
			}
			response = stringBuilder.ToString().Trim();
			StringBuilderPool.Shared.Return(stringBuilder);
			return true;
		}
	}
}
