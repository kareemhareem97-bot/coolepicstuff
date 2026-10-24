using System;
using PlayerRoles;
using PlayerRoles.PlayableScps;

namespace CustomPlayerEffects.Danger
{
	// Token: 0x0200091C RID: 2332
	public class ZombieEncounterDanger : EncounterDangerBase
	{
		// Token: 0x17000A7C RID: 2684
		// (get) Token: 0x06003534 RID: 13620 RVA: 0x000AE1B1 File Offset: 0x000AC3B1
		// (set) Token: 0x06003535 RID: 13621 RVA: 0x000AD9FB File Offset: 0x000ABBFB
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

		// Token: 0x17000A7D RID: 2685
		// (get) Token: 0x06003536 RID: 13622 RVA: 0x000AE1BF File Offset: 0x000AC3BF
		public override float DangerPerEncounter { get; } = 0.25f;

		// Token: 0x17000A7E RID: 2686
		// (get) Token: 0x06003537 RID: 13623 RVA: 0x000AE1C7 File Offset: 0x000AC3C7
		public override float DangerPerAdditionalEncounter { get; } = 0.25f;

		// Token: 0x06003538 RID: 13624 RVA: 0x000AE1D0 File Offset: 0x000AC3D0
		private void UpdateState()
		{
			foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
			{
				if (!(referenceHub == base.Owner) && referenceHub.roleManager.CurrentRole.RoleTypeId == RoleTypeId.Scp0492 && VisionInformation.IsInView(base.Owner, referenceHub))
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
