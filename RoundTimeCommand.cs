using System;
using System.Globalization;
using GameCore;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A13 RID: 2579
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class RoundTimeCommand : ICommand
	{
		// Token: 0x17000C5A RID: 3162
		// (get) Token: 0x06003A48 RID: 14920 RVA: 0x000C0491 File Offset: 0x000BE691
		public string Command { get; } = "roundtime";

		// Token: 0x17000C5B RID: 3163
		// (get) Token: 0x06003A49 RID: 14921 RVA: 0x000C0499 File Offset: 0x000BE699
		public string[] Aliases { get; } = new string[]
		{
			"rtime",
			"rt"
		};

		// Token: 0x17000C5C RID: 3164
		// (get) Token: 0x06003A4A RID: 14922 RVA: 0x000C04A1 File Offset: 0x000BE6A1
		public string Description { get; } = "Displays the current round duration.";

		// Token: 0x06003A4B RID: 14923 RVA: 0x000C04AC File Offset: 0x000BE6AC
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (RoundStart.RoundLength.Ticks == 0L)
			{
				response = "The round has not started yet!";
				return false;
			}
			response = "Round time: " + RoundStart.RoundLength.ToString("hh\\:mm\\:ss\\.fff", CultureInfo.InvariantCulture);
			return true;
		}
	}
}
