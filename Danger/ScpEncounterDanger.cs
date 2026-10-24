using System;
using PlayerRoles;
using PlayerRoles.PlayableScps;

namespace CustomPlayerEffects.Danger
{
	// Token: 0x0200091A RID: 2330
	public class ScpEncounterDanger : EncounterDangerBase
	{
		// Token: 0x17000A78 RID: 2680
		// (get) Token: 0x06003527 RID: 13607 RVA: 0x000AE01F File Offset: 0x000AC21F
		// (set) Token: 0x06003528 RID: 13608 RVA: 0x000AD9FB File Offset: 0x000ABBFB
		public override bool IsActive
		{
			get
			{
				this.UpdateState();
				return base.IsActive;
			}
			protected set
			{
				base.IsActive = value;
			}
		}

		// Token: 0x17000A79 RID: 2681
		// (get) Token: 0x06003529 RID: 13609 RVA: 0x000AE02D File Offset: 0x000AC22D
		public override float DangerPerEncounter { get; } = 1f;

		// Token: 0x17000A7A RID: 2682
		// (get) Token: 0x0600352A RID: 13610 RVA: 0x000AE035 File Offset: 0x000AC235
		public override float DangerPerAdditionalEncounter { get; } = 0.5f;

		// Token: 0x0600352B RID: 13611 RVA: 0x000AE040 File Offset: 0x000AC240
		private void UpdateState()
		{
			foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
			{
				if (!(referenceHub == base.Owner) && referenceHub.roleManager.CurrentRole.Team == Team.SCPs && referenceHub.roleManager.CurrentRole.RoleTypeId != RoleTypeId.Scp0492 && VisionInformation.IsInView(base.Owner, referenceHub))
				{
					CachedEncounterDanger cachedEncounterDanger;
					if (base.WasEncounteredRecently(referenceHub, out cachedEncounterDanger))
					{
						cachedEncounterDanger.TimeTracker.Restart();
					}
					else
					{
						base.RegisterEncounter(referenceHub);
					}
				}
			}
		}
	}
}
