using System;
using System.Collections.Generic;
using System.Text;
using NorthwoodLib.Pools;
using RemoteAdmin;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009F2 RID: 2546
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class GodCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000BEA RID: 3050
		// (get) Token: 0x06003981 RID: 14721 RVA: 0x000BC9D5 File Offset: 0x000BABD5
		public string Command { get; } = "god";

		// Token: 0x17000BEB RID: 3051
		// (get) Token: 0x06003982 RID: 14722 RVA: 0x000BC9DD File Offset: 0x000BABDD
		public string[] Aliases { get; }

		// Token: 0x17000BEC RID: 3052
		// (get) Token: 0x06003983 RID: 14723 RVA: 0x000BC9E5 File Offset: 0x000BABE5
		public string Description { get; } = "Changes the status of god mode for the specified player(s).";

		// Token: 0x17000BED RID: 3053
		// (get) Token: 0x06003984 RID: 14724 RVA: 0x000BC9ED File Offset: 0x000BABED
		public string[] Usage { get; } = new string[]
		{
			"%player%",
			"enable/disable (Leave blank for toggle)"
		};

		// Token: 0x06003985 RID: 14725 RVA: 0x000BC9F8 File Offset: 0x000BABF8
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
			{
				return false;
			}
			string[] array = new string[0];
			PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
			List<ReferenceHub> list;
			if (playerCommandSender != null && (arguments.Count == 0 || (arguments.Count == 1 && !arguments.At(0).Contains(".") && !arguments.At(0).Contains("@"))))
			{
				list = new List<ReferenceHub>();
				list.Add(playerCommandSender.ReferenceHub);
				if (arguments.Count > 1)
				{
					array[0] = arguments.At(1);
				}
				else
				{
					array = null;
				}
			}
			else
			{
				list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out array, false);
			}
			Misc.CommandOperationMode commandOperationMode;
			if (!Misc.TryCommandModeFromArgs(ref array, out commandOperationMode))
			{
				response = "Invalid option " + array[0] + " - leave null for toggle or use 1/0, true/false, enable/disable or on/off.";
				return false;
			}
			StringBuilder stringBuilder = (commandOperationMode == Misc.CommandOperationMode.Toggle) ? null : StringBuilderPool.Shared.Rent();
			int num = 0;
			if (list != null)
			{
				foreach (ReferenceHub referenceHub in list)
				{
					CharacterClassManager characterClassManager = referenceHub.characterClassManager;
					switch (commandOperationMode)
					{
					case Misc.CommandOperationMode.Disable:
						if (!characterClassManager.GodMode)
						{
							continue;
						}
						characterClassManager.GodMode = false;
						if (num != 0)
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.Append(referenceHub.LoggedNameFromRefHub());
						break;
					case Misc.CommandOperationMode.Enable:
						if (characterClassManager.GodMode)
						{
							continue;
						}
						characterClassManager.GodMode = true;
						if (num != 0)
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.Append(referenceHub.LoggedNameFromRefHub());
						break;
					case Misc.CommandOperationMode.Toggle:
						characterClassManager.GodMode = !characterClassManager.GodMode;
						if (characterClassManager.GodMode)
						{
							ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " enabled god mode for player " + referenceHub.LoggedNameFromRefHub() + " using god mode toggle command.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
						}
						else
						{
							ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " disabled god mode for player " + referenceHub.LoggedNameFromRefHub() + " using god mode toggle command.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
						}
						break;
					}
					num++;
				}
			}
			if (num > 0)
			{
				if (commandOperationMode != Misc.CommandOperationMode.Disable)
				{
					if (commandOperationMode == Misc.CommandOperationMode.Enable)
					{
						ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} enabled god mode for player{1}{2}.", sender.LogName, (num == 1) ? " " : "s ", stringBuilder), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
						StringBuilderPool.Shared.Return(stringBuilder);
					}
				}
				else
				{
					ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} disabled god mode for player{1}{2}.", sender.LogName, (num == 1) ? " " : "s ", stringBuilder), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
					StringBuilderPool.Shared.Return(stringBuilder);
				}
			}
			response = string.Format("Done! The request affected {0} player{1}", num, (num == 1) ? "!" : "s!");
			return true;
		}
	}
}
