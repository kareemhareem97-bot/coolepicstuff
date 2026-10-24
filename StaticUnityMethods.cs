using System;
using Mirror;
using UnityEngine;

// Token: 0x0200009D RID: 157
public class StaticUnityMethods : MonoBehaviour
{
	// Token: 0x14000008 RID: 8
	// (add) Token: 0x06000352 RID: 850 RVA: 0x0000CA6C File Offset: 0x0000AC6C
	// (remove) Token: 0x06000353 RID: 851 RVA: 0x0000CAA0 File Offset: 0x0000ACA0
	public static event Action OnUpdate;

	// Token: 0x14000009 RID: 9
	// (add) Token: 0x06000354 RID: 852 RVA: 0x0000CAD4 File Offset: 0x0000ACD4
	// (remove) Token: 0x06000355 RID: 853 RVA: 0x0000CB08 File Offset: 0x0000AD08
	public static event Action OnLateUpdate;

	// Token: 0x1400000A RID: 10
	// (add) Token: 0x06000356 RID: 854 RVA: 0x0000CB3C File Offset: 0x0000AD3C
	// (remove) Token: 0x06000357 RID: 855 RVA: 0x0000CB70 File Offset: 0x0000AD70
	public static event Action OnFixedUpdate;

	// Token: 0x1400000B RID: 11
	// (add) Token: 0x06000358 RID: 856 RVA: 0x0000CBA4 File Offset: 0x0000ADA4
	// (remove) Token: 0x06000359 RID: 857 RVA: 0x0000CBD8 File Offset: 0x0000ADD8
	public static event Action OnGui;

	// Token: 0x170000A6 RID: 166
	// (get) Token: 0x0600035A RID: 858 RVA: 0x0000CC0C File Offset: 0x0000AE0C
	public static bool IsPlaying
	{
		get
		{
			ReferenceHub referenceHub;
			return ReferenceHub.TryGetLocalHub(out referenceHub) && (NetworkClient.active || NetworkServer.active);
		}
	}

	// Token: 0x0600035B RID: 859 RVA: 0x0000CC32 File Offset: 0x0000AE32
	private void Update()
	{
		Action onUpdate = StaticUnityMethods.OnUpdate;
		if (onUpdate == null)
		{
			return;
		}
		onUpdate();
	}

	// Token: 0x0600035C RID: 860 RVA: 0x0000CC43 File Offset: 0x0000AE43
	private void FixedUpdate()
	{
		Action onFixedUpdate = StaticUnityMethods.OnFixedUpdate;
		if (onFixedUpdate == null)
		{
			return;
		}
		onFixedUpdate();
	}

	// Token: 0x0600035D RID: 861 RVA: 0x0000CC54 File Offset: 0x0000AE54
	private void LateUpdate()
	{
		Action onLateUpdate = StaticUnityMethods.OnLateUpdate;
		if (onLateUpdate == null)
		{
			return;
		}
		onLateUpdate();
	}

	// Token: 0x0600035E RID: 862 RVA: 0x0000CC65 File Offset: 0x0000AE65
	private void OnGUI()
	{
		Action onGui = StaticUnityMethods.OnGui;
		if (onGui == null)
		{
			return;
		}
		onGui();
	}
}
