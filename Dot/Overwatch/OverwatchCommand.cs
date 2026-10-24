using System;
using RemoteAdmin;

namespace CommandSystem.Commands.Dot.Overwatch
{
	// Token: 0x02000A87 RID: 2695
	[CommandHandler(typeof(ClientCommandHandler))]
	public class OverwatchCommand : ParentCommand
	{
		// Token: 0x17000DB1 RID: 3505
		// (get) Token: 0x06003CD2 RID: 15570 RVA: 0x000C813D File Offset: 0x000C633D
		public override string Command { get; } = "overwatch";

		// Token: 0x17000DB2 RID: 3506
		// (get) Token: 0x06003CD3 RID: 15571 RVA: 0x000C8145 File Offset: 0x000C6345
		public override string[] Aliases { get; } = new string[]
		{
			"ovr",
			"ow"
		};

		// Token: 0x17000DB3 RID: 3507
		// (get) Token: 0x06003CD4 RID: 15572 RVA: 0x000C814D File Offset: 0x000C634D
		public override string Description { get; } = "Toggle overwatch mode.";

		// Token: 0x06003CD5 RID: 15573 RVA: 0x000C8155 File Offset: 0x000C6355
		public static OverwatchCommand Create()
		{
			OverwatchCommand overwatchCommand = new OverwatchCommand();
			overwatchCommand.LoadGeneratedCommands();
			return overwatchCommand;
		}

		// Token: 0x06003CD6 RID: 15574 RVA: 0x000C8162 File Offset: 0x000C6362
		protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (arguments.Count == 0)
			{
				return OverwatchCommand.SetOverwatchStatus(sender, 2, out response);
			}
			response = "SYNTAX: overwatch [enable/disable]";
			return false;
		}

		// Token: 0x06003CD7 RID: 15575 RVA: 0x000C817E File Offset: 0x000C637E
		public override void LoadGeneratedCommands()
		{
			this.RegisterCommand(new DisableCommand());
			this.RegisterCommand(new EnableCommand());
		}

		// Token: 0x06003CD8 RID: 15576 RVA: 0x000C8198 File Offset: 0x000C6398
		internal static bool SetOverwatchStatus(ICommandSender sender, int status, out string response)
		{
			PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
			if (playerCommandSender == null)
			{
				response = "You must be in-game to use this command!";
				return false;
			}
			switch (status)
			{
			case 0:
				if (!playerCommandSender.ReferenceHub.serverRoles.IsInOverwatch)
				{
					response = "Overwatch mode is already disabled.";
					return true;
				}
				playerCommandSender.ReferenceHub.serverRoles.IsInOverwatch = false;
				response = "Overwatch mode has been disabled.";
				return true;
			case 1:
				if (playerCommandSender.ReferenceHub.serverRoles.IsInOverwatch)
				{
					response = "Overwatch mode is already enabled.";
					return true;
				}
				if (!playerCommandSender.CheckPermission(PlayerPermissions.Overwatch, out response))
				{
					return false;
				}
				playerCommandSender.ReferenceHub.serverRoles.IsInOverwatch = true;
				response = "Overwatch mode has been enabled.";
				return true;
			case 2:
				if (playerCommandSender.ReferenceHub.serverRoles.IsInOverwatch)
				{
					playerCommandSender.ReferenceHub.serverRoles.IsInOverwatch = false;
					response = "Overwatch mode has been disabled.";
					return true;
				}
				if (!playerCommandSender.CheckPermission(PlayerPermissions.Overwatch, out response))
				{
					return false;
				}
				playerCommandSender.ReferenceHub.serverRoles.IsInOverwatch = true;
				response = "Overwatch mode has been enabled.";
				return true;
			default:
				response = "Unknown error occured in overwatch command.";
				return false;
			}
		}
	}
}
