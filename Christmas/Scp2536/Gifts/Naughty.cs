using System;
using Footprinting;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using Mirror;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace Christmas.Scp2536.Gifts
{
	// Token: 0x020010E4 RID: 4324
	public class Naughty : Scp2536ItemGift
	{
		// Token: 0x170013F1 RID: 5105
		// (get) Token: 0x06006038 RID: 24632 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override UrgencyLevel Urgency
		{
			get
			{
				return UrgencyLevel.Exclusive;
			}
		}

		// Token: 0x170013F2 RID: 5106
		// (get) Token: 0x06006039 RID: 24633 RVA: 0x00002DA5 File Offset: 0x00000FA5
		internal override bool IgnoredByRandomness
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170013F3 RID: 5107
		// (get) Token: 0x0600603A RID: 24634 RVA: 0x00141900 File Offset: 0x0013FB00
		protected override Scp2536Reward[] Rewards
		{
			get
			{
				return new Scp2536Reward[]
				{
					new Scp2536Reward(ItemType.Coal, 45f),
					new Scp2536Reward(ItemType.SpecialCoal, 5f),
					new Scp2536Reward(ItemType.GrenadeFlash, 10f),
					new Scp2536Reward(ItemType.GrenadeHE, 10f),
					new Scp2536Reward(ItemType.SCP018, 10f),
					new Scp2536Reward(ItemType.None, 10f),
					new Scp2536Reward(ItemType.Scp021J, 10f)
				};
			}
		}

		// Token: 0x0600603B RID: 24635 RVA: 0x00141998 File Offset: 0x0013FB98
		public override bool CanBeGranted(ReferenceHub hub)
		{
			return UnityEngine.Random.Range(0f, 100f) <= 50f && Scp2536GiftController.Gifts.Count((Scp2536GiftBase g) => g.ObtainedBy.Contains(hub)) > 3;
		}

		// Token: 0x0600603C RID: 24636 RVA: 0x001419E8 File Offset: 0x0013FBE8
		public override void ServerGrant(ReferenceHub hub)
		{
			ItemType itemType = base.GenerateRandomReward();
			if (itemType == ItemType.GrenadeFlash || itemType == ItemType.GrenadeHE)
			{
				this.SpawnProjectile(itemType, hub, new Action<ThrownProjectile>(this.SetupEffectGrenade));
				return;
			}
			if (itemType == ItemType.SCP018)
			{
				for (int i = 0; i < 3; i++)
				{
					this.SpawnProjectile(itemType, hub, new Action<ThrownProjectile>(this.SetupScp018));
				}
				return;
			}
			if (itemType != ItemType.None)
			{
				hub.inventory.ServerAddItem(itemType, ItemAddReason.Scp2536, 0, null).GrantAmmoReward();
				return;
			}
			Scp956Pinata scp956Pinata;
			if (!Scp956Pinata.TryGetInstance(out scp956Pinata))
			{
				return;
			}
			foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
			{
				referenceHub.playerEffectsController.DisableEffect<Scp956Target>();
			}
			Scp956Pinata.ActiveTargets.Clear();
			scp956Pinata.SpawnBehindTarget(hub);
		}

		// Token: 0x0600603D RID: 24637 RVA: 0x00141AC0 File Offset: 0x0013FCC0
		private void SetupEffectGrenade(ThrownProjectile projectile)
		{
			projectile.ServerActivate();
		}

		// Token: 0x0600603E RID: 24638 RVA: 0x00141AC8 File Offset: 0x0013FCC8
		private void SetupScp018(ThrownProjectile projectile)
		{
			projectile.GetComponent<Rigidbody>().linearVelocity = UnityEngine.Random.onUnitSphere * 15f;
		}

		// Token: 0x0600603F RID: 24639 RVA: 0x00141AE4 File Offset: 0x0013FCE4
		private void SpawnProjectile(ItemType id, ReferenceHub hub, Action<ThrownProjectile> setupMethod)
		{
			ThrowableItem throwableItem;
			if (!InventoryItemLoader.TryGetItem<ThrowableItem>(id, out throwableItem))
			{
				return;
			}
			ThrownProjectile thrownProjectile = UnityEngine.Object.Instantiate<ThrownProjectile>(throwableItem.Projectile, hub.transform.position, Quaternion.identity);
			PickupSyncInfo networkInfo = new PickupSyncInfo(id, throwableItem.Weight, ItemSerialGenerator.GenerateNext(), false)
			{
				Locked = true
			};
			thrownProjectile.NetworkInfo = networkInfo;
			thrownProjectile.PreviousOwner = new Footprint(hub);
			setupMethod(thrownProjectile);
			NetworkServer.Spawn(thrownProjectile.gameObject, null);
		}

		// Token: 0x0400461B RID: 17947
		private const float GiftReplacementChance = 50f;

		// Token: 0x0400461C RID: 17948
		private const int MaxGiftOpenings = 3;
	}
}
