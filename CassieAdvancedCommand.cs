using System;
using Cassie;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009D3 RID: 2515
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class CassieAdvancedCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000B8B RID: 2955
		// (get) Token: 0x060038CD RID: 14541 RVA: 0x000B91E1 File Offset: 0x000B73E1
		public string Command { get; } = "cassieadvanced";

		// Token: 0x17000B8C RID: 2956
		// (get) Token: 0x060038CE RID: 14542 RVA: 0x000B91E9 File Offset: 0x000B73E9
		public string[] Aliases { get; }

		// Token: 0x17000B8D RID: 2957
		// (get) Token: 0x060038CF RID: 14543 RVA: 0x000B91F1 File Offset: 0x000B73F1
		public string Description { get; } = "Sends an announcement over the CASSIE system with custom subtitle.";

		// Token: 0x17000B8E RID: 2958
		// (get) Token: 0x060038D0 RID: 14544 RVA: 0x000B91F9 File Offset: 0x000B73F9
		public string[] Usage { get; } = new string[]
		{
			"message"
		};

		// Token: 0x060038D1 RID: 14545 RVA: 0x000B9204 File Offset: 0x000B7404
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.Announcer, out response))
			{
				return false;
			}
			CassieTtsPayload.SubtitleMode[] array = new CassieTtsPayload.SubtitleMode[]
			{
				CassieTtsPayload.SubtitleMode.None,
				CassieTtsPayload.SubtitleMode.Automatic,
				CassieTtsPayload.SubtitleMode.Custom
			};
			CassieTtsPayload.SubtitleMode subtitleMode;
			if (arguments.Count < 1 || !Enum.TryParse<CassieTtsPayload.SubtitleMode>(arguments.At(0), true, out subtitleMode) || !array.Contains(subtitleMode))
			{
				response = "First argument should specify one of the following subtitle modes: ";
				for (int i = 0; i < array.Length; i++)
				{
					if (i > 0)
					{
						response += ", ";
					}
					response += array[i].ToString();
				}
				return false;
			}
			bool playBackground;
			if (arguments.Count < 2 || !bool.TryParse(arguments.At(1), out playBackground))
			{
				response = "Second argument should be a bool specifying if background noises should play.";
				return false;
			}
			if (arguments.Count < 3)
			{
				response = "Announcement is empty";
				return false;
			}
			if (subtitleMode == CassieTtsPayload.SubtitleMode.Custom)
			{
				string text = CassieAdvancedCommand.PlayCustomSubtitles(arguments, playBackground);
				if (!string.IsNullOrEmpty(text))
				{
					response = text;
					return false;
				}
			}
			else
			{
				string customAnnouncement = RAUtils.FormatArguments(arguments, 2, null).ToUpper();
				bool autoGenerateSubtitles = subtitleMode == CassieTtsPayload.SubtitleMode.Automatic;
				new CassieAnnouncement(new CassieTtsPayload(customAnnouncement, autoGenerateSubtitles, playBackground), 0f, 0f).AddToQueue();
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " started an advanced cassie announcement: " + RAUtils.FormatArguments(arguments, 0, null) + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			response = "Announcement sent!";
			return true;
		}

		// Token: 0x060038D2 RID: 14546 RVA: 0x000B935C File Offset: 0x000B755C
		private static string PlayCustomSubtitles(ArraySegment<string> arguments, bool playBackground)
		{
			int num;
			if (!int.TryParse(arguments.At(2), out num))
			{
				return "For custom subtitles, the third argument should represent the number of words to be used as the custom subtitle content.";
			}
			if (arguments.Count < 4)
			{
				return "Announcement is empty";
			}
			string customSubtitles = RAUtils.FormatArguments(arguments, 3, new int?(num));
			new CassieAnnouncement(new CassieTtsPayload(RAUtils.FormatArguments(arguments, 3 + num, null).ToUpper(), customSubtitles, playBackground), 0f, 0f).AddToQueue();
			return null;
		}
	}
}
