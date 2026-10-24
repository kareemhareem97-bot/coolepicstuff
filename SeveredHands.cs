using System;
using InventorySystem;
using InventorySystem.Items;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl.Thirdperson;
using PlayerStatsSystem;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008D8 RID: 2264
	public class SeveredHands : TickingEffectBase, IInteractionBlocker
	{
		// Token: 0x170009EF RID: 2543
		// (get) Token: 0x060033BD RID: 13245 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool AllowEnabling
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170009F0 RID: 2544
		// (get) Token: 0x060033BE RID: 13246 RVA: 0x0000549C File Offset: 0x0000369C
		public bool CanBeCleared
		{
			get
			{
				return !base.IsEnabled;
			}
		}

		// Token: 0x170009F1 RID: 2545
		// (get) Token: 0x060033BF RID: 13247 RVA: 0x00005495 File Offset: 0x00003695
		public BlockedInteraction BlockedInteractions
		{
			get
			{
				return BlockedInteraction.All;
			}
		}

		// Token: 0x060033C0 RID: 13248 RVA: 0x000AB385 File Offset: 0x000A9585
		protected override void Enabled()
		{
			base.Enabled();
			base.Hub.interCoordinator.AddBlocker(this);
			this.ChangeHandsState(true);
		}

		// Token: 0x060033C1 RID: 13249 RVA: 0x000AB3A5 File Offset: 0x000A95A5
		protected override void Disabled()
		{
			base.Disabled();
			this.ChangeHandsState(false);
		}

		// Token: 0x060033C2 RID: 13250 RVA: 0x000AB3B4 File Offset: 0x000A95B4
		protected override void OnTick()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			base.Hub.inventory.ServerDropItem(base.Hub.inventory.CurItem.SerialNumber);
			base.Hub.playerStats.DealDamage(new UniversalDamageHandler(this._tickDamage, DeathTranslations.SeveredHands, null));
		}

		// Token: 0x060033C3 RID: 13251 RVA: 0x000AB414 File Offset: 0x000A9614
		private void ChangeHandsState(bool handsCut)
		{
			HumanRole humanRole = base.Hub.roleManager.CurrentRole as HumanRole;
			if (humanRole == null)
			{
				return;
			}
			AnimatedCharacterModel animatedCharacterModel = humanRole.FpcModule.CharacterModelInstance as AnimatedCharacterModel;
			if (animatedCharacterModel == null)
			{
				return;
			}
			if (animatedCharacterModel.HasParameter(SeveredHands.HashSeveredHands))
			{
				animatedCharacterModel.Animator.SetBool(SeveredHands.HashSeveredHands, handsCut);
			}
		}

		// Token: 0x040027DF RID: 10207
		private const BlockedInteraction Interactions = BlockedInteraction.All;

		// Token: 0x040027E0 RID: 10208
		private static readonly int HashSeveredHands = Animator.StringToHash("SeveredHands");

		// Token: 0x040027E1 RID: 10209
		[SerializeField]
		private float _tickDamage;
	}
}
