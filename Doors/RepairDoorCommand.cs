using System;
using Interactables.Interobjects.DoorUtils;

namespace CommandSystem.Commands.RemoteAdmin.Doors
{
	// Token: 0x02000A6B RID: 2667
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class RepairDoorCommand : BaseDoorCommand
	{
		// Token: 0x17000D64 RID: 3428
		// (get) Token: 0x06003C41 RID: 15425 RVA: 0x000C6F2D File Offset: 0x000C512D
		public override string Command { get; } = "repair";

		// Token: 0x17000D65 RID: 3429
		// (get) Token: 0x06003C42 RID: 15426 RVA: 0x000C6F35 File Offset: 0x000C5135
		public override string[] Aliases { get; } = new string[]
		{
			"repairdoor"
		};

		// Token: 0x17000D66 RID: 3430
		// (get) Token: 0x06003C43 RID: 15427 RVA: 0x000C6F3D File Offset: 0x000C513D
		public override string Description { get; } = "Repairs a specified door.";

		// Token: 0x17000D67 RID: 3431
		// (get) Token: 0x06003C44 RID: 15428 RVA: 0x00004094 File Offset: 0x00002294
		public override bool AllowNonDamageableTargets
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06003C45 RID: 15429 RVA: 0x000C6F45 File Offset: 0x000C5145
		protected override void OnTargetFound(DoorVariant door)
		{
			(door as IDamageableDoor).ServerRepair();
		}
	}
}
