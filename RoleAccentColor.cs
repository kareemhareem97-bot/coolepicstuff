using System;
using PlayerRoles;
using UnityEngine;

// Token: 0x020000EC RID: 236
[Serializable]
public class RoleAccentColor
{
	// Token: 0x170000BE RID: 190
	// (get) Token: 0x0600047B RID: 1147 RVA: 0x00011128 File Offset: 0x0000F328
	// (set) Token: 0x0600047A RID: 1146 RVA: 0x00011116 File Offset: 0x0000F316
	public Color Color
	{
		get
		{
			if (this.RoleColorInfluence == 0f)
			{
				return this.ClampBrigtness(this._savedColor);
			}
			ReferenceHub referenceHub;
			bool flag;
			Color color;
			if (ReferenceHub.TryGetLocalHub(out referenceHub))
			{
				PlayerRoleBase currentRole = referenceHub.roleManager.CurrentRole;
				flag = (this._prevType == currentRole.RoleTypeId);
				color = currentRole.RoleColor;
			}
			else
			{
				flag = false;
				color = this._fallbackColor;
			}
			if (this.RoleColorInfluence == 2f)
			{
				return color;
			}
			if (!flag)
			{
				Color color2 = new Color(this._savedColor.r * color.r, this._savedColor.g * color.g, this._savedColor.b * color.b, this._savedColor.a * color.a);
				if (this.RoleColorInfluence == 1f)
				{
					this._prevMixed = color2;
				}
				else if (this.RoleColorInfluence < 1f)
				{
					this._prevMixed = Color.Lerp(this._savedColor, color2, this.RoleColorInfluence);
				}
				else
				{
					this._prevMixed = Color.Lerp(color2, color, this.RoleColorInfluence - 1f);
				}
				this._prevMixed = this.ClampBrigtness(this._prevMixed);
			}
			return this._prevMixed;
		}
		set
		{
			this._savedColor = value;
			this._prevType = (RoleTypeId)(-128);
		}
	}

	// Token: 0x0600047C RID: 1148 RVA: 0x0001125C File Offset: 0x0000F45C
	private Color ClampBrigtness(Color color)
	{
		float grayscale = color.grayscale;
		if (grayscale < this._minBrightness)
		{
			float num = this._minBrightness - grayscale;
			return new Color(Mathf.Clamp01(color.r + num), Mathf.Clamp01(color.g + num), Mathf.Clamp01(color.b + num), color.a);
		}
		if (grayscale > this._maxBrightness)
		{
			float num2 = this._maxBrightness / grayscale;
			return new Color(Mathf.Clamp01(color.r * num2), Mathf.Clamp01(color.g * num2), Mathf.Clamp01(color.b * num2), color.a);
		}
		return color;
	}

	// Token: 0x040003F0 RID: 1008
	[SerializeField]
	private Color _savedColor = Color.white;

	// Token: 0x040003F1 RID: 1009
	[SerializeField]
	private Color _fallbackColor = Color.white;

	// Token: 0x040003F2 RID: 1010
	[SerializeField]
	[Range(0f, 1f)]
	private float _minBrightness;

	// Token: 0x040003F3 RID: 1011
	[SerializeField]
	[Range(0f, 1f)]
	private float _maxBrightness = 1f;

	// Token: 0x040003F4 RID: 1012
	[Range(0f, 2f)]
	public float RoleColorInfluence = 1f;

	// Token: 0x040003F5 RID: 1013
	private Color _prevMixed = Color.white;

	// Token: 0x040003F6 RID: 1014
	private RoleTypeId _prevType = (RoleTypeId)(-128);

	// Token: 0x040003F7 RID: 1015
	private const RoleTypeId NoPrevRole = (RoleTypeId)(-128);
}
