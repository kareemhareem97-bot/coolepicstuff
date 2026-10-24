using System;
using PlayerRoles.PlayableScps.Scp096;
using RemoteAdmin;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A22 RID: 2594
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class StateCommand : ICommand
	{
		// Token: 0x17000C95 RID: 3221
		// (get) Token: 0x06003AA7 RID: 15015 RVA: 0x000C1B5F File Offset: 0x000BFD5F
		public string Command { get; } = "096state";

		// Token: 0x17000C96 RID: 3222
		// (get) Token: 0x06003AA8 RID: 15016 RVA: 0x000C1B67 File Offset: 0x000BFD67
		public string[] Aliases { get; } = new string[]
		{
			"state096",
			"state"
		};

		// Token: 0x17000C97 RID: 3223
		// (get) Token: 0x06003AA9 RID: 15017 RVA: 0x000C1B6F File Offset: 0x000BFD6F
		public string Description { get; } = "Prints which state you are in server-side as 096.";

		// Token: 0x17000C98 RID: 3224
		// (get) Token: 0x06003AAA RID: 15018 RVA: 0x000C1B77 File Offset: 0x000BFD77
		public string[] Usage { get; } = new string[]
		{
			"a"
		};

		// Token: 0x06003AAB RID: 15019 RVA: 0x000C1B80 File Offset: 0x000BFD80
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(new PlayerPermissions[]
			{
				PlayerPermissions.ForceclassSelf,
				PlayerPermissions.ForceclassWithoutRestrictions
			}, out response))
			{
				return false;
			}
			PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
			if (playerCommandSender == null)
			{
				response = "Only players can run this command.";
				return false;
			}
			Scp096Role scp096Role = playerCommandSender.ReferenceHub.roleManager.CurrentRole as Scp096Role;
			if (scp096Role == null)
			{
				response = "You must be SCP-096 to use this command.";
				return false;
			}
			response = string.Format("Your current state server-side is: RageState: {0} | AbilityState: {1}", scp096Role.StateController.RageState, scp096Role.StateController.AbilityState);
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + "'s " + response, ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			return true;
		}
	}
}
