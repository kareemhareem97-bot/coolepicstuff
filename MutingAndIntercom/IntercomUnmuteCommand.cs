using System;
using System.Collections.Generic;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using RemoteAdmin;
using Utils;
using VoiceChat;

namespace CommandSystem.Commands.RemoteAdmin.MutingAndIntercom
{
	// Token: 0x02000A50 RID: 2640
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class IntercomUnmuteCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000D16 RID: 3350
		// (get) Token: 0x06003BA4 RID: 15268 RVA: 0x000C49A9 File Offset: 0x000C2BA9
		public string Command { get; } = "iunmute";

		// Token: 0x17000D17 RID: 3351
		// (get) Token: 0x06003BA5 RID: 15269 RVA: 0x000C49B1 File Offset: 0x000C2BB1
		public string[] Aliases { get; }

		// Token: 0x17000D18 RID: 3352
		// (get) Token: 0x06003BA6 RID: 15270 RVA: 0x000C49B9 File Offset: 0x000C2BB9
		public string Description { get; } = "Allows the specified player(s) from being able to speak over the intercom again.";

		// Token: 0x17000D19 RID: 3353
		// (get) Token: 0x06003BA7 RID: 15271 RVA: 0x000C49C1 File Offset: 0x000C2BC1
		public string[] Usage { get; } = new string[]
		{
			"%player%"
		};

		// Token: 0x06003BA8 RID: 15272 RVA: 0x000C49CC File Offset: 0x000C2BCC
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
					PlayerUnmutingEventArgs playerUnmutingEventArgs = new PlayerUnmutingEventArgs(referenceHub, issuer, true);
					PlayerEvents.OnUnmuting(playerUnmutingEventArgs);
					if (playerUnmutingEventArgs.IsAllowed)
					{
						VoiceChatMutes.RevokeLocalMute(referenceHub.authManager.UserId, true);
						PlayerEvents.OnUnmuted(new PlayerUnmutedEventArgs(referenceHub, issuer, true));
						ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " revoked an intercom mute of player " + referenceHub.LoggedNameFromRefHub() + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
						num++;
					}
				}
			}
			response = string.Format("Done! The request affected {0} player{1}", num, (num == 1) ? "!" : "s!");
			return true;
		}
	}
}
