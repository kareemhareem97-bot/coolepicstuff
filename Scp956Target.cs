using System;
using CursorManagement;
using CustomPlayerEffects;
using InventorySystem.Items;
using MapGeneration.Holidays;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

// Token: 0x0200002F RID: 47
public class Scp956Target : StatusEffectBase, IMovementSpeedModifier, ICursorOverride, IInteractionBlocker, IHolidayEffect
{
	// Token: 0x1700003B RID: 59
	// (get) Token: 0x060000FB RID: 251 RVA: 0x00005420 File Offset: 0x00003620
	public bool IsAffected
	{
		get
		{
			float num;
			if (base.Intensity == 255)
			{
				num = 10f;
			}
			else
			{
				num = 2.2f;
			}
			float num2 = num;
			return base.IsEnabled && (double)this._activeTime > (double)num2 - NetworkTime.rtt;
		}
	}

	// Token: 0x1700003C RID: 60
	// (get) Token: 0x060000FC RID: 252 RVA: 0x00005465 File Offset: 0x00003665
	public Vector3 Position
	{
		get
		{
			return (base.Hub.roleManager.CurrentRole as HumanRole).FpcModule.Position;
		}
	}

	// Token: 0x1700003D RID: 61
	// (get) Token: 0x060000FD RID: 253 RVA: 0x00005486 File Offset: 0x00003686
	public bool MovementModifierActive
	{
		get
		{
			return this.IsAffected;
		}
	}

	// Token: 0x1700003E RID: 62
	// (get) Token: 0x060000FE RID: 254 RVA: 0x0000548E File Offset: 0x0000368E
	public float MovementSpeedMultiplier
	{
		get
		{
			return 0f;
		}
	}

	// Token: 0x1700003F RID: 63
	// (get) Token: 0x060000FF RID: 255 RVA: 0x0000548E File Offset: 0x0000368E
	public float MovementSpeedLimit
	{
		get
		{
			return 0f;
		}
	}

	// Token: 0x17000040 RID: 64
	// (get) Token: 0x06000100 RID: 256 RVA: 0x00004094 File Offset: 0x00002294
	public CursorOverrideMode CursorOverride
	{
		get
		{
			return CursorOverrideMode.NoOverride;
		}
	}

	// Token: 0x17000041 RID: 65
	// (get) Token: 0x06000101 RID: 257 RVA: 0x00005486 File Offset: 0x00003686
	public bool LockMovement
	{
		get
		{
			return this.IsAffected;
		}
	}

	// Token: 0x17000042 RID: 66
	// (get) Token: 0x06000102 RID: 258 RVA: 0x00005495 File Offset: 0x00003695
	public BlockedInteraction BlockedInteractions
	{
		get
		{
			return BlockedInteraction.All;
		}
	}

	// Token: 0x17000043 RID: 67
	// (get) Token: 0x06000103 RID: 259 RVA: 0x0000549C File Offset: 0x0000369C
	public bool CanBeCleared
	{
		get
		{
			return !base.IsEnabled;
		}
	}

	// Token: 0x17000044 RID: 68
	// (get) Token: 0x06000104 RID: 260 RVA: 0x000054A7 File Offset: 0x000036A7
	public HolidayType[] TargetHolidays { get; } = new HolidayType[]
	{
		HolidayType.Christmas,
		HolidayType.AprilFools
	};

	// Token: 0x17000045 RID: 69
	// (get) Token: 0x06000105 RID: 261 RVA: 0x00004094 File Offset: 0x00002294
	public override StatusEffectBase.EffectClassification Classification
	{
		get
		{
			return StatusEffectBase.EffectClassification.Technical;
		}
	}

	// Token: 0x06000106 RID: 262 RVA: 0x000054AF File Offset: 0x000036AF
	protected override void Start()
	{
		base.Start();
	}

