using System;
using CentralAuth;
using Mirror;
using Utils.NonAllocLINQ;

namespace CommandSystem.Commands.Console
{
	// Token: 0x02000A96 RID: 2710
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class IdleCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000DD9 RID: 3545
		// (get) Token: 0x06003D24 RID: 15652 RVA: 0x000C8F27 File Offset: 0x000C7127
		public string Command { get; } = "idle";

		// Token: 0x17000DDA RID: 3546
		// (get) Token: 0x06003D25 RID: 15653 RVA: 0x000C8F2F File Offset: 0x000C712F
		public string[] Aliases { get; } = new string[]
		{
			"i"
		};

		// Token: 0x17000DDB RID: 3547
		// (get) Token: 0x06003D26 RID: 15654 RVA: 0x000C8F37 File Offset: 0x000C7137
		public string Description { get; } = "Controls server idle mode";

		// Token: 0x17000DDC RID: 3548
		// (get) Token: 0x06003D27 RID: 15655 RVA: 0x000C8F3F File Offset: 0x000C713F
		public string[] Usage { get; } = new string[]
		{
			"Enable/Disable/ForceEnable (Optional)"
		};

		// Token: 0x06003D28 RID: 15656 RVA: 0x000C8F48 File Offset: 0x000C7148
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!ServerStatic.IsDedicated)
			{
				response = "This command can only be executed on a dedicated server.";
				return false;
			}
			if (!NetworkServer.active)
			{
				response = "This command can only be used on a server.";
				return false;
			}
			if (!sender.CheckPermission(PlayerPermissions.ServerConsoleCommands, out response))
			{
				return false;
			}
			if (arguments.Count == 0)
			{
				response = "Server is " + (IdleMode.IdleModeActive ? string.Empty : "**NOT** ") + "currently in idle mode.";
				return true;
			}
			response = "";
			string text = arguments.At(0).ToUpperInvariant();
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			if (num <= 2311963226U)
			{
				if (num <= 2278407988U)
				{
					if (num != 785938062U)
					{
						if (num != 2278407988U)
						{
							goto IL_2AC;
						}
						if (!(text == "-D"))
						{
							goto IL_2AC;
						}
						goto IL_26D;
					}
					else if (!(text == "ENABLE"))
					{
						goto IL_2AC;
					}
				}
				else if (num != 2295185607U)
				{
					if (num != 2311963226U)
					{
						goto IL_2AC;
					}
					if (!(text == "-F"))
					{
						goto IL_2AC;
					}
					goto IL_22E;
				}
				else if (!(text == "-E"))
				{
					goto IL_2AC;
				}
			}
			else if (num <= 3222007936U)
			{
				if (num != 2616878531U)
				{
					if (num != 3222007936U)
					{
						goto IL_2AC;
					}
					if (!(text == "E"))
					{
						goto IL_2AC;
					}
				}
				else
				{
					if (!(text == "DISABLE"))
					{
						goto IL_2AC;
					}
					goto IL_26D;
				}
			}
			else if (num != 3238785555U)
			{
				if (num != 3272340793U)
				{
					if (num != 3792880932U)
					{
						goto IL_2AC;
					}
					if (!(text == "FORCE"))
					{
						goto IL_2AC;
					}
					goto IL_22E;
				}
				else
				{
					if (!(text == "F"))
					{
						goto IL_2AC;
					}
					goto IL_22E;
				}
			}
			else
			{
				if (!(text == "D"))
				{
					goto IL_2AC;
				}
				goto IL_26D;
			}
			if (IdleMode.IdleModeActive)
			{
				response = "Server is already in the idle mode.";
				return false;
			}
			if (ReferenceHub.AllHubs.Any((ReferenceHub x) => x.Mode == ClientInstanceMode.ReadyClient))
			{
				response = "You can't enable the idle mode when players are connected to the server.";
				return false;
			}
			if (!(sender is ServerConsoleSender))
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " enabled the idle mode.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
				response = "Idle mode enabled.";
			}
			IdleMode.SetIdleMode(true);
			return true;
			IL_22E:
			if (IdleMode.IdleModeActive)
			{
				response = "Server is already in the idle mode.";
				return false;
			}
			if (!(sender is ServerConsoleSender))
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " force enabled the idle mode.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
				response = "Idle mode force enabled.";
			}
			IdleMode.SetIdleMode(true);
			return true;
			IL_26D:
			if (!IdleMode.IdleModeActive)
			{
				response = "Server isn't in idle mode.";
				return false;
			}
			if (!(sender is ServerConsoleSender))
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " disabled the idle mode.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
				response = "Idle mode disabled";
			}
			IdleMode.SetIdleMode(false);
			return true;
			IL_2AC:
			response = string.Concat(new string[]
			{
				"Unknown subcommand.\nUsage: ",
				this.Command,
				" ",
				this.DisplayCommandUsage(),
				"."
			});
			return false;
		}
	}
}
