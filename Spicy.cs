using System;
using System.Collections.Generic;
using AudioPooling;
using Footprinting;
using InventorySystem.Items;
using MapGeneration.Holidays;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerStatsSystem;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008E1 RID: 2273
	public class Spicy : TickingEffectBase, IInteractionBlocker, IHolidayEffect
	{
		// Token: 0x17000A0C RID: 2572
		// (get) Token: 0x060033FF RID: 13311 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool AllowEnabling
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000A0D RID: 2573
		// (get) Token: 0x06003400 RID: 13312 RVA: 0x0000549C File Offset: 0x0000369C
		public bool CanBeCleared
		{
			get
			{
				return !base.IsEnabled;
			}
		}

		// Token: 0x17000A0E RID: 2574
		// (get) Token: 0x06003401 RID: 13313 RVA: 0x00034727 File Offset: 0x00032927
		public BlockedInteraction BlockedInteractions
		{
			get
			{
				return BlockedInteraction.OpenInventory;
			}
		}

		// Token: 0x17000A0F RID: 2575
		// (get) Token: 0x06003402 RID: 13314 RVA: 0x000AB8E7 File Offset: 0x000A9AE7
		public HolidayType[] TargetHolidays { get; } = new HolidayType[]
		{
			HolidayType.Halloween
		};

		// Token: 0x06003403 RID: 13315 RVA: 0x000AB8F0 File Offset: 0x000A9AF0
		protected override void Enabled()
		{
			base.Enabled();
			this._vfxTimer = 0;
			if (this._initialScreechClip != null)
			{
				AudioSourcePoolManager.PlayOnTransform(this._initialScreechClip, base.Hub.transform, 12f, 0.4f, FalloffType.Exponential, MixerChannel.DefaultSfx, 1f);
			}
			base.Hub.interCoordinator.AddBlocker(this);
		}

		// Token: 0x06003404 RID: 13316 RVA: 0x000AB951 File Offset: 0x000A9B51
		protected override void Disabled()
		{
			base.Disabled();
		}

		// Token: 0x06003405 RID: 13317 RVA: 0x000AB95C File Offset: 0x000A9B5C
		protected override void OnTick()
		{
			bool flag = this._vfxTimer <= 3;
			bool flag2 = this._vfxTimer % 4 == 0;
			if (this._flameBreathVFX != null && flag2 && !flag)
			{
				float d = base.Hub.isLocalPlayer ? -0.55f : -0.35f;
				Vector3 position = base.Hub.PlayerCameraReference.position + Vector3.up * d;
				WorldPlayerVFX worldPlayerVFX;
				if (UnityEngine.Object.Instantiate<GameObject>(this._flameBreathVFX, position, base.Hub.PlayerCameraReference.rotation, base.Hub.transform).TryGetComponent<WorldPlayerVFX>(out worldPlayerVFX))
				{
					worldPlayerVFX.Assign(base.Hub);
				}
			}
			this._vfxTimer++;
			if (!NetworkServer.active || flag)
			{
				return;
			}
			IFpcRole fpcRole = base.Hub.roleManager.CurrentRole as IFpcRole;
			if (fpcRole == null)
			{
				return;
			}
			Vector3 position2 = fpcRole.FpcModule.Position;
			bool flag3 = false;
			foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
			{
				if (referenceHub != base.Hub)
				{
					IFpcRole fpcRole2 = referenceHub.roleManager.CurrentRole as IFpcRole;
					if (fpcRole2 != null)
					{
						Vector3 a = fpcRole2.FpcModule.Position - position2;
						float sqrMagnitude = a.sqrMagnitude;
						if (sqrMagnitude <= 25f)
						{
							float num = Mathf.Sqrt(sqrMagnitude);
							Vector3 vector = Mathf.Approximately(num, 0f) ? Vector3.zero : (a / num);
							float num2 = Vector3.Dot(vector, base.Hub.PlayerCameraReference.forward);
							if ((num < 0.7f || num2 >= 0.75f) && !Physics.Raycast(position2, vector, num, PlayerRolesUtils.AttackMask) && this._alreadyDamagedEntities.Add(referenceHub.networkIdentity.netId))
							{
								float num3 = 15f;
								HumeShieldStat humeShieldStat;
								if (referenceHub.playerStats.TryGetModule<HumeShieldStat>(out humeShieldStat))
								{
									if (num3 * 3f < humeShieldStat.CurValue)
									{
										num3 *= 3f;
									}
									else
									{
										num3 += humeShieldStat.CurValue / 3f;
									}
								}
								ExplosionDamageHandler explosionDamageHandler = new ExplosionDamageHandler(new Footprint(base.Hub), Vector3.zero, num3, 100, ExplosionType.Custom);
								if (referenceHub.playerStats.DealDamage(explosionDamageHandler))
								{
									flag3 |= Hitmarker.CheckHitmarkerPerms(explosionDamageHandler, referenceHub);
									referenceHub.playerEffectsController.EnableEffect<Burned>(60f, false);
								}
							}
						}
					}
				}
			}
			if (flag3)
			{
				Hitmarker.SendHitmarkerDirectly(base.Hub, 1f, true, HitmarkerType.Regular);
			}
			this._alreadyDamagedEntities.Clear();
		}

		// Token: 0x040027F5 RID: 10229
		private const int VfxInterval = 4;

		// Token: 0x040027F6 RID: 10230
		private const float EnemyDotProductThreshold = 0.75f;

		// Token: 0x040027F7 RID: 10231
		private const float OmnidirectionalRange = 0.7f;

		// Token: 0x040027F8 RID: 10232
		private const float ForwardRange = 5f;

		// Token: 0x040027F9 RID: 10233
		private const float SfxRange = 12f;

		// Token: 0x040027FA RID: 10234
		private const float TickDamage = 15f;

		// Token: 0x040027FC RID: 10236
		[SerializeField]
		private AudioClip _fireBreathingClip;

		// Token: 0x040027FD RID: 10237
		[SerializeField]
		private AudioClip _initialScreechClip;

		// Token: 0x040027FE RID: 10238
		[SerializeField]
		private GameObject _flameBreathVFX;

		// Token: 0x040027FF RID: 10239
		private readonly HashSet<uint> _alreadyDamagedEntities = new HashSet<uint>();

		// Token: 0x04002800 RID: 10240
		private int _vfxTimer;
	}
}
