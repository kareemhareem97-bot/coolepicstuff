using System;

namespace CustomPlayerEffects.Danger
{
	// Token: 0x02000911 RID: 2321
	public abstract class EncounterDangerBase : ParentDangerBase
	{
		// Token: 0x17000A6B RID: 2667
		// (get) Token: 0x060034F3 RID: 13555
		public abstract float DangerPerEncounter { get; }

		// Token: 0x17000A6C RID: 2668
		// (get) Token: 0x060034F4 RID: 13556
		public abstract float DangerPerAdditionalEncounter { get; }

		// Token: 0x060034F5 RID: 13557 RVA: 0x000AD85C File Offset: 0x000ABA5C
		public void RegisterEncounter(ReferenceHub target)
		{
			float dangerValue = base.ChildDangers.IsEmpty<DangerStackBase>() ? this.DangerPerEncounter : this.DangerPerAdditionalEncounter;
			base.ChildDangers.Add(new CachedEncounterDanger(dangerValue, base.Owner, target));
		}

		// Token: 0x060034F6 RID: 13558 RVA: 0x000AD8A0 File Offset: 0x000ABAA0
		public bool WasEncounteredRecently(ReferenceHub hub, out CachedEncounterDanger cachedEncounter)
		{
			cachedEncounter = null;
			foreach (DangerStackBase dangerStackBase in base.ChildDangers)
			{
				CachedEncounterDanger cachedEncounterDanger = (CachedEncounterDanger)dangerStackBase;
				if (!(cachedEncounterDanger.EncounteredHub != hub))
				{
					cachedEncounter = cachedEncounterDanger;
					return true;
				}
			}
			return false;
		}
	}
}