	// Token: 0x06000107 RID: 263 RVA: 0x000054B8 File Offset: 0x000036B8
	protected override void Enabled()
	{
		base.Enabled();
		this._musicSource.Stop();
		this._musicSource.volume = 1f;
		this._musicSource.loop = false;
		Scp956Target.EffectReason intensity = (Scp956Target.EffectReason)base.Intensity;
		if (intensity != Scp956Target.EffectReason.Child)
		{
			if (intensity == Scp956Target.EffectReason.HasCandy)
			{
				this._musicSource.PlayOneShot(this._longClip);
			}
		}
		else
		{
			this._musicSource.PlayOneShot(this._shortClip);
		}
		Scp956Pinata.IsSpawned = true;
		IFpcRole fpcRole = base.Hub.roleManager.CurrentRole as IFpcRole;
		if (fpcRole != null)
		{
			fpcRole.FpcModule.Motor.RegisterMovementLock(this);
		}
	}

	// Token: 0x06000108 RID: 264 RVA: 0x00005560 File Offset: 0x00003760
	protected override void Disabled()
	{
		base.Disabled();
		IFpcRole fpcRole = base.Hub.roleManager.CurrentRole as IFpcRole;
		if (fpcRole != null)
		{
			fpcRole.FpcModule.Motor.UnregisterMovementLock(this);
		}
	}

	// Token: 0x06000109 RID: 265 RVA: 0x000055A0 File Offset: 0x000037A0
	protected override void Update()
	{
		base.Update();
		this._musicSource.mute = (!base.IsLocalPlayer && !base.IsSpectated);
		if (base.IsEnabled)
		{
			this._activeTime += Time.deltaTime;
			if (!this.IsAffected)
			{
				return;
			}
			if (NetworkServer.active || base.IsLocalPlayer)
			{
				base.Hub.interCoordinator.AddBlocker(this);
			}
			if (NetworkServer.active)
			{
				base.Hub.inventory.ServerSelectItem(0);
			}
			if (base.IsLocalPlayer)
			{
				this.LookAtPinata();
			}
			if (this._musicSource.loop)
			{
				return;
			}
			this._musicSource.loop = true;
			this._musicSource.volume = 1f;
			this._musicSource.Stop();
			this._musicSource.Play();
			return;
		}
		else
		{
			this._activeTime = 0f;
			if (!this._musicSource.isPlaying)
			{
				return;
			}
			this._musicSource.volume -= Time.deltaTime * 3f;
			if (this._musicSource.volume <= 0f)
			{
				this._musicSource.Stop();
			}
			return;
		}
	}

	// Token: 0x0600010A RID: 266 RVA: 0x000056D4 File Offset: 0x000038D4
	private void LookAtPinata()
	{
		Vector3 position = MainCameraController.CurrentCamera.position;
		Vector3 eulerAngles = Quaternion.LookRotation(Scp956Pinata.LastPosition - position, Vector3.up).eulerAngles;
		IFpcRole fpcRole = base.Hub.roleManager.CurrentRole as IFpcRole;
		if (fpcRole == null)
		{
			return;
		}
		FpcMouseLook mouseLook = fpcRole.FpcModule.MouseLook;
		mouseLook.CurrentHorizontal = Mathf.LerpAngle(mouseLook.CurrentHorizontal, eulerAngles.y, Time.deltaTime * 4f);
		mouseLook.CurrentVertical = Mathf.LerpAngle(mouseLook.CurrentVertical, -eulerAngles.x, Time.deltaTime * 4f);
	}

	// Token: 0x040000F2 RID: 242
	private float _activeTime;

	// Token: 0x040000F3 RID: 243
	[SerializeField]
	private AudioSource _musicSource;

	// Token: 0x040000F4 RID: 244
	[SerializeField]
	private AudioClip _shortClip;

	// Token: 0x040000F5 RID: 245
	[SerializeField]
	private AudioClip _longClip;

	// Token: 0x02000030 RID: 48
	public enum EffectReason
	{
		// Token: 0x040000F7 RID: 247
		None,
		// Token: 0x040000F8 RID: 248
		Child = 254,
		// Token: 0x040000F9 RID: 249
		HasCandy
	}
}
