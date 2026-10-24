using System;
using Interactables.Interobjects.DoorUtils;

namespace CommandSystem.Commands.RemoteAdmin.Doors
{
	// Token: 0x02000A6A RID: 2666
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class OpenDoorCommand : BaseDoorCommand
	{
		// Token: 0x17000D61 RID: 3425
		// (get) Token: 0x06003C3C RID: 15420 RVA: 0x000C6ED2 File Offset: 0x000C50D2
		public override string Command { get; } = "open";

		// Token: 0x17000D62 RID: 3426
		// (get) Token: 0x06003C3D RID: 15421 RVA: 0x000C6EDA File Offset: 0x000C50DA
		public override string[] Aliases { get; } = new string[]
		{
			"opendoor",
			"o"
		};

		// Token: 0x17000D63 RID: 3427
		// (get) Token: 0x06003C3E RID: 15422 RVA: 0x000C6EE2 File Offset: 0x000C50E2
		public override string Description { get; } = "Opens a specified door.";

		// Token: 0x06003C3F RID: 15423 RVA: 0x000C6EEA File Offset: 0x000C50EA
		protected override void OnTargetFound(DoorVariant door)
		{
			door.NetworkTargetState = true;
		}
	}
}
