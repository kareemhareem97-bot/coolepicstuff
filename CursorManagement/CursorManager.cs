using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CursorManagement
{
	// Token: 0x02000995 RID: 2453
	public static class CursorManager
	{
		// Token: 0x17000AF9 RID: 2809
		// (get) Token: 0x06003796 RID: 14230 RVA: 0x000B43EF File Offset: 0x000B25EF
		// (set) Token: 0x06003797 RID: 14231 RVA: 0x000B43F6 File Offset: 0x000B25F6
		public static bool MovementLocked { get; private set; }

		// Token: 0x06003798 RID: 14232 RVA: 0x000B43FE File Offset: 0x000B25FE
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			Action value;
			if ((value = CursorManager.<>O.<0>__LateUpdate) == null)
			{
				value = (CursorManager.<>O.<0>__LateUpdate = new Action(CursorManager.LateUpdate));
			}
			StaticUnityMethods.OnLateUpdate += value;
		}

		// Token: 0x06003799 RID: 14233 RVA: 0x000B4420 File Offset: 0x000B2620
		private static void LateUpdate()
		{
			CursorOverrideMode cursorOverrideMode;
			bool movementLocked;
			CursorManager.ProcessOverrides(out cursorOverrideMode, out movementLocked);
			if (cursorOverrideMode != CursorOverrideMode.Centered)
			{
				if (cursorOverrideMode == CursorOverrideMode.Confined)
				{
					CursorManager.SetLockMode(CursorLockMode.Confined);
				}
				else
				{
					CursorManager.SetLockMode(CursorLockMode.None);
				}
			}
			else
			{
				CursorManager.SetLockMode(CursorLockMode.Locked);
			}
			CursorManager.MovementLocked = movementLocked;
		}

		// Token: 0x0600379A RID: 14234 RVA: 0x000B445C File Offset: 0x000B265C
		private static void ProcessOverrides(out CursorOverrideMode bestOverride, out bool anyMovementLocked)
		{
			bestOverride = CursorOverrideMode.NoOverride;
			anyMovementLocked = false;
			if (!StaticUnityMethods.IsPlaying)
			{
				return;
			}
			int num = 0;
			CursorManager.Overrides.RemoveWhere(delegate(ICursorOverride x)
			{
				if (x != null)
				{
					UnityEngine.Object @object = x as UnityEngine.Object;
					return @object != null && @object == null;
				}
				return true;
			});
			foreach (ICursorOverride cursorOverride in CursorManager.Overrides)
			{
				num = Mathf.Max(num, (int)cursorOverride.CursorOverride);
				anyMovementLocked |= cursorOverride.LockMovement;
			}
			bestOverride = (CursorOverrideMode)num;
		}

		// Token: 0x0600379B RID: 14235 RVA: 0x000B44FC File Offset: 0x000B26FC
		public static void SetLockMode(CursorLockMode lockMode)
		{
			Cursor.lockState = lockMode;
			Cursor.visible = (lockMode != CursorLockMode.Locked);
		}

		// Token: 0x0600379C RID: 14236 RVA: 0x000B4510 File Offset: 0x000B2710
		public static bool Register(ICursorOverride target)
		{
			return CursorManager.Overrides.Add(target);
		}

		// Token: 0x0600379D RID: 14237 RVA: 0x000B451D File Offset: 0x000B271D
		public static bool Unregister(ICursorOverride target)
		{
			return CursorManager.Overrides.Remove(target);
		}

		// Token: 0x040029AE RID: 10670
		private static readonly HashSet<ICursorOverride> Overrides = new HashSet<ICursorOverride>();

		// Token: 0x02000996 RID: 2454
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x040029B0 RID: 10672
			public static Action <0>__LateUpdate;
		}
	}
}
