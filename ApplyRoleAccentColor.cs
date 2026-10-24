using System;
using PlayerRoles;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000A3 RID: 163
public class ApplyRoleAccentColor : MonoBehaviour
{
	// Token: 0x06000366 RID: 870 RVA: 0x0000CD3A File Offset: 0x0000AF3A
	private void Awake()
	{
		this._graphic = base.GetComponent<Graphic>();
		this._graphic.color = this._color.Color;
		PlayerRoleManager.OnRoleChanged += this.OnRoleChanged;
	}

	// Token: 0x06000367 RID: 871 RVA: 0x0000CD6F File Offset: 0x0000AF6F
	private void OnDestroy()
	{
		PlayerRoleManager.OnRoleChanged -= this.OnRoleChanged;
	}

	// Token: 0x06000368 RID: 872 RVA: 0x0000CD82 File Offset: 0x0000AF82
	private void OnRoleChanged(ReferenceHub userHub, PlayerRoleBase prevRole, PlayerRoleBase newRole)
	{
		if (!userHub.isLocalPlayer)
		{
			return;
		}
		this._graphic.color = this._color.Color;
	}

	// Token: 0x040002FF RID: 767
	[SerializeField]
	private RoleAccentColor _color;

	// Token: 0x04000300 RID: 768
	private Graphic _graphic;
}
