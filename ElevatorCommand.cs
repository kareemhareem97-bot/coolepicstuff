using System;
using Interactables.Interobjects;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009E3 RID: 2531
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class ElevatorCommand : ParentCommand, IUsageProvider
	{
		// Token: 0x17000BBE RID: 3006
		// (get) Token: 0x06003923 RID: 14627 RVA: 0x000BA771 File Offset: 0x000B8971
		public override string Command { get; } = "elevator";

		// Token: 0x17000BBF RID: 3007
		// (get) Token: 0x06003924 RID: 14628 RVA: 0x000BA779 File Offset: 0x000B8979
		public override string[] Aliases { get; } = new string[]
		{
			"lift",
			"elev",
			"el",
			"lt"
		};

		// Token: 0x17000BC0 RID: 3008
		// (get) Token: 0x06003925 RID: 14629 RVA: 0x000BA781 File Offset: 0x000B8981
		public override string Description { get; } = "Allows to check status of all elevators and force an elevator to move to a specific floor.";

		// Token: 0x17000BC1 RID: 3009
		// (get) Token: 0x06003926 RID: 14630 RVA: 0x000BA789 File Offset: 0x000B8989
		public string[] Usage { get; } = new string[]
		{
			"list/lock/send/teleport/unlock",
			"Elevator ID / \"all\""
		};

		// Token: 0x06003927 RID: 14631 RVA: 0x000BA791 File Offset: 0x000B8991
		public static ElevatorCommand Create()
		{
			ElevatorCommand elevatorCommand = new ElevatorCommand();
			elevatorCommand.LoadGeneratedCommands();
			return elevatorCommand;
		}

		// Token: 0x06003928 RID: 14632 RVA: 0x000BA79E File Offset: 0x000B899E
		protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = "Please specify a valid subcommand!\n- elevator list [\"detailed\"]\n- elevator lock <Elevator ID / \"all\"> <level / \"dynamic\" (optional)>\n- elevator send <Elevator ID / \"all\"> [level ID]\n- elevator teleport <Elevator ID> <Target Players> [\"inside\"/level ID/\"outside\"]\n- elevator unlock <Elevator ID / \"all\"> <level (optional)>\n";
			return false;
		}

		// Token: 0x06003929 RID: 14633 RVA: 0x000BA7A8 File Offset: 0x000B89A8
		public override void LoadGeneratedCommands()
		{
			this.RegisterCommand(new ElevatorListCommand());
			this.RegisterCommand(new ElevatorLockCommand());
			this.RegisterCommand(new ElevatorSendCommand());
			this.RegisterCommand(new ElevatorTeleportCommand());
			this.RegisterCommand(new ElevatorUnlockCommand());
		}

		// Token: 0x0600392A RID: 14634 RVA: 0x000BA7E4 File Offset: 0x000B89E4
		internal static bool TryParseGroup(string txt, out ElevatorGroup group)
		{
			int num;
			if (!int.TryParse(txt, out num))
			{
				return Enum.TryParse<ElevatorGroup>(txt, true, out group);
			}
			group = (ElevatorGroup)num;
			return Enum.IsDefined(typeof(ElevatorGroup), group);
		}
	}
}
