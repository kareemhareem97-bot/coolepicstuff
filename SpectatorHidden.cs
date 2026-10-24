using System;
using GameObjectPools;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000054 RID: 84
public class SpectatorHidden : MonoBehaviour, IPoolResettable
{
	// Token: 0x17000079 RID: 121
	// (get) Token: 0x060001FA RID: 506 RVA: 0x00008550 File Offset: 0x00006750
	// (set) Token: 0x060001FB RID: 507 RVA: 0x00008558 File Offset: 0x00006758
	public Renderer AttachedRenderer { get; private set; }

	// Token: 0x060001FC RID: 508 RVA: 0x00008561 File Offset: 0x00006761
	private void Awake()
	{
		this.AttachedRenderer = base.GetComponent<Renderer>();
	}

	// Token: 0x060001FD RID: 509 RVA: 0x0000856F File Offset: 0x0000676F
	public void ResetObject()
	{
		this.AttachedRenderer.shadowCastingMode = ShadowCastingMode.On;
	}
}
