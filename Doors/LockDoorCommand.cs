using System;
using Interactables.Interobjects.DoorUtils;

namespace CommandSystem.Commands.RemoteAdmin.Doors
{
	// Token: 0x02000A67 RID: 2663
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class LockDoorCommand : BaseDoorCommand
	{
		// Token: 0x17000D5A RID: 3418
		// (get) Token: 0x06003C2E RID: 15406 RVA: 0x000C6BD9 File Offset: 0x000C4DD9
		public override string Command { get; } = "lock";

		// Token: 0x17000D5B RID: 3419
		// (get) Token: 0x06003C2F RID: 15407 RVA: 0x000C6BE1 File Offset: 0x000C4DE1
		public override string[] Aliases { get; } = new string[]
		{
			"lockdoor",
			"l"
		};

		// Token: 0x17000D5C RID: 3420
		// (get) Token: 0x06003C30 RID: 15408 RVA: 0x000C6BE9 File Offset: 0x000C4DE9
		public override string Description { get; } = "Locks a specified door.";

		// Token: 0x06003C31 RID: 15409 RVA: 0x000C6BF1 File Offset: 0x000C4DF1
		protected override void OnTargetFound(DoorVariant door)
		{
			door.ServerChangeLock(DoorLockReason.AdminCommand, true);
		}
	}
}
