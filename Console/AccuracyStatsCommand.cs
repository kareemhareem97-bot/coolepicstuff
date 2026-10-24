using System;
using InventorySystem.Crosshairs;

namespace CommandSystem.Commands.Console
{
	// Token: 0x02000A88 RID: 2696
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class AccuracyStatsCommand : ICommand
	{
		// Token: 0x17000DB4 RID: 3508
		// (get) Token: 0x06003CDA RID: 15578 RVA: 0x000C82E5 File Offset: 0x000C64E5
		public string Command { get; } = "accuracystats";

		// Token: 0x17000DB5 RID: 3509
		// (get) Token: 0x06003CDB RID: 15579 RVA: 0x000C82ED File Offset: 0x000C64ED
		public string[] Aliases { get; } = new string[]
		{
			"accstats",
			"inaccstats",
			"inaccuracystats",
			"statsacc",
			"statsinacc"
		};

		// Token: 0x17000DB6 RID: 3510
		// (get) Token: 0x06003CDC RID: 15580 RVA: 0x000C82F5 File Offset: 0x000C64F5
		public string Description { get; } = "Replaces gun crosshair with one that displays current inaccuracy.";

		// Token: 0x17000DB7 RID: 3511
		// (get) Token: 0x06003CDD RID: 15581 RVA: 0x000C82FD File Offset: 0x000C64FD
		// (set) Token: 0x06003CDE RID: 15582 RVA: 0x000C8304 File Offset: 0x000C6504
		private bool Enabled
		{
			get
			{
				return DebugStatsCrosshair.Enabled;
			}
			set
			{
				DebugStatsCrosshair.Enabled = value;
			}
		}

		// Token: 0x06003CDF RID: 15583 RVA: 0x000C830C File Offset: 0x000C650C
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			this.Enabled = !this.Enabled;
			response = "Accuracy stats crosshair status: " + (this.Enabled ? "Enabled" : "Disabled") + "\nIf changes aren't applied automatically, re-equip your firearm.";
			ReferenceHub referenceHub;
			if (ReferenceHub.TryGetLocalHub(out referenceHub))
			{
				CrosshairController.Refresh(referenceHub, referenceHub.inventory.CurItem.SerialNumber);
			}
			return true;
		}
	}
}
