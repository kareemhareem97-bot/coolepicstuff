using System;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp079;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A15 RID: 2581
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class AddExperienceCommand : Scp079CommandBase
	{
		// Token: 0x17000C61 RID: 3169
		// (get) Token: 0x06003A53 RID: 14931 RVA: 0x000C0741 File Offset: 0x000BE941
		public override string Command { get; } = "addexperience";

		// Token: 0x17000C62 RID: 3170
		// (get) Token: 0x06003A54 RID: 14932 RVA: 0x000C0749 File Offset: 0x000BE949
		public override string[] Aliases { get; } = new string[]
		{
			"addexp",
			"add079exp",
			"addxp"
		};

		// Token: 0x17000C63 RID: 3171
		// (get) Token: 0x06003A55 RID: 14933 RVA: 0x000C0751 File Offset: 0x000BE951
		public override string Description { get; } = "Adds the specified experience of the player playing as SCP-079.";

		// Token: 0x17000C64 RID: 3172
		// (get) Token: 0x06003A56 RID: 14934 RVA: 0x000C0759 File Offset: 0x000BE959
		public override string[] Usage { get; } = new string[]
		{
			"%player%",
			"Experience"
		};

		// Token: 0x06003A57 RID: 14935 RVA: 0x000C0761 File Offset: 0x000BE961
		public override void ApplyChanges(Scp079TierManager manager, int input)
		{
			manager.ServerGrantExperience(input, Scp079HudTranslation.ExpGainAdminCommand, RoleTypeId.None);
		}
	}
}
