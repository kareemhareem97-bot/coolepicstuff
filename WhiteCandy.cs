using System;
using System.Collections.Generic;
using AudioPooling;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Armor;
using MapGeneration;
using MapGeneration.Holidays;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.FirstPersonControl.Thirdperson;
using PlayerRoles.FirstPersonControl.Thirdperson.Subcontrollers.Wearables;
using PlayerRoles.PlayableScps.Scp106;
using PlayerRoles.Subroutines;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace CustomPlayerEffects
{
	// Token: 0x020008ED RID: 2285
	public class WhiteCandy : StatusEffectBase, IFpcCollisionModifier, IInteractionBlocker, IHolidayEffect
	{
		// Token: 0x17000A43 RID: 2627
		// (get) Token: 0x0600346B RID: 13419 RVA: 0x0006CB8D File Offset: 0x0006AD8D
		public LayerMask DetectionMask
		{
			get
			{
				return Scp106MovementModule.PassableDetectionMask;
			}
		}

		// Token: 0x17000A44 RID: 2628
		// (get) Token: 0x0600346C RID: 13420 RVA: 0x00040288 File Offset: 0x0003E488
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Positive;
			}
		}

		// Token: 0x17000A45 RID: 2629
		// (get) Token: 0x0600346D RID: 13421 RVA: 0x000409F8 File Offset: 0x0003EBF8
		public BlockedInteraction BlockedInteractions
		{
			get
			{
				return BlockedInteraction.GrabItems;
			}
		}

		// Token: 0x17000A46 RID: 2630
		// (get) Token: 0x0600346E RID: 13422 RVA: 0x0000549C File Offset: 0x0000369C
		public bool CanBeCleared
		{
			get
			{
				return !base.IsEnabled;
			}
		}

		// Token: 0x17000A47 RID: 2631
		// (get) Token: 0x0600346F RID: 13423 RVA: 0x000AC8D5 File Offset: 0x000AAAD5
		public HolidayType[] TargetHolidays { get; } = new HolidayType[]
		{
			HolidayType.Halloween
		};

		// Token: 0x06003470 RID: 13424 RVA: 0x000AC8E0 File Offset: 0x000AAAE0
		public void ProcessColliders(ArraySegment<Collider> detections)
		{
			float num = 0f;
			foreach (Collider collider in detections)
			{
				bool flag;
				float slowdownFromCollider = Scp106MovementModule.GetSlowdownFromCollider(collider, out flag);
				if (flag)
				{
					num = Mathf.Max(num, slowdownFromCollider);
					collider.enabled = (slowdownFromCollider == 0f);
				}
			}
			if (!NetworkServer.active)
			{
				return;
			}
			if (this._inDoor)
			{
				this._inDoor = (num > 0f);
				return;
			}
			if (num < 0.7f)
			{
				return;
			}
			this._inDoor = true;
			if (!this._rateLimit.IsReady)
			{
				return;
			}
			this._rateLimit.Trigger(1.2000000476837158);
			NetworkServer.SendToReady<GhostlyWalkThroughMessage>(new GhostlyWalkThroughMessage
			{
				Position = base.Hub.transform.position
			}, 0);
		}

		// Token: 0x06003471 RID: 13425 RVA: 0x000AC9CC File Offset: 0x000AABCC
		protected override void Enabled()
		{
			base.Enabled();
			if (this._loopSound.isPlaying)
			{
				this._loopSound.Stop();
			}
			this._loopSound.mute = true;
			this._loopSound.Play();
			this._loopSound.volume = 1f;
			if (NetworkServer.active)
			{
				base.Hub.playerEffectsController.DisableEffect<OrangeCandy>();
				base.Hub.playerEffectsController.DisableEffect<Metal>();
				base.Hub.DisableWearables(WearableElements.Armor);
			}
			IFpcRole fpcRole = base.Hub.roleManager.CurrentRole as IFpcRole;
			if (fpcRole == null)
			{
				return;
			}
			WhiteCandy.FlickerLights(fpcRole.FpcModule.Position);
			FpcCollisionProcessor.AddModifier(this, fpcRole);
			base.Hub.interCoordinator.AddBlocker(this);
			this._modelEffect = new ModelVisualEffect(fpcRole.FpcModule.CharacterModelInstance, this._ghostMaterial);
		}

		// Token: 0x06003472 RID: 13426 RVA: 0x000ACAB4 File Offset: 0x000AACB4
		protected override void Disabled()
		{
			base.Disabled();
			this.ReEnableArmor();
			if (NetworkServer.active)
			{
				base.Hub.playerEffectsController.ChangeState<TemporaryBypass>(2, 3f, false);
			}
			FpcCollisionProcessor.RemoveModifier(this);
			ModelVisualEffect modelEffect = this._modelEffect;
			if (modelEffect == null)
			{
				return;
			}
			modelEffect.Restore();
		}

		// Token: 0x06003473 RID: 13427 RVA: 0x000ACB04 File Offset: 0x000AAD04
		private void ReEnableArmor()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			if (base.Hub.inventory.UserInventory.Items.Any((KeyValuePair<ushort, ItemBase> x) => x.Value is BodyArmor))
			{
				base.Hub.EnableWearables(WearableElements.Armor);
			}
		}

		// Token: 0x06003474 RID: 13428 RVA: 0x000ACB60 File Offset: 0x000AAD60
		protected override void Update()
		{
			base.Update();
			if (!this._loopSound.isPlaying)
			{
				return;
			}
			if (!base.IsEnabled)
			{
				this._loopSound.volume -= Time.deltaTime / 2f;
				if (this._loopSound.volume <= 0f)
				{
					this._loopSound.Stop();
				}
			}
			this._loopSound.mute = (!base.IsLocalPlayer && !base.IsSpectated);
		}

		// Token: 0x06003475 RID: 13429 RVA: 0x000ACBE4 File Offset: 0x000AADE4
		private static void FlickerLights(Vector3 pos)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			RoomIdentifier roomIdentifier;
			if (!pos.TryGetRoom(out roomIdentifier))
			{
				return;
			}
			foreach (RoomLightController roomLightController in roomIdentifier.LightControllers)
			{
				if (roomLightController.LightsEnabled)
				{
					roomLightController.ServerFlickerLights(0.02f);
				}
			}
		}

		// Token: 0x06003476 RID: 13430 RVA: 0x000ACC58 File Offset: 0x000AAE58
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			Inventory.OnCurrentItemChanged += delegate(ReferenceHub hub, ItemIdentifier _, ItemIdentifier newItem)
			{
				if (!NetworkServer.active)
				{
					return;
				}
				WhiteCandy whiteCandy;
				if (!WhiteCandy.TryGetEffectFast(hub, out whiteCandy) || !whiteCandy.IsEnabled)
				{
					return;
				}
				hub.inventory.ServerDropItem(newItem.SerialNumber);
			};
			CustomNetworkManager.OnClientReady += delegate()
			{
				NetworkClient.ReplaceHandler<GhostlyWalkThroughMessage>(delegate(GhostlyWalkThroughMessage msg)
				{
					ReferenceHub hub;
					if (!ReferenceHub.TryGetLocalHub(out hub))
					{
						return;
					}
					WhiteCandy whiteCandy;
					if (!WhiteCandy.TryGetEffectFast(hub, out whiteCandy))
					{
						return;
					}
					AudioSourcePoolManager.PlayAtPosition(whiteCandy._walkThroughClip, msg.Position, 8f, 1f, FalloffType.Exponential, MixerChannel.DefaultSfx, 1f);
				}, true);
			};
		}

		// Token: 0x06003477 RID: 13431 RVA: 0x000ACCB0 File Offset: 0x000AAEB0
		private static bool TryGetEffectFast(ReferenceHub hub, out WhiteCandy fx)
		{
			StatusEffectBase[] allEffects = hub.playerEffectsController.AllEffects;
			if (!WhiteCandy._whiteCandyEffectIndexCacheSet)
			{
				WhiteCandy._whiteCandyEffectIndexCacheSet = true;
				for (int i = 0; i < allEffects.Length; i++)
				{
					WhiteCandy whiteCandy = allEffects[i] as WhiteCandy;
					if (whiteCandy != null)
					{
						WhiteCandy._whiteCandyEffectIndex = new int?(i);
						fx = whiteCandy;
						return true;
					}
				}
				fx = null;
				return false;
			}
			if (WhiteCandy._whiteCandyEffectIndex == null)
			{
				fx = null;
				return false;
			}
			fx = (allEffects[WhiteCandy._whiteCandyEffectIndex.Value] as WhiteCandy);
			return true;
		}

		// Token: 0x0400282D RID: 10285
		private const float InsideThreshold = 0.7f;

		// Token: 0x0400282E RID: 10286
		private const float OutsideThreshold = 0f;

		// Token: 0x0400282F RID: 10287
		private const float RateLimitCooldown = 1.2f;

		// Token: 0x04002830 RID: 10288
		private const float LightFlickerDuration = 0.02f;

		// Token: 0x04002832 RID: 10290
		[SerializeField]
		private AudioClip _walkThroughClip;

		// Token: 0x04002833 RID: 10291
		[SerializeField]
		private AudioSource _loopSound;

		// Token: 0x04002834 RID: 10292
		[SerializeField]
		private Material _ghostMaterial;

		// Token: 0x04002835 RID: 10293
		private readonly AbilityCooldown _rateLimit = new AbilityCooldown();

		// Token: 0x04002836 RID: 10294
		private ModelVisualEffect _modelEffect;

		// Token: 0x04002837 RID: 10295
		private bool _inDoor;

		// Token: 0x04002838 RID: 10296
		private static int? _whiteCandyEffectIndex;

		// Token: 0x04002839 RID: 10297
		private static bool _whiteCandyEffectIndexCacheSet;
	}
}
