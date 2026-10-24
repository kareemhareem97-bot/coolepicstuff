using System;
using System.Collections.Generic;
using Mirror;
using PlayerRoles.FirstPersonControl;
using UnityEngine;
using Utils;

namespace CustomPlayerEffects
{
	// Token: 0x020008AA RID: 2218
	public abstract class CokeBase<TStack> : CokeBase, IHealableEffect, IMovementSpeedModifier, IConflictableEffect where TStack : ICokeStack
	{
		// Token: 0x17000968 RID: 2408
		// (get) Token: 0x06003268 RID: 12904
		public abstract Dictionary<PlayerMovementState, float> StateMultipliers { get; }

		// Token: 0x17000969 RID: 2409
		// (get) Token: 0x06003269 RID: 12905
		public abstract float MovementSpeedMultiplier { get; }

		// Token: 0x1700096A RID: 2410
		// (get) Token: 0x0600326A RID: 12906 RVA: 0x00004070 File Offset: 0x00002270
		public bool MovementModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x1700096B RID: 2411
		// (get) Token: 0x0600326B RID: 12907 RVA: 0x0000407F File Offset: 0x0000227F
		public float MovementSpeedLimit
		{
			get
			{
				return float.MaxValue;
			}
		}

		// Token: 0x1700096C RID: 2412
		// (get) Token: 0x0600326C RID: 12908 RVA: 0x000A8D04 File Offset: 0x000A6F04
		// (set) Token: 0x0600326D RID: 12909 RVA: 0x000A8D0C File Offset: 0x000A6F0C
		private protected TStack[] StackMultipliers { protected get; private set; }

		// Token: 0x1700096D RID: 2413
		// (get) Token: 0x0600326E RID: 12910 RVA: 0x000A8D15 File Offset: 0x000A6F15
		// (set) Token: 0x0600326F RID: 12911 RVA: 0x000A8D1D File Offset: 0x000A6F1D
		private protected TStack CurrentStack { protected get; private set; }

		// Token: 0x1700096E RID: 2414
		// (get) Token: 0x06003270 RID: 12912 RVA: 0x000A8D26 File Offset: 0x000A6F26
		// (set) Token: 0x06003271 RID: 12913 RVA: 0x000A8D2E File Offset: 0x000A6F2E
		private protected bool GoingToExplode
		{
			protected get
			{
				return this._goingToExplode;
			}
			private set
			{
				this._goingToExplode = value;
				this._explosionTimer = 0f;
			}
		}

		// Token: 0x06003272 RID: 12914 RVA: 0x00004242 File Offset: 0x00002442
		public bool IsHealable(ItemType it)
		{
			return it == ItemType.SCP500;
		}

		// Token: 0x06003273 RID: 12915 RVA: 0x000A8D44 File Offset: 0x000A6F44
		protected float GetMovementStateMultiplier()
		{
			IFpcRole fpcRole = base.Hub.roleManager.CurrentRole as IFpcRole;
			if (fpcRole == null)
			{
				return 1f;
			}
			PlayerMovementState playerMovementState = fpcRole.FpcModule.CurrentMovementState;
			float num = base.Hub.GetVelocity().SqrMagnitudeIgnoreY();
			if (playerMovementState == PlayerMovementState.Walking && num == 0f)
			{
				playerMovementState = PlayerMovementState.Crouching;
			}
			float result;
			if (!this.StateMultipliers.TryGetValue(playerMovementState, out result))
			{
				return 1f;
			}
			return result;
		}

		// Token: 0x06003274 RID: 12916 RVA: 0x000A8DB7 File Offset: 0x000A6FB7
		protected override void Awake()
		{
			base.Awake();
			this.CurrentStack = this.StackMultipliers[0];
		}

		// Token: 0x06003275 RID: 12917 RVA: 0x000A8DD1 File Offset: 0x000A6FD1
		protected override void IntensityChanged(byte prevState, byte newState)
		{
			this.CurrentStack = this.StackMultipliers[Mathf.Clamp((int)newState, 0, this.StackMultipliers.Length - 1)];
		}

		// Token: 0x06003276 RID: 12918 RVA: 0x000A8DF5 File Offset: 0x000A6FF5
		protected override void Enabled()
		{
			base.Enabled();
			this.GoingToExplode = false;
		}

		// Token: 0x06003277 RID: 12919 RVA: 0x000A8E04 File Offset: 0x000A7004
		protected override void Disabled()
		{
			base.Disabled();
			this.GoingToExplode = false;
		}

		// Token: 0x06003278 RID: 12920 RVA: 0x000A8E14 File Offset: 0x000A7014
		protected override void OnEffectUpdate()
		{
			base.OnEffectUpdate();
			if (!NetworkServer.active)
			{
				return;
			}
			if (!this.GoingToExplode)
			{
				return;
			}
			this._explosionTimer += Time.deltaTime;
			if (this._explosionTimer < 1.05f)
			{
				return;
			}
			ExplosionUtils.ServerExplode(base.Hub, ExplosionType.Cola);
			this.GoingToExplode = false;
		}

		// Token: 0x06003279 RID: 12921 RVA: 0x000A8E6C File Offset: 0x000A706C
		public virtual bool CheckConflicts(StatusEffectBase other)
		{
			if (other is Scp1853)
			{
				Poisoned poisoned;
				if (!base.Hub.playerEffectsController.TryGetEffect<Poisoned>(out poisoned))
				{
					return false;
				}
				if (!poisoned.IsEnabled)
				{
					poisoned.ForceIntensity(1);
				}
				return true;
			}
			else
			{
				CokeBase cokeBase = other as CokeBase;
				if (cokeBase == null)
				{
					return false;
				}
				this.GoingToExplode = true;
				cokeBase.ServerDisable();
				return true;
			}
		}

		// Token: 0x0400274F RID: 10063
		private const float ConflictExplosionDelay = 1.05f;

		// Token: 0x04002752 RID: 10066
		private bool _goingToExplode;

		// Token: 0x04002753 RID: 10067
		private float _explosionTimer;
	}
}
