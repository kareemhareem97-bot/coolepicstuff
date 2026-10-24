using System;
using System.Runtime.CompilerServices;
using CursorManagement;
using InventorySystem.Items;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Scp3114;
using PlayerStatsSystem;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008E3 RID: 2275
	public class Strangled : StatusEffectBase, ISoundtrackMutingEffect, IMovementInputOverride, IStaminaModifier, IMovementSpeedModifier, ICursorOverride, IInteractionBlocker
	{
		// Token: 0x17000A17 RID: 2583
		// (get) Token: 0x06003410 RID: 13328 RVA: 0x00004070 File Offset: 0x00002270
		public bool MuteSoundtrack
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x17000A18 RID: 2584
		// (get) Token: 0x06003411 RID: 13329 RVA: 0x00004070 File Offset: 0x00002270
		public bool MovementOverrideActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x17000A19 RID: 2585
		// (get) Token: 0x06003412 RID: 13330 RVA: 0x000ABCC5 File Offset: 0x000A9EC5
		// (set) Token: 0x06003413 RID: 13331 RVA: 0x000ABCCD File Offset: 0x000A9ECD
		public Vector3 MovementOverrideDirection { get; private set; }

		// Token: 0x17000A1A RID: 2586
		// (get) Token: 0x06003414 RID: 13332 RVA: 0x00004070 File Offset: 0x00002270
		public bool StaminaModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x17000A1B RID: 2587
		// (get) Token: 0x06003415 RID: 13333 RVA: 0x00004070 File Offset: 0x00002270
		public bool SprintingDisabled
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x17000A1C RID: 2588
		// (get) Token: 0x06003416 RID: 13334 RVA: 0x00004070 File Offset: 0x00002270
		public bool MovementModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x17000A1D RID: 2589
		// (get) Token: 0x06003417 RID: 13335 RVA: 0x0000408D File Offset: 0x0000228D
		public float MovementSpeedMultiplier
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x17000A1E RID: 2590
		// (get) Token: 0x06003418 RID: 13336 RVA: 0x000ABCD6 File Offset: 0x000A9ED6
		// (set) Token: 0x06003419 RID: 13337 RVA: 0x000ABCDE File Offset: 0x000A9EDE
		public float MovementSpeedLimit { get; private set; }

		// Token: 0x17000A1F RID: 2591
		// (get) Token: 0x0600341A RID: 13338 RVA: 0x00004094 File Offset: 0x00002294
		public CursorOverrideMode CursorOverride
		{
			get
			{
				return CursorOverrideMode.NoOverride;
			}
		}

		// Token: 0x17000A20 RID: 2592
		// (get) Token: 0x0600341B RID: 13339 RVA: 0x00004070 File Offset: 0x00002270
		public bool LockMovement
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x17000A21 RID: 2593
		// (get) Token: 0x0600341C RID: 13340 RVA: 0x000A84BF File Offset: 0x000A66BF
		public override bool AllowEnabling
		{
			get
			{
				return !SpawnProtected.CheckPlayer(base.Hub);
			}
		}

		// Token: 0x17000A22 RID: 2594
		// (get) Token: 0x0600341D RID: 13341 RVA: 0x000ABCE8 File Offset: 0x000A9EE8
		private float DamageRate
		{
			get
			{
				double elapsedSeconds = this.ElapsedSeconds;
				if (elapsedSeconds < 0.0)
				{
					return 0f;
				}
				double num = 5.0 * elapsedSeconds;
				return (float)(5.0 + num);
			}
		}

		// Token: 0x17000A23 RID: 2595
		// (get) Token: 0x0600341E RID: 13342 RVA: 0x000ABD28 File Offset: 0x000A9F28
		public float EstimatedTimeToKill
		{
			get
			{
				StatBase module = base.Hub.playerStats.GetModule<HealthStat>();
				AhpStat module2 = base.Hub.playerStats.GetModule<AhpStat>();
				float num = module.CurValue + module2.CurValue;
				float damageRate = this.DamageRate;
				float num2 = damageRate * damageRate;
				return (-damageRate + Mathf.Sqrt(num2 + 10f * num)) / 5f;
			}
		}

		// Token: 0x17000A24 RID: 2596
		// (get) Token: 0x0600341F RID: 13343 RVA: 0x000ABD83 File Offset: 0x000A9F83
		public double ElapsedSeconds
		{
			get
			{
				return NetworkTime.time - this._startTime;
			}
		}

		// Token: 0x17000A25 RID: 2597
		// (get) Token: 0x06003420 RID: 13344 RVA: 0x000ABD91 File Offset: 0x000A9F91
		public BlockedInteraction BlockedInteractions
		{
			get
			{
				return (BlockedInteraction)50;
			}
		}

		// Token: 0x17000A26 RID: 2598
		// (get) Token: 0x06003421 RID: 13345 RVA: 0x0000549C File Offset: 0x0000369C
		public bool CanBeCleared
		{
			get
			{
				return !base.IsEnabled;
			}
		}

		// Token: 0x06003422 RID: 13346 RVA: 0x000ABD98 File Offset: 0x000A9F98
		protected override void Enabled()
		{
			base.Enabled();
			this._startTime = NetworkTime.time;
			this._remainingLineOfSightTolerance = 0.5f;
			this.MovementSpeedLimit = 0f;
			this.MovementOverrideDirection = Vector3.zero;
			base.Hub.interCoordinator.AddBlocker(this);
			IFpcRole fpcRole = base.Hub.roleManager.CurrentRole as IFpcRole;
			if (fpcRole != null)
			{
				fpcRole.FpcModule.Motor.RegisterMovementLock(this);
				this._overrideRegistered = true;
			}
		}

		// Token: 0x06003423 RID: 13347 RVA: 0x000ABE1C File Offset: 0x000AA01C
		protected override void Disabled()
		{
			base.Disabled();
			IFpcRole fpcRole = base.Hub.roleManager.CurrentRole as IFpcRole;
			if (fpcRole != null && this._overrideRegistered)
			{
				fpcRole.FpcModule.Motor.UnregisterMovementLock(this);
			}
		}

		// Token: 0x06003424 RID: 13348 RVA: 0x000ABE61 File Offset: 0x000AA061
		protected override void OnEffectUpdate()
		{
			base.OnEffectUpdate();
			if (base.IsLocalPlayer)
			{
				this.UpdateInputOverride();
			}
			if (NetworkServer.active && !this.ServerUpdate())
			{
				this.DisableEffect();
			}
		}

		// Token: 0x06003425 RID: 13349 RVA: 0x000ABE8C File Offset: 0x000AA08C
		private bool ServerUpdate()
		{
			ReferenceHub referenceHub;
			if (!this.TryUpdateAttacker(out referenceHub))
			{
				return false;
			}
			Vector3 position = base.Hub.PlayerCameraReference.position;
			Vector3 position2 = referenceHub.PlayerCameraReference.position;
			if (!Physics.Linecast(position, position2, FpcStateProcessor.Mask))
			{
				this._remainingLineOfSightTolerance = 0.5f;
			}
			else
			{
				this._remainingLineOfSightTolerance -= Time.deltaTime;
				if (this._remainingLineOfSightTolerance < 0f)
				{
					return false;
				}
			}
			base.Hub.inventory.ServerSelectItem(0);
			return base.Hub.playerStats.DealDamage(new Scp3114DamageHandler(referenceHub, this.DamageRate * Time.deltaTime, Scp3114DamageHandler.HandlerType.Strangulation));
		}

		// Token: 0x06003426 RID: 13350 RVA: 0x000ABF38 File Offset: 0x000AA138
		private void UpdateInputOverride()
		{
			ReferenceHub referenceHub;
			if (!this.TryUpdateAttacker(out referenceHub))
			{
				return;
			}
			IFpcRole fpcRole = base.Hub.roleManager.CurrentRole as IFpcRole;
			if (fpcRole == null)
			{
				return;
			}
			FirstPersonMovementModule fpcModule = fpcRole.FpcModule;
			fpcRole.LookAtPoint(this._strangleTarget.AttackerPosition.Position, Time.deltaTime * 20f);
			Vector3 vector = this._strangleTarget.TargetPosition.Position - fpcModule.Position;
			float num = vector.MagnitudeIgnoreY();
			Vector3 movementOverrideDirection = vector / num;
			if (Mathf.Approximately(num, 0f))
			{
				return;
			}
			this.MovementOverrideDirection = movementOverrideDirection;
			this.MovementSpeedLimit = Mathf.Min(5f, num / Time.deltaTime);
		}

		// Token: 0x06003427 RID: 13351 RVA: 0x000ABFF0 File Offset: 0x000AA1F0
		public bool TryUpdateAttacker(out ReferenceHub attacker)
		{
			if (this._hasCache && this._cachedHub != null && this.<TryUpdateAttacker>g__CheckPlayer|52_0(this._cachedHub))
			{
				attacker = this._cachedHub;
				return true;
			}
			foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
			{
				if (this.<TryUpdateAttacker>g__CheckPlayer|52_0(referenceHub))
				{
					attacker = referenceHub;
					this._hasCache = true;
					return true;
				}
			}
			this._hasCache = false;
			attacker = null;
			return false;
		}

		// Token: 0x06003428 RID: 13352 RVA: 0x000AC090 File Offset: 0x000AA290
		private void OnDestroy()
		{
			IFpcRole fpcRole = base.Hub.roleManager.CurrentRole as IFpcRole;
			if (fpcRole != null && this._overrideRegistered)
			{
				fpcRole.FpcModule.Motor.UnregisterMovementLock(this);
			}
		}

		// Token: 0x0600342A RID: 13354 RVA: 0x000AC0D0 File Offset: 0x000AA2D0
		[CompilerGenerated]
		private bool <TryUpdateAttacker>g__CheckPlayer|52_0(ReferenceHub hub)
		{
			Scp3114Role scp3114Role = hub.roleManager.CurrentRole as Scp3114Role;
			if (scp3114Role == null)
			{
				return false;
			}
			Scp3114Strangle scp3114Strangle;
			if (!scp3114Role.SubroutineModule.TryGetSubroutine<Scp3114Strangle>(out scp3114Strangle))
			{
				return false;
			}
			if (scp3114Strangle.SyncTarget == null)
			{
				return false;
			}
			if (scp3114Strangle.SyncTarget.Value.Target != base.Hub)
			{
				return false;
			}
			this._cachedHub = hub;
			this._strangleTarget = scp3114Strangle.SyncTarget.Value;
			return true;
		}

		// Token: 0x04002806 RID: 10246
		private bool _hasCache;

		// Token: 0x04002807 RID: 10247
		private bool _overrideRegistered;

		// Token: 0x04002808 RID: 10248
		private double _startTime;

		// Token: 0x04002809 RID: 10249
		private float _remainingLineOfSightTolerance;

		// Token: 0x0400280A RID: 10250
		private ReferenceHub _cachedHub;

		// Token: 0x0400280B RID: 10251
		private Scp3114Strangle.StrangleTarget _strangleTarget;

		// Token: 0x0400280C RID: 10252
		private const float MaxMovementSpeed = 5f;

		// Token: 0x0400280D RID: 10253
		private const float LookLerp = 20f;

		// Token: 0x0400280E RID: 10254
		private const float StartDamageRate = 5f;

		// Token: 0x0400280F RID: 10255
		private const float DamageRatePerSec = 5f;

		// Token: 0x04002810 RID: 10256
		private const float LineOfSightToleranceSeconds = 0.5f;
	}
}
