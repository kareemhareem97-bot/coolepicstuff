using System;
using PlayerRoles.Voice;

namespace CommandSystem.Commands.RemoteAdmin.MutingAndIntercom
{
	// Token: 0x02000A4F RID: 2639
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class IntercomTextCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000D12 RID: 3346
		// (get) Token: 0x06003B9E RID: 15262 RVA: 0x000C4886 File Offset: 0x000C2A86
		public string Command { get; } = "intercomtext";

		// Token: 0x17000D13 RID: 3347
		// (get) Token: 0x06003B9F RID: 15263 RVA: 0x000C488E File Offset: 0x000C2A8E
		public string[] Aliases { get; } = new string[]
		{
			"icomtxt"
		};

		// Token: 0x17000D14 RID: 3348
		// (get) Token: 0x06003BA0 RID: 15264 RVA: 0x000C4896 File Offset: 0x000C2A96
		public string Description { get; } = "Changes the intercom text.";

		// Token: 0x17000D15 RID: 3349
		// (get) Token: 0x06003BA1 RID: 15265 RVA: 0x000C489E File Offset: 0x000C2A9E
		public string[] Usage { get; } = new string[]
		{
			"text"
		};

		// Token: 0x06003BA2 RID: 15266 RVA: 0x000C48A8 File Offset: 0x000C2AA8
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.Broadcasting, out response))
			{
				return false;
			}
			if (arguments.Count < 1)
			{
				if (!IntercomDisplay.TrySetDisplay(null))
				{
					response = "Intercom text reset failed. Display not found.";
					return false;
				}
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " cleared the intercom text.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
				response = "Reset intercom text.";
				return true;
			}
			else
			{
				string text = string.Join(" ", arguments).Trim();
				if (!IntercomDisplay.TrySetDisplay(text))
				{
					response = "Intercom text override failed. Display not found.";
					return false;
				}
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " set the intercom text to \"" + text + "\".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
				response = "Set intercom text to: " + text;
				return true;
			}
		}
	}
}
