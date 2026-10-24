using System;
using CustomPlayerEffects;
using Mirror;
using UnityEngine;

// Token: 0x0200013B RID: 315
public class PitKiller : MonoBehaviour
{
	// Token: 0x14000015 RID: 21
	// (add) Token: 0x06000615 RID: 1557 RVA: 0x00017430 File Offset: 0x00015630
	// (remove) Token: 0x06000616 RID: 1558 RVA: 0x00017468 File Offset: 0x00015668
	public event Action<ReferenceHub> OnEffectApplied;

	// Token: 0x06000617 RID: 1559 RVA: 0x000174A0 File Offset: 0x000156A0
	private void OnTriggerEnter(Collider other)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		ReferenceHub referenceHub;
		if (!ReferenceHub.TryGetHub(other.transform.root.gameObject, out referenceHub))
		{
			return;
		}
		if (!PitDeath.ValidatePlayer(referenceHub))
		{
			return;
		}
		PitDeath effect = referenceHub.playerEffectsController.GetEffect<PitDeath>();
		if (effect.IsEnabled)
		{
			return;
		}
		Action<ReferenceHub> onEffectApplied = this.OnEffectApplied;
		if (onEffectApplied != null)
		{
			onEffectApplied(referenceHub);
		}
		effect.IsEnabled = true;
	}
}
