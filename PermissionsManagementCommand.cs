using System;
using CommandSystem.Commands.RemoteAdmin.PermissionsManagement;
using CommandSystem.Commands.RemoteAdmin.PermissionsManagement.Group;
using CommandSystem.Commands.Shared;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A01 RID: 2561
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class PermissionsManagementCommand : ParentCommand
	{
		// Token: 0x17000C23 RID: 3107
		// (get) Token: 0x060039DA RID: 14810 RVA: 0x000BE8B5 File Offset: 0x000BCAB5
		public override string Command { get; } = "permissionsmanagement";

		// Token: 0x17000C24 RID: 3108
		// (get) Token: 0x060039DB RID: 14811 RVA: 0x000BE8BD File Offset: 0x000BCABD
		public override string[] Aliases { get; } = new string[]
		{
			"pm"
		};

		// Token: 0x17000C25 RID: 3109
		// (get) Token: 0x060039DC RID: 14812 RVA: 0x000BE8C5 File Offset: 0x000BCAC5
		public override string Description { get; } = "Permissions management system. Type \"pm\" for help.";

		// Token: 0x060039DD RID: 14813 RVA: 0x000BE8CD File Offset: 0x000BCACD
		public static PermissionsManagementCommand Create()
		{
			PermissionsManagementCommand permissionsManagementCommand = new PermissionsManagementCommand();
			permissionsManagementCommand.LoadGeneratedCommands();
			return permissionsManagementCommand;
		}

		// Token: 0x060039DE RID: 14814 RVA: 0x000BE8DA File Offset: 0x000BCADA
		protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (arguments.Count == 0)
			{
				response = "Permissions manager help:\nSyntax: " + arguments.Array[0] + " action\n\nAvailable actions:\ngroups - lists all groups\ngroup info <group name> - prints group info\ngroup grant <group name> <permission name> - grants permission to a group\ngroup revoke <group name> <permission name> - revokes permission from a group\ngroup setcolor <group name> <color name> - sets group color\ngroup settag <group name> <tag> - sets group tag\ngroup enablecover <group name> - enables badge cover for group\ngroup disablecover <group name> - disables badge cover for group\n\nusers - lists all privileged users\nsetgroup <UserID> <group name> - sets membership of user (use group name \"-1\" to remove user from group)\nreload - reloads permission file\n\n\"< >\" are only used to indicate the arguments, don't put them\nMore commands will be added in the next versions of the game";
				return true;
			}
			response = "Unknown subcommand. Type \"pm\" for a list of valid subcommands.";
			return true;
		}

		// Token: 0x060039DF RID: 14815 RVA: 0x000BE909 File Offset: 0x000BCB09
		public override void LoadGeneratedCommands()
		{
			this.RegisterCommand(new GroupsCommand());
			this.RegisterCommand(new CommandSystem.Commands.RemoteAdmin.PermissionsManagement.ReloadCommand());
			this.RegisterCommand(new SetGroupCommand());
			this.RegisterCommand(new UsersCommand());
			this.RegisterCommand(GroupCommand.Create());
		}
	}
}
