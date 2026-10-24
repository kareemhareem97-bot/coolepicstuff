using System;
using System.Collections.Generic;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using RemoteAdmin;
using Utils;
using VoiceChat;

namespace CommandSystem.Commands.RemoteAdmin.MutingAndIntercom
{
	// Token: 0x02000A51 RID: 2641
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class MuteCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000D1A RID: 3354
		// (get) Token: 0x06003BAA RID: 15274 RVA: 0x000C4B3E File Offset: 0x000C2D3E
		public string Command { get; } = "mute";

		// Token: 0x17000D1B RID: 3355
		// (get) Token: 0x06003BAB RID: 15275 RVA: 0x000C4B46 File Offset: 0x000C2D46
		public string[] Aliases { get; }

		// Token: 0x17000D1C RID: 3356
		// (get) Token: 0x06003BAC RID: 15276 RVA: 0x000C4B4E File Offset: 0x000C2D4E
		public string Description { get; } = "Prevents the specified player(s) from being able to speak.";

		// Token: 0x17000D1D RID: 3357
		// (get) Token: 0x06003BAD RID: 15277 RVA: 0x000C4B56 File Offset: 0x000C2D56
		public string[] Usage { get; } = new string[]
		{
			"%player%"
		};

		// Token: 0x06003BAE RID: 15278 RVA: 0x000C4B60 File Offset: 0x000C2D60
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
					PlayerMutingEventArgs playerMutingEventArgs = new PlayerMutingEventArgs(referenceHub, issuer, false);
					PlayerEvents.OnMuting(playerMutingEventArgs);
					if (playerMutingEventArgs.IsAllowed)
					{
						VoiceChatMutes.IssueLocalMute(referenceHub.authManager.UserId, false);
						PlayerEvents.OnMuted(new PlayerMutedEventArgs(referenceHub, issuer, false));
						ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " muted player " + referenceHub.LoggedNameFromRefHub() + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
						num++;
					}
				}
			}
			response = string.Format("Done! The request affected {0} player{1}", num, (num == 1) ? "!" : "s!");
			return true;
		}
	}
}
