using System;
using NorthwoodLib;
using UnityEngine;

// Token: 0x02000228 RID: 552
public class MainThreadDispatcher : MonoBehaviour
{
	// Token: 0x06000C46 RID: 3142 RVA: 0x000303C3 File Offset: 0x0002E5C3
	public static void Dispatch(Action action, MainThreadDispatcher.DispatchTime dispatchTime = MainThreadDispatcher.DispatchTime.Update)
	{
		switch (dispatchTime)
		{
		case MainThreadDispatcher.DispatchTime.Update:
			MainThreadDispatcher.UpdateDispatcher.Dispatch(action);
			return;
		case MainThreadDispatcher.DispatchTime.LateUpdate:
			MainThreadDispatcher.LateUpdateDispatcher.Dispatch(action);
			return;
		case MainThreadDispatcher.DispatchTime.FixedUpdate:
			MainThreadDispatcher.FixedUpdateDispatcher.Dispatch(action);
			return;
		default:
			return;
		}
	}

	// Token: 0x06000C47 RID: 3143 RVA: 0x000303FB File Offset: 0x0002E5FB
	private void Update()
	{
		MainThreadDispatcher.UpdateDispatcher.Invoke();
	}

	// Token: 0x06000C48 RID: 3144 RVA: 0x00030407 File Offset: 0x0002E607
	private void LateUpdate()
	{
		MainThreadDispatcher.LateUpdateDispatcher.Invoke();
	}

	// Token: 0x06000C49 RID: 3145 RVA: 0x00030413 File Offset: 0x0002E613
	private void FixedUpdate()
	{
		MainThreadDispatcher.FixedUpdateDispatcher.Invoke();
	}

	// Token: 0x04000A20 RID: 2592
	private static readonly ActionDispatcher UpdateDispatcher = new ActionDispatcher();

	// Token: 0x04000A21 RID: 2593
	private static readonly ActionDispatcher LateUpdateDispatcher = new ActionDispatcher();

	// Token: 0x04000A22 RID: 2594
	private static readonly ActionDispatcher FixedUpdateDispatcher = new ActionDispatcher();

	// Token: 0x02000229 RID: 553
	public enum DispatchTime
	{
		// Token: 0x04000A24 RID: 2596
		Update,
		// Token: 0x04000A25 RID: 2597
		LateUpdate,
		// Token: 0x04000A26 RID: 2598
		FixedUpdate
	}
}
