using System;
using PlayerRoles.PlayableScps.Scp079;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A17 RID: 2583
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class SetExperienceCommand : Scp079CommandBase
	{
		// Token: 0x17000C69 RID: 3177
		// (get) Token: 0x06003A61 RID: 14945 RVA: 0x000C09AC File Offset: 0x000BEBAC
		public override string Command { get; } = "setexperience";

		// Token: 0x17000C6A RID: 3178
		// (get) Token: 0x06003A62 RID: 14946 RVA: 0x000C09B4 File Offset: 0x000BEBB4
		public override string[] Aliases { get; } = new string[]
		{
			"setexp",
			"set079exp",
			"setxp",
			"exp"
		};

		// Token: 0x17000C6B RID: 3179
		// (get) Token: 0x06003A63 RID: 14947 RVA: 0x000C09BC File Offset: 0x000BEBBC
		public override string Description { get; } = "Sets the experience of the player playing as SCP-079.";

		// Token: 0x17000C6C RID: 3180
		// (get) Token: 0x06003A64 RID: 14948 RVA: 0x000C09C4 File Offset: 0x000BEBC4
		public override string[] Usage { get; } = new string[]
		{
			"%player%",
			"New Experience"
		};

		// Token: 0x06003A65 RID: 14949 RVA: 0x000C09CC File Offset: 0x000BEBCC
		public override void ApplyChanges(Scp079TierManager manager, int input)
		{
			manager.TotalExp = input;
		}
	}
}
