using System;
using System.Collections.Generic;
using System.Text;
using NorthwoodLib.Pools;
using RemoteAdmin;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A00 RID: 2560
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class OverwatchCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000C1F RID: 3103
		// (get) Token: 0x060039D3 RID: 14803 RVA: 0x000BE4F1 File Offset: 0x000BC6F1
		public string Command { get; } = "overwatch";

		// Token: 0x17000C20 RID: 3104
		// (get) Token: 0x060039D4 RID: 14804 RVA: 0x000BE4F9 File Offset: 0x000BC6F9
		public string[] Aliases { get; } = new string[]
		{
			"ovr"
		};

		// Token: 0x17000C21 RID: 3105
		// (get) Token: 0x060039D5 RID: 14805 RVA: 0x000BE501 File Offset: 0x000BC701
		public string Description { get; } = "Changes the status of overwatch mode for the specified player(s).";

		// Token: 0x17000C22 RID: 3106
		// (get) Token: 0x060039D6 RID: 14806 RVA: 0x000BE509 File Offset: 0x000BC709
		public string[] Usage { get; } = new string[]
		{
			"%player%",
			"enable/disable (Leave blank for toggle)"
		};

		// Token: 0x060039D7 RID: 14807 RVA: 0x000BE514 File Offset: 0x000BC714
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.Overwatch, out response))
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
			PlayerCommandSender playerCommandSender2 = (PlayerCommandSender)sender;
			bool flag = playerCommandSender2.ReferenceHub.authManager.NorthwoodStaff || playerCommandSender2.CheckPermission(PlayerPermissions.PermissionsManagement);
			int num = 0;
			if (list != null)
			{
				foreach (ReferenceHub referenceHub in list)
				{
					ServerRoles serverRoles = referenceHub.serverRoles;
					if (!referenceHub.authManager.BypassBansFlagSet || flag)
					{
						switch (commandOperationMode)
						{
						case Misc.CommandOperationMode.Disable:
							if (!serverRoles.IsInOverwatch)
							{
								continue;
							}
							OverwatchCommand.SetOverwatchStatus(serverRoles, 0);
							if (num != 0)
							{
								stringBuilder.Append(", ");
							}
							stringBuilder.Append(referenceHub.LoggedNameFromRefHub());
							break;
						case Misc.CommandOperationMode.Enable:
							if (serverRoles.IsInOverwatch)
							{
								continue;
							}
							OverwatchCommand.SetOverwatchStatus(serverRoles, 1);
							if (num != 0)
							{
								stringBuilder.Append(", ");
							}
							stringBuilder.Append(referenceHub.LoggedNameFromRefHub());
							break;
						case Misc.CommandOperationMode.Toggle:
							OverwatchCommand.SetOverwatchStatus(serverRoles, 2);
							if (serverRoles.IsInOverwatch)
							{
								ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " enabled overwatch mode for player " + referenceHub.LoggedNameFromRefHub() + " using overwatch mode toggle command.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
							}
							else
							{
								ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " disabled overwatch mode for player " + referenceHub.LoggedNameFromRefHub() + " using overwatch mode toggle command.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
							}
							break;
						}
						num++;
					}
				}
			}
			if (num > 0)
			{
				if (commandOperationMode != Misc.CommandOperationMode.Disable)
				{
					if (commandOperationMode == Misc.CommandOperationMode.Enable)
					{
						ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} enabled overwatch mode for player{1}{2}.", sender.LogName, (num == 1) ? " " : "s ", stringBuilder), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
						StringBuilderPool.Shared.Return(stringBuilder);
					}
				}
				else
				{
					ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} disabled overwatch mode for player{1}{2}.", sender.LogName, (num == 1) ? " " : "s ", stringBuilder), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
					StringBuilderPool.Shared.Return(stringBuilder);
				}
			}
			response = string.Format("Done! The request affected {0} player{1}", num, (num == 1) ? "!" : "s!");
			return true;
		}

		// Token: 0x060039D8 RID: 14808 RVA: 0x000BE814 File Offset: 0x000BCA14
		private static void SetOverwatchStatus(ServerRoles sr, byte status)
		{
			bool isInOverwatch;
			switch (status)
			{
			case 0:
				isInOverwatch = false;
				break;
			case 1:
				isInOverwatch = true;
				break;
			case 2:
				isInOverwatch = !sr.IsInOverwatch;
				break;
			default:
				isInOverwatch = sr.IsInOverwatch;
				break;
			}
			sr.IsInOverwatch = isInOverwatch;
		}
	}
}
