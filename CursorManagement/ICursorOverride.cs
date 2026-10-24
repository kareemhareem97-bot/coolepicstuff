using System;

namespace CursorManagement
{
	// Token: 0x02000998 RID: 2456
	public interface ICursorOverride
	{
		// Token: 0x17000AFA RID: 2810
		// (get) Token: 0x060037A2 RID: 14242
		CursorOverrideMode CursorOverride { get; }

		// Token: 0x17000AFB RID: 2811
		// (get) Token: 0x060037A3 RID: 14243
		bool LockMovement { get; }
	}
}
