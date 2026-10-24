using System;

namespace CommandSystem.Commands.RemoteAdmin.Dms
{
	// Token: 0x02000A6E RID: 2670
	[CommandHandler(typeof(DmsCommand))]
	public class ForceCommand : ICommand
	{
		// Token: 0x17000D6F RID: 3439
		// (get) Token: 0x06003C54 RID: 15444 RVA: 0x000C705E File Offset: 0x000C525E
		public string Command
		{
			get
			{
				return "force";
			}
		}

		// Token: 0x17000D70 RID: 3440
		// (get) Token: 0x06003C55 RID: 15445 RVA: 0x000C7065 File Offset: 0x000C5265
		public string[] Aliases
		{
			get
			{
				return new string[]
				{
					"f"
				};
			}
		}

		// Token: 0x17000D71 RID: 3441
		// (get) Token: 0x06003C56 RID: 15446 RVA: 0x000C7075 File Offset: 0x000C5275
		public string Description
		{
			get
			{
				return "Forces the Deadman's Switch to begin.";
			}
		}

		// Token: 0x06003C57 RID: 15447 RVA: 0x000C707C File Offset: 0x000C527C
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.WarheadEvents, out response))
			{
				return false;
			}
			if (DeadmanSwitch.IsSequenceActive)
			{
				response = "DMS is already in progress.";
				return false;
			}
			DeadmanSwitch.InitiateProtocol();
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " forced the DMS sequence.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			response = "DMS sequence has been forced.";
			return true;
		}
	}
}
