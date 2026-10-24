using System;
using System.Collections.Generic;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using RemoteAdmin;
using Utils;
using VoiceChat;

namespace CommandSystem.Commands.RemoteAdmin.MutingAndIntercom
{
	// Token: 0x02000A52 RID: 2642
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class UnmuteCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000D1E RID: 3358
		// (get) Token: 0x06003BB0 RID: 15280 RVA: 0x000C4CD2 File Offset: 0x000C2ED2
		public string Command { get; } = "unmute";

		// Token: 0x17000D1F RID: 3359
		// (get) Token: 0x06003BB1 RID: 15281 RVA: 0x000C4CDA File Offset: 0x000C2EDA
		public string[] Aliases { get; }

		// Token: 0x17000D20 RID: 3360
		// (get) Token: 0x06003BB2 RID: 15282 RVA: 0x000C4CE2 File Offset: 0x000C2EE2
		public string Description { get; } = "Allows the specified player(s) to speak again.";

		// Token: 0x17000D21 RID: 3361
		// (get) Token: 0x06003BB3 RID: 15283 RVA: 0x000C4CEA File Offset: 0x000C2EEA
		public string[] Usage { get; } = new string[]
		{
			"%player%"
		};

		// Token: 0x06003BB4 RID: 15284 RVA: 0x000C4CF4 File Offset: 0x000C2EF4
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(new PlayerPermissions[]
			{
				PlayerPermissions.BanningUpToDay,
				PlayerPermissions.LongTermBanning,
				PlayerPermissions.PlayersManagement
			}, out response))
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
			PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
			ReferenceHub issuer = (playerCommandSender != null) ? playerCommandSender.ReferenceHub : ReferenceHub.HostHub;
			int num = 0;
			if (list != null)
			{
				foreach (ReferenceHub referenceHub in list)
				{
					PlayerUnmutingEventArgs playerUnmutingEventArgs = new PlayerUnmutingEventArgs(referenceHub, issuer, false);
					PlayerEvents.OnUnmuting(playerUnmutingEventArgs);
					if (playerUnmutingEventArgs.IsAllowed)
					{
						VoiceChatMutes.RevokeLocalMute(referenceHub.authManager.UserId, false);
						PlayerEvents.OnUnmuted(new PlayerUnmutedEventArgs(referenceHub, issuer, false));
						ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " unmuted player " + referenceHub.LoggedNameFromRefHub() + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
						num++;
					}
				}
			}
			response = string.Format("Done! The request affected {0} player{1}", num, (num == 1) ? "!" : "s!");
			return true;
		}
	}
}
