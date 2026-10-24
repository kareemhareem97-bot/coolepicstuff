using System;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009E9 RID: 2537
	[CommandHandler(typeof(ElevatorCommand))]
	public class ElevatorUnlockCommand : ICommand
	{
		// Token: 0x17000BCE RID: 3022
		// (get) Token: 0x06003949 RID: 14665 RVA: 0x000BB65D File Offset: 0x000B985D
		public string Command { get; } = "unlock";

		// Token: 0x17000BCF RID: 3023
		// (get) Token: 0x0600394A RID: 14666 RVA: 0x000BB665 File Offset: 0x000B9865
		public string[] Aliases { get; } = new string[]
		{
			"u",
			"ul",
			"ulck"
		};

		// Token: 0x17000BD0 RID: 3024
		// (get) Token: 0x0600394B RID: 14667 RVA: 0x000BB66D File Offset: 0x000B986D
		public string Description { get; } = "Unlocks an elevator.";

		// Token: 0x0600394C RID: 14668 RVA: 0x000BB678 File Offset: 0x000B9878
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
			{
				return false;
			}
			int count = arguments.Count;
			if (count != 1)
			{
				if (count == 2)
				{
					int num;
					if (int.TryParse(arguments.At(1), out num) && num >= 0)
					{
						return ElevatorLockCommand.TrySetLock(arguments.At(0), false, out response, sender, num);
					}
				}
				response = "Syntax error: elevator unlock <Elevator ID / \"all\"> <level (optional)>";
				return false;
			}
			return ElevatorLockCommand.TrySetLock(arguments.At(0), false, out response, sender, -1);
		}
	}
}
