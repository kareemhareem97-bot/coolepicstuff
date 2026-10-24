using System;
using Interactables.Interobjects.DoorUtils;

namespace CommandSystem.Commands.RemoteAdmin.Doors
{
	// Token: 0x02000A62 RID: 2658
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class CloseDoorCommand : BaseDoorCommand
	{
		// Token: 0x17000D4C RID: 3404
		// (get) Token: 0x06003C12 RID: 15378 RVA: 0x000C6690 File Offset: 0x000C4890
		public override string Command { get; } = "close";

		// Token: 0x17000D4D RID: 3405
		// (get) Token: 0x06003C13 RID: 15379 RVA: 0x000C6698 File Offset: 0x000C4898
		public override string[] Aliases { get; } = new string[]
		{
			"closedoor",
			"c"
		};

		// Token: 0x17000D4E RID: 3406
		// (get) Token: 0x06003C14 RID: 15380 RVA: 0x000C66A0 File Offset: 0x000C48A0
		public override string Description { get; } = "Closes a specified door.";

		// Token: 0x06003C15 RID: 15381 RVA: 0x000C66A8 File Offset: 0x000C48A8
		protected override void OnTargetFound(DoorVariant door)
		{
			door.NetworkTargetState = false;
		}
	}
}
