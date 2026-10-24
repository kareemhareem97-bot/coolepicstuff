using System;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using Mirror;
using PlayerRoles;
using PlayerRoles.Spectating;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x02000906 RID: 2310
	public abstract class StatusEffectBase : MonoBehaviour, IEquatable<StatusEffectBase>
	{
		// Token: 0x17000A54 RID: 2644
		// (get) Token: 0x060034A3 RID: 13475 RVA: 0x000AD083 File Offset: 0x000AB283
		// (set) Token: 0x060034A4 RID: 13476 RVA: 0x000AD08B File Offset: 0x000AB28B
		public byte Intensity
		{
			get
			{
				return this._intensity;
			}
			set
			{
				if (value > this._intensity && !this.AllowEnabling)
				{
					return;
				}
				this.ForceIntensity(value);
			}
		}

		// Token: 0x17000A55 RID: 2645
		// (get) Token: 0x060034A5 RID: 13477 RVA: 0x000AD0A6 File Offset: 0x000AB2A6
		public virtual byte MaxIntensity { get; } = 255;

		// Token: 0x17000A56 RID: 2646
		// (get) Token: 0x060034A6 RID: 13478 RVA: 0x000AD0AE File Offset: 0x000AB2AE
		// (set) Token: 0x060034A7 RID: 13479 RVA: 0x000AD0B9 File Offset: 0x000AB2B9
		public bool IsEnabled
		{
			get
			{
				return this.Intensity > 0;
			}
			set
			{
				if (value == this.IsEnabled)
				{
					return;
				}
				this.Intensity = (value ? 1 : 0);
			}
		}

		// Token: 0x17000A57 RID: 2647
		// (get) Token: 0x060034A8 RID: 13480 RVA: 0x000AD0D2 File Offset: 0x000AB2D2
		public virtual bool AllowEnabling
		{
			get
			{
				return this.Classification != StatusEffectBase.EffectClassification.Negative || (!SpawnProtected.CheckPlayer(this.Hub) && !Vitality.CheckPlayer(this.Hub));
			}
		}

		// Token: 0x17000A58 RID: 2648
		// (get) Token: 0x060034A9 RID: 13481 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public virtual StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Negative;
			}
		}

		// Token: 0x17000A59 RID: 2649
		// (get) Token: 0x060034AA RID: 13482 RVA: 0x000AD0FC File Offset: 0x000AB2FC
		public bool IsLocalPlayer
		{
			get
			{
				return this.Hub.isLocalPlayer;
			}
		}

		// Token: 0x17000A5A RID: 2650
		// (get) Token: 0x060034AB RID: 13483 RVA: 0x000AD109 File Offset: 0x000AB309
		public bool IsSpectated
		{
			get
			{
				return this.Hub.IsLocallySpectated();
			}
		}

		// Token: 0x17000A5B RID: 2651
		// (get) Token: 0x060034AC RID: 13484 RVA: 0x000AD116 File Offset: 0x000AB316
		public bool IsPOV
		{
			get
			{
				return this.Hub.IsPOV;
			}
		}

		// Token: 0x17000A5C RID: 2652
		// (get) Token: 0x060034AD RID: 13485 RVA: 0x000AD123 File Offset: 0x000AB323
		// (set) Token: 0x060034AE RID: 13486 RVA: 0x000AD12B File Offset: 0x000AB32B
		public float Duration
		{
			get
			{
				return this._duration;
			}
			private set
			{
				this._duration = Mathf.Max(0f, value);
			}
		}

		// Token: 0x17000A5D RID: 2653
		// (get) Token: 0x060034AF RID: 13487 RVA: 0x000AD13E File Offset: 0x000AB33E
		// (set) Token: 0x060034B0 RID: 13488 RVA: 0x000AD146 File Offset: 0x000AB346
		public float TimeLeft
		{
			get
			{
				return this._timeLeft;
			}
			set
			{
				this._timeLeft = Mathf.Max(0f, value);
				if (this._timeLeft == 0f && this.Duration != 0f)
				{
					this.DisableEffect();
				}
			}
		}

		// Token: 0x17000A5E RID: 2654
		// (get) Token: 0x060034B1 RID: 13489 RVA: 0x000AD179 File Offset: 0x000AB379
		// (set) Token: 0x060034B2 RID: 13490 RVA: 0x000AD181 File Offset: 0x000AB381
		public ReferenceHub Hub { get; private set; }

		// Token: 0x060034B3 RID: 13491 RVA: 0x000AD18A File Offset: 0x000AB38A
		[Server]
		public void ServerSetState(byte intensity, float duration = 0f, bool addDuration = false)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void CustomPlayerEffects.StatusEffectBase::ServerSetState(System.Byte,System.Single,System.Boolean)' called when server was not active");
				return;
			}
			this.Intensity = intensity;
			this.ServerChangeDuration(duration, addDuration);
		}

		// Token: 0x060034B4 RID: 13492 RVA: 0x000AD1B0 File Offset: 0x000AB3B0
		[Server]
		public void ServerDisable()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void CustomPlayerEffects.StatusEffectBase::ServerDisable()' called when server was not active");
				return;
			}
			this.DisableEffect();
		}

		// Token: 0x060034B5 RID: 13493 RVA: 0x000AD1D0 File Offset: 0x000AB3D0
		[Server]
		public void ServerChangeDuration(float duration, bool addDuration = false)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void CustomPlayerEffects.StatusEffectBase::ServerChangeDuration(System.Single,System.Boolean)' called when server was not active");
				return;
			}
			if (addDuration && duration > 0f)
			{
				this.Duration += duration;
				this.TimeLeft += duration;
				return;
			}
			this.Duration = duration;
			this.TimeLeft = this.Duration;
		}

		// Token: 0x060034B6 RID: 13494 RVA: 0x000AD230 File Offset: 0x000AB430
		public void ForceIntensity(byte value)
		{
			if (this._intensity == value)
			{
				return;
			}
			byte intensity = this._intensity;
			bool active = NetworkServer.active;
			bool flag = intensity == 0 && value > 0;
			if (active)
			{
				PlayerEffectUpdatingEventArgs playerEffectUpdatingEventArgs = new PlayerEffectUpdatingEventArgs(this.Hub, this, value, this.Duration);
				PlayerEvents.OnUpdatingEffect(playerEffectUpdatingEventArgs);
				if (!playerEffectUpdatingEventArgs.IsAllowed)
				{
					return;
				}
				value = playerEffectUpdatingEventArgs.Intensity;
				this.Duration = playerEffectUpdatingEventArgs.Duration;
			}
			this._intensity = (byte)Mathf.Min((int)value, (int)this.MaxIntensity);
			if (active)
			{
				this.Hub.playerEffectsController.ServerSyncEffect(this);
				PlayerEvents.OnUpdatedEffect(new PlayerEffectUpdatedEventArgs(this.Hub, this, value, this.Duration));
			}
			if (flag)
			{
				this.Enabled();
				Action<StatusEffectBase> onEnabled = StatusEffectBase.OnEnabled;
				if (onEnabled != null)
				{
					onEnabled(this);
				}
			}
			else if (intensity > 0 && value == 0)
			{
				this.Disabled();
				Action<StatusEffectBase> onDisabled = StatusEffectBase.OnDisabled;
				if (onDisabled != null)
				{
					onDisabled(this);
				}
			}
			this.IntensityChanged(intensity, value);
			Action<StatusEffectBase, byte, byte> onIntensityChanged = StatusEffectBase.OnIntensityChanged;
			if (onIntensityChanged == null)
			{
				return;
			}
			onIntensityChanged(this, intensity, value);
		}

		// Token: 0x060034B7 RID: 13495 RVA: 0x000AD32C File Offset: 0x000AB52C
		protected virtual void Awake()
		{
			this.Hub = ReferenceHub.GetHub(base.transform.root.gameObject);
		}

		// Token: 0x060034B8 RID: 13496 RVA: 0x00002E9A File Offset: 0x0000109A
		protected virtual void Start()
		{
		}

		// Token: 0x060034B9 RID: 13497 RVA: 0x000AD349 File Offset: 0x000AB549
		protected virtual void Update()
		{
			if (!this.IsEnabled)
			{
				return;
			}
			this.RefreshTime();
			this.OnEffectUpdate();
		}

		// Token: 0x060034BA RID: 13498 RVA: 0x000AD360 File Offset: 0x000AB560
		private void RefreshTime()
		{
			if (this.Duration == 0f)
			{
				return;
			}
			this.TimeLeft -= Time.deltaTime;
		}

		// Token: 0x060034BB RID: 13499 RVA: 0x00002E9A File Offset: 0x0000109A
		protected virtual void Enabled()
		{
		}

		// Token: 0x060034BC RID: 13500 RVA: 0x00002E9A File Offset: 0x0000109A
		protected virtual void Disabled()
		{
		}

		// Token: 0x060034BD RID: 13501 RVA: 0x00002E9A File Offset: 0x0000109A
		protected virtual void OnEffectUpdate()
		{
		}

		// Token: 0x060034BE RID: 13502 RVA: 0x00002E9A File Offset: 0x0000109A
		protected virtual void IntensityChanged(byte prevState, byte newState)
		{
		}

		// Token: 0x060034BF RID: 13503 RVA: 0x00002E9A File Offset: 0x0000109A
		public virtual void OnBeginSpectating()
		{
		}

		// Token: 0x060034C0 RID: 13504 RVA: 0x00002E9A File Offset: 0x0000109A
		public virtual void OnStopSpectating()
		{
		}

		// Token: 0x060034C1 RID: 13505 RVA: 0x000AA998 File Offset: 0x000A8B98
		internal virtual void OnRoleChanged(PlayerRoleBase previousRole, PlayerRoleBase newRole)
		{
			this.DisableEffect();
		}

		// Token: 0x060034C2 RID: 13506 RVA: 0x000AA998 File Offset: 0x000A8B98
		internal virtual void OnDeath(PlayerRoleBase previousRole)
		{
			this.DisableEffect();
		}

		// Token: 0x060034C3 RID: 13507 RVA: 0x000AD382 File Offset: 0x000AB582
		protected virtual void DisableEffect()
		{
			if (NetworkServer.active)
			{
				this.Duration = 0f;
				this.Intensity = 0;
			}
		}

		// Token: 0x140000A7 RID: 167
		// (add) Token: 0x060034C4 RID: 13508 RVA: 0x000AD3A0 File Offset: 0x000AB5A0
		// (remove) Token: 0x060034C5 RID: 13509 RVA: 0x000AD3D4 File Offset: 0x000AB5D4
		public static event Action<StatusEffectBase> OnEnabled;

		// Token: 0x140000A8 RID: 168
		// (add) Token: 0x060034C6 RID: 13510 RVA: 0x000AD408 File Offset: 0x000AB608
		// (remove) Token: 0x060034C7 RID: 13511 RVA: 0x000AD43C File Offset: 0x000AB63C
		public static event Action<StatusEffectBase> OnDisabled;

		// Token: 0x140000A9 RID: 169
		// (add) Token: 0x060034C8 RID: 13512 RVA: 0x000AD470 File Offset: 0x000AB670
		// (remove) Token: 0x060034C9 RID: 13513 RVA: 0x000AD4A4 File Offset: 0x000AB6A4
		public static event Action<StatusEffectBase, byte, byte> OnIntensityChanged;

		// Token: 0x060034CA RID: 13514 RVA: 0x000AD4D7 File Offset: 0x000AB6D7
		public bool Equals(StatusEffectBase other)
		{
			return other != null && other.gameObject == base.gameObject;
		}

		// Token: 0x060034CB RID: 13515 RVA: 0x000AD4F5 File Offset: 0x000AB6F5
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((StatusEffectBase)obj)));
		}

		// Token: 0x060034CC RID: 13516 RVA: 0x0000C034 File Offset: 0x0000A234
		public override int GetHashCode()
		{
			return base.gameObject.GetHashCode();
		}

		// Token: 0x04002845 RID: 10309
		private byte _intensity;

		// Token: 0x04002846 RID: 10310
		private float _duration;

		// Token: 0x04002847 RID: 10311
		private float _timeLeft;

		// Token: 0x02000907 RID: 2311
		public enum EffectClassification
		{
			// Token: 0x0400284C RID: 10316
			Technical,
			// Token: 0x0400284D RID: 10317
			Negative,
			// Token: 0x0400284E RID: 10318
			Mixed,
			// Token: 0x0400284F RID: 10319
			Positive
		}
	}
}
