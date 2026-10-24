using System;
using CustomPlayerEffects;
using InventorySystem.Items.Firearms.Attachments;
using MapGeneration.Holidays;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

// Token: 0x02000027 RID: 39
public class Scp559Effect : StatusEffectBase, IMovementSpeedModifier, IStaminaModifier, IWeaponModifierPlayerEffect, IHealableEffect, IHolidayEffect
{
	// Token: 0x17000022 RID: 34
	// (get) Token: 0x060000AC RID: 172 RVA: 0x0000405F File Offset: 0x0000225F
	// (set) Token: 0x060000AD RID: 173 RVA: 0x00004067 File Offset: 0x00002267
	public float ModelScale { get; private set; }

	// Token: 0x17000023 RID: 35
	// (get) Token: 0x060000AE RID: 174 RVA: 0x00004070 File Offset: 0x00002270
	public bool MovementModifierActive
	{
		get
		{
			return base.IsEnabled;
		}
	}

	// Token: 0x17000024 RID: 36
	// (get) Token: 0x060000AF RID: 175 RVA: 0x00004078 File Offset: 0x00002278
	public float MovementSpeedMultiplier
	{
		get
		{
			return 1.05f;
		}
	}

	// Token: 0x17000025 RID: 37
	// (get) Token: 0x060000B0 RID: 176 RVA: 0x0000407F File Offset: 0x0000227F
	public float MovementSpeedLimit
	{
		get
		{
			return float.MaxValue;
		}
	}

	// Token: 0x17000026 RID: 38
	// (get) Token: 0x060000B1 RID: 177 RVA: 0x00004070 File Offset: 0x00002270
	public bool StaminaModifierActive
	{
		get
		{
			return base.IsEnabled;
		}
	}

	// Token: 0x17000027 RID: 39
	// (get) Token: 0x060000B2 RID: 178 RVA: 0x00004086 File Offset: 0x00002286
	public float StaminaUsageMultiplier
	{
		get
		{
			return 0.5f;
		}
	}

	// Token: 0x17000028 RID: 40
	// (get) Token: 0x060000B3 RID: 179 RVA: 0x0000408D File Offset: 0x0000228D
	public float StaminaRegenMultiplier
	{
		get
		{
			return 1f;
		}
	}

	// Token: 0x17000029 RID: 41
	// (get) Token: 0x060000B4 RID: 180 RVA: 0x00004094 File Offset: 0x00002294
	public bool SprintingDisabled
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700002A RID: 42
	// (get) Token: 0x060000B5 RID: 181 RVA: 0x00004070 File Offset: 0x00002270
	public bool ParamsActive
	{
		get
		{
			return base.IsEnabled;
		}
	}

	// Token: 0x1700002B RID: 43
	// (get) Token: 0x060000B6 RID: 182 RVA: 0x00004097 File Offset: 0x00002297
	public HolidayType[] TargetHolidays { get; } = new HolidayType[]
	{
		HolidayType.Christmas,
		HolidayType.AprilFools
	};

	// Token: 0x1700002C RID: 44
	// (get) Token: 0x060000B7 RID: 183 RVA: 0x00004094 File Offset: 0x00002294
	public override StatusEffectBase.EffectClassification Classification
	{
		get
		{
			return StatusEffectBase.EffectClassification.Technical;
		}
	}

	// Token: 0x060000B8 RID: 184 RVA: 0x000040A0 File Offset: 0x000022A0
	private void UpdateSize()
	{
		this._offset.localPosition = Vector3.up * (0.628f - this._heightReduction * this._state);
		this.ModelScale = this._scaleReduction * this._state;
		if (base.IsLocalPlayer)
		{
			Scp559Effect.LocallyActive = (this._state > 0f);
			Scp559Effect.LocalPitchMultiplier = Mathf.Lerp(1f, this._voicePitch, this._state);
		}
		IFpcRole fpcRole = base.Hub.roleManager.CurrentRole as IFpcRole;
		if (fpcRole == null)
		{
			return;
		}
		Transform transform = fpcRole.FpcModule.CharacterModelInstance.transform;
		Vector3 localScale = fpcRole.FpcModule.CharacterModelTemplate.transform.localScale;
		transform.localScale = Vector3.Lerp(localScale, localScale * this._scaleReduction, this._state);
	}

	// Token: 0x060000B9 RID: 185 RVA: 0x0000417C File Offset: 0x0000237C
	protected override void Awake()
	{
		base.Awake();
		this._offset = base.Hub.PlayerCameraReference.parent;
		this.ModelScale = 1f;
	}

	// Token: 0x060000BA RID: 186 RVA: 0x000041A5 File Offset: 0x000023A5
	internal override void OnRoleChanged(PlayerRoleBase previousRole, PlayerRoleBase newRole)
	{
		base.OnRoleChanged(previousRole, newRole);
		this._state = 0f;
		this.UpdateSize();
	}

	// Token: 0x060000BB RID: 187 RVA: 0x000041C0 File Offset: 0x000023C0
	protected override void Update()
	{
		base.Update();
		float num = Mathf.MoveTowards(this._state, (float)(base.IsEnabled ? 1 : 0), this._adjustSpeed * Time.deltaTime);
		if (num == this._state)
		{
			return;
		}
		this._state = num;
		this.UpdateSize();
	}

	// Token: 0x060000BC RID: 188 RVA: 0x0000420F File Offset: 0x0000240F
	public bool TryGetWeaponParam(AttachmentParam param, out float val)
	{
		if (param == AttachmentParam.OverallRecoilMultiplier)
		{
			val = 1.1f;
			return true;
		}
		if (param == AttachmentParam.DrawSpeedMultiplier)
		{
			val = 0.7f;
			return true;
		}
		if (param == AttachmentParam.ReloadSpeedMultiplier)
		{
			val = 0.8f;
			return true;
		}
		val = 0f;
		return false;
	}

	// Token: 0x060000BD RID: 189 RVA: 0x00004242 File Offset: 0x00002442
	public bool IsHealable(ItemType item)
	{
		return item == ItemType.SCP500;
	}

	// Token: 0x040000AC RID: 172
	private const float Height = 0.628f;

	// Token: 0x040000AD RID: 173
	public static bool LocallyActive;

	// Token: 0x040000AE RID: 174
	public static float LocalPitchMultiplier;

	// Token: 0x040000B1 RID: 177
	[SerializeField]
	private float _heightReduction;

	// Token: 0x040000B2 RID: 178
	[SerializeField]
	private float _scaleReduction;

	// Token: 0x040000B3 RID: 179
	[SerializeField]
	private float _adjustSpeed;

	// Token: 0x040000B4 RID: 180
	[SerializeField]
	private float _voicePitch;

	// Token: 0x040000B5 RID: 181
	private float _state;

	// Token: 0x040000B6 RID: 182
	private Transform _offset;
}
