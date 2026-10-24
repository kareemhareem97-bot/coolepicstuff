using System;
using System.Collections.Generic;
using System.Text;
using NorthwoodLib.Pools;
using PlayerRoles.FirstPersonControl;
using RemoteAdmin;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009FE RID: 2558
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class NoclipCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000C17 RID: 3095
		// (get) Token: 0x060039C6 RID: 14790 RVA: 0x000BDFB5 File Offset: 0x000BC1B5
		public string Command { get; } = "noclip";

		// Token: 0x17000C18 RID: 3096
		// (get) Token: 0x060039C7 RID: 14791 RVA: 0x000BDFBD File Offset: 0x000BC1BD
		public string[] Aliases { get; } = new string[]
		{
			"n",
			"nc"
		};

		// Token: 0x17000C19 RID: 3097
		// (get) Token: 0x060039C8 RID: 14792 RVA: 0x000BDFC5 File Offset: 0x000BC1C5
		public string Description
		{
			get
			{
				return "Toggles noclip on the selected player(s). Press the Left Alt key (by default) to toggle, use scroll to change speed.";
			}
		}

		// Token: 0x17000C1A RID: 3098
		// (get) Token: 0x060039C9 RID: 14793 RVA: 0x000BDFCC File Offset: 0x000BC1CC
		public string[] Usage { get; } = new string[]
		{
			"%player%",
			"enable/disable (Leave blank for toggle)"
		};

		// Token: 0x060039CA RID: 14794 RVA: 0x000BDFD4 File Offset: 0x000BC1D4
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.Noclip, out response))
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
				if (arguments.Count > 0)
				{
					array[0] = arguments.At(0);
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
					ServerRoles serverRoles = referenceHub.serverRoles;
					switch (commandOperationMode)
					{
					case Misc.CommandOperationMode.Disable:
						if (!FpcNoclip.IsPermitted(referenceHub))
						{
							continue;
						}
						FpcNoclip.UnpermitPlayer(referenceHub);
						if (num != 0)
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.Append(referenceHub.LoggedNameFromRefHub());
						break;
					case Misc.CommandOperationMode.Enable:
						if (FpcNoclip.IsPermitted(referenceHub))
						{
							continue;
						}
						FpcNoclip.PermitPlayer(referenceHub);
						if (num != 0)
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.Append(referenceHub.LoggedNameFromRefHub());
						break;
					case Misc.CommandOperationMode.Toggle:
						if (FpcNoclip.IsPermitted(referenceHub))
						{
							FpcNoclip.UnpermitPlayer(referenceHub);
						}
						else
						{
							FpcNoclip.PermitPlayer(referenceHub);
						}
						if (serverRoles.BypassMode)
						{
							ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " enabled noclip for player " + referenceHub.LoggedNameFromRefHub() + " using noclip toggle command.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
						}
						else
						{
							ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " disabled noclip for player " + referenceHub.LoggedNameFromRefHub() + " using noclip toggle command.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
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
						ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} enabled noclip for player{1}{2}.", sender.LogName, (num == 1) ? " " : "s ", stringBuilder), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
						StringBuilderPool.Shared.Return(stringBuilder);
					}
				}
				else
				{
					ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} disabled noclip for player{1}{2}.", sender.LogName, (num == 1) ? " " : "s ", stringBuilder), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
					StringBuilderPool.Shared.Return(stringBuilder);
				}
			}
			response = string.Format("Done! The request affected {0} player{1}", num, (num == 1) ? "!" : "s!");
			return true;
		}
	}
}
