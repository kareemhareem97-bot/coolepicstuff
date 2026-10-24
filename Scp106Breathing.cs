using System;
using PlayerRoles.PlayableScps.Scp106;
using PlayerRoles.Spectating;
using PlayerRoles.Subroutines;
using UnityEngine;

// Token: 0x02000144 RID: 324
public class Scp106Breathing : StandardSubroutine<Scp106Role>
{
	// Token: 0x06000636 RID: 1590 RVA: 0x0001783F File Offset: 0x00015A3F
	protected override void Awake()
	{
		base.Awake();
		base.GetSubroutine<Scp106SinkholeController>(out this._sinkholeController);
	}

	// Token: 0x06000637 RID: 1591 RVA: 0x00017854 File Offset: 0x00015A54
	private void Update()
	{
		this._breathingSource.volume = (1f - this._sinkholeController.SubmergeProgress) * ((base.Owner.isLocalPlayer || base.Owner.IsLocallySpectated()) ? 0.5f : 1f);
		this._breathingSource.spatialBlend = ((base.Owner.isLocalPlayer || base.Owner.IsLocallySpectated()) ? 0f : 1f);
	}

	// Token: 0x040005BC RID: 1468
	private const float SelfMaxVolume = 0.5f;

	// Token: 0x040005BD RID: 1469
	private const float MaxVolume = 1f;

	// Token: 0x040005BE RID: 1470
	private Scp106SinkholeController _sinkholeController;

	// Token: 0x040005BF RID: 1471
	[SerializeField]
	private AudioSource _breathingSource;
}
