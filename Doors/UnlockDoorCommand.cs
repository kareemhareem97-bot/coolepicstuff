using System;
using Interactables.Interobjects.DoorUtils;

namespace CommandSystem.Commands.RemoteAdmin.Doors
{
	// Token: 0x02000A6C RID: 2668
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class UnlockDoorCommand : BaseDoorCommand
	{
		// Token: 0x17000D68 RID: 3432
		// (get) Token: 0x06003C47 RID: 15431 RVA: 0x000C6F85 File Offset: 0x000C5185
		public override string Command { get; } = "unlock";

		// Token: 0x17000D69 RID: 3433
		// (get) Token: 0x06003C48 RID: 15432 RVA: 0x000C6F8D File Offset: 0x000C518D
		public override string[] Aliases { get; } = new string[]
		{
			"unlockdoor",
			"ul"
		};

		// Token: 0x17000D6A RID: 3434
		// (get) Token: 0x06003C49 RID: 15433 RVA: 0x000C6F95 File Offset: 0x000C5195
		public override string Description { get; } = "Unlocks a specified door.";

		// Token: 0x06003C4A RID: 15434 RVA: 0x000C6F9D File Offset: 0x000C519D
		protected override void OnTargetFound(DoorVariant door)
		{
			door.ServerChangeLock(DoorLockReason.AdminCommand, false);
		}
	}
}
