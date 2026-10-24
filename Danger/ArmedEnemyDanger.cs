using System;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Jailbird;
using PlayerRoles;
using PlayerRoles.PlayableScps;

namespace CustomPlayerEffects.Danger
{
	// Token: 0x02000913 RID: 2323
	public class ArmedEnemyDanger : EncounterDangerBase
	{
		// Token: 0x17000A70 RID: 2672
		// (get) Token: 0x06003501 RID: 13569 RVA: 0x000AD9ED File Offset: 0x000ABBED
		// (set) Token: 0x06003502 RID: 13570 RVA: 0x000AD9FB File Offset: 0x000ABBFB
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

		// Token: 0x17000A71 RID: 2673
		// (get) Token: 0x06003503 RID: 13571 RVA: 0x000ADA04 File Offset: 0x000ABC04
		public override float DangerPerEncounter { get; } = 1f;

		// Token: 0x17000A72 RID: 2674
		// (get) Token: 0x06003504 RID: 13572 RVA: 0x000ADA0C File Offset: 0x000ABC0C
		public override float DangerPerAdditionalEncounter { get; } = 0.25f;

		// Token: 0x06003505 RID: 13573 RVA: 0x000ADA14 File Offset: 0x000ABC14
		private void UpdateState()
		{
			foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
			{
				if (!(referenceHub == base.Owner) && referenceHub.IsHuman())
				{
					if (!HitboxIdentity.IsEnemy(base.Owner, referenceHub))
					{
						break;
					}
					if (VisionInformation.IsInView(base.Owner, referenceHub) && this.IsArmed(referenceHub))
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

		// Token: 0x06003506 RID: 13574 RVA: 0x000ADABC File Offset: 0x000ABCBC
		private bool IsArmed(ReferenceHub hub)
		{
			foreach (ItemBase itemBase in hub.inventory.UserInventory.Items.Values)
			{
				if (itemBase is Firearm || itemBase is JailbirdItem)
				{
					return true;
				}
			}
			return false;
		}
	}
}
