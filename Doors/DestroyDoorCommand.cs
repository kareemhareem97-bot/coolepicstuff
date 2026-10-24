using System;
using Footprinting;
using Interactables.Interobjects.DoorUtils;

namespace CommandSystem.Commands.RemoteAdmin.Doors
{
	// Token: 0x02000A63 RID: 2659
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class DestroyDoorCommand : BaseDoorCommand
	{
		// Token: 0x17000D4F RID: 3407
		// (get) Token: 0x06003C17 RID: 15383 RVA: 0x000C66EB File Offset: 0x000C48EB
		public override string Command { get; } = "destroy";

		// Token: 0x17000D50 RID: 3408
		// (get) Token: 0x06003C18 RID: 15384 RVA: 0x000C66F3 File Offset: 0x000C48F3
		public override string[] Aliases { get; } = new string[]
		{
			"destroydoor"
		};

		// Token: 0x17000D51 RID: 3409
		// (get) Token: 0x06003C19 RID: 15385 RVA: 0x000C66FB File Offset: 0x000C48FB
		public override string Description { get; } = "Destroys a specified door.";

		// Token: 0x17000D52 RID: 3410
		// (get) Token: 0x06003C1A RID: 15386 RVA: 0x00004094 File Offset: 0x00002294
		public override bool AllowNonDamageableTargets
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06003C1B RID: 15387 RVA: 0x000C6704 File Offset: 0x000C4904
		protected override void OnTargetFound(DoorVariant door)
		{
			(door as IDamageableDoor).ServerDamage(65535f, DoorDamageType.ServerCommand, default(Footprint));
		}
	}
}
