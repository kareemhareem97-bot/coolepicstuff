using System;

namespace CommandSystem.Commands.RemoteAdmin.Dms
{
	// Token: 0x02000A71 RID: 2673
	[CommandHandler(typeof(DmsCommand))]
	public class ToggleCommand : ICommand
	{
		// Token: 0x17000D78 RID: 3448
		// (get) Token: 0x06003C63 RID: 15459 RVA: 0x000C71B5 File Offset: 0x000C53B5
		public string Command
		{
			get
			{
				return "toggle";
			}
		}

		// Token: 0x17000D79 RID: 3449
		// (get) Token: 0x06003C64 RID: 15460 RVA: 0x000C71BC File Offset: 0x000C53BC
		public string[] Aliases
		{
			get
			{
				return new string[]
				{
					"t"
				};
			}
		}

		// Token: 0x17000D7A RID: 3450
		// (get) Token: 0x06003C65 RID: 15461 RVA: 0x000C71CC File Offset: 0x000C53CC
		public string Description
		{
			get
			{
				return "Toggles whether the Deadman's Switch is enabled and can tick down.";
			}
		}

		// Token: 0x06003C66 RID: 15462 RVA: 0x000C71D4 File Offset: 0x000C53D4
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.WarheadEvents, out response))
			{
				return false;
			}
			DeadmanSwitch.ForceCountdownToggle = !DeadmanSwitch.ForceCountdownToggle;
			bool forceCountdownToggle = DeadmanSwitch.ForceCountdownToggle;
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " toggled " + (forceCountdownToggle ? "on" : "off") + " the DMS sequence countdown.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			response = "DMS sequence countdown has been toggled " + (forceCountdownToggle ? "on" : "off") + ".";
			return true;
		}
	}
}
