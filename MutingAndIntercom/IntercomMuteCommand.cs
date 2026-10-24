using System;
using System.Collections.Generic;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using RemoteAdmin;
using Utils;
using VoiceChat;

namespace CommandSystem.Commands.RemoteAdmin.MutingAndIntercom
{
	// Token: 0x02000A4E RID: 2638
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class IntercomMuteCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000D0E RID: 3342
		// (get) Token: 0x06003B98 RID: 15256 RVA: 0x000C46F1 File Offset: 0x000C28F1
		public string Command { get; } = "imute";

		// Token: 0x17000D0F RID: 3343
		// (get) Token: 0x06003B99 RID: 15257 RVA: 0x000C46F9 File Offset: 0x000C28F9
		public string[] Aliases { get; }

		// Token: 0x17000D10 RID: 3344
		// (get) Token: 0x06003B9A RID: 15258 RVA: 0x000C4701 File Offset: 0x000C2901
		public string Description { get; } = "Prevents the specified player(s) from being able to speak over the intercom.";

		// Token: 0x17000D11 RID: 3345
		// (get) Token: 0x06003B9B RID: 15259 RVA: 0x000C4709 File Offset: 0x000C2909
		public string[] Usage { get; } = new string[]
		{
			"%player%"
		};

		// Token: 0x06003B9C RID: 15260 RVA: 0x000C4714 File Offset: 0x000C2914
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
					PlayerMutingEventArgs playerMutingEventArgs = new PlayerMutingEventArgs(referenceHub, issuer, true);
					PlayerEvents.OnMuting(playerMutingEventArgs);
					if (playerMutingEventArgs.IsAllowed)
					{
						VoiceChatMutes.IssueLocalMute(referenceHub.authManager.UserId, true);
						PlayerEvents.OnMuted(new PlayerMutedEventArgs(referenceHub, issuer, true));
						ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " issued an intercom mute to player " + referenceHub.LoggedNameFromRefHub() + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
						num++;
					}
				}
			}
			response = string.Format("Done! The request affected {0} player{1}", num, (num == 1) ? "!" : "s!");
			return true;
		}
	}
}
