using System;
using System.Collections.Generic;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009F0 RID: 2544
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class FriendlyFireDetectorCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000BE2 RID: 3042
		// (get) Token: 0x06003973 RID: 14707 RVA: 0x000BC45C File Offset: 0x000BA65C
		public string Command { get; } = "friendlyfiredetector";

		// Token: 0x17000BE3 RID: 3043
		// (get) Token: 0x06003974 RID: 14708 RVA: 0x000BC464 File Offset: 0x000BA664
		public string[] Aliases { get; } = new string[]
		{
			"tk",
			"tkd",
			"teamkilldetector",
			"ffd"
		};

		// Token: 0x17000BE4 RID: 3044
		// (get) Token: 0x06003975 RID: 14709 RVA: 0x000BC46C File Offset: 0x000BA66C
		public string Description { get; } = "Friendly fire detection and logging";

		// Token: 0x17000BE5 RID: 3045
		// (get) Token: 0x06003976 RID: 14710 RVA: 0x000BC474 File Offset: 0x000BA674
		public string[] Usage { get; } = new string[]
		{
			"Player ID or Name/status/pause/unpause>"
		};

		// Token: 0x06003977 RID: 14711 RVA: 0x000BC47C File Offset: 0x000BA67C
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (arguments.Count < 1)
			{
				response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
			{
				return false;
			}
			string a = arguments.At(0);
			if (a == "status")
			{
				response = "FFD#Friendly fire detector is currently " + (FriendlyFireConfig.PauseDetector ? string.Empty : "**NOT** ") + "paused.";
				return true;
			}
			if (!(a == "pause"))
			{
				if (!(a == "unpause"))
				{
					string[] array;
					List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out array, false);
					if (list != null && list.Count != 1)
					{
						response = "FFD command requires exact one selected player, not an array of players!";
						return false;
					}
					if (list == null)
					{
						response = "The specified player was not found! \nUsage: FFD <Player ID or Name/status/pause/unpause>";
						return false;
					}
					FriendlyFireHandler friendlyFireHandler = list[0].FriendlyFireHandler;
					response = string.Format("--- Friendly Fire Detector Stats ---\nKills - Damage dealt\n\nRound: {0} - {1}\nLife: {2} - {3}\nWindow: {4} - {5} [Window: {6}s]\nRespawn: {7} - {8} [Window: {9}s]", new object[]
					{
						friendlyFireHandler.Round.Kills,
						friendlyFireHandler.Round.Damage,
						friendlyFireHandler.Life.Kills,
						friendlyFireHandler.Life.Damage,
						friendlyFireHandler.Window.Kills,
						friendlyFireHandler.Window.Damage,
						FriendlyFireConfig.Window,
						friendlyFireHandler.Respawn.Kills,
						friendlyFireHandler.Respawn.Damage,
						FriendlyFireConfig.RespawnWindow
					});
					return true;
				}
				else
				{
					if (!FriendlyFireConfig.PauseDetector)
					{
						response = "Friendly fire detector is not paused.";
						return false;
					}
					FriendlyFireConfig.PauseDetector = false;
					response = "Friendly fire detector has been unpaused.";
					ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " unpaused paused Friendly Fire Detector.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
					return true;
				}
			}
			else
			{
				if (FriendlyFireConfig.PauseDetector)
				{
					response = "Friendly fire detector is already paused.";
					return false;
				}
				FriendlyFireConfig.PauseDetector = true;
				response = "Friendly fire detector has been paused.";
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " paused Friendly Fire Detector.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
				return true;
			}
		}
	}
}
