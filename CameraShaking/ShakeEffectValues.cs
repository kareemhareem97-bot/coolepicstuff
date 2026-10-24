using System;
using UnityEngine;

namespace CameraShaking
{
	// Token: 0x0200088D RID: 2189
	public readonly struct ShakeEffectValues
	{
		// Token: 0x060031EE RID: 12782 RVA: 0x000A7C1C File Offset: 0x000A5E1C
		public ShakeEffectValues(Quaternion? rootCameraRotation = null, Quaternion? viewmodelCameraRotation = null, Vector3? rootCameraPositionOffset = null, float fovPercent = 1f, float verticalLook = 0f, float horizontalLook = 0f)
		{
			this.RootCameraRotation = (rootCameraRotation ?? Quaternion.identity);
			this.ViewmodelCameraRotation = (viewmodelCameraRotation ?? Quaternion.identity);
			this.RootCameraPositionOffset = (rootCameraPositionOffset ?? Vector3.zero);
			this.FovPercent = fovPercent;
			this.VerticalMouseLook = verticalLook;
			this.HorizontalMouseLook = horizontalLook;
		}

		// Token: 0x04002700 RID: 9984
		public static readonly ShakeEffectValues None = new ShakeEffectValues(null, null, null, 1f, 0f, 0f);

		// Token: 0x04002701 RID: 9985
		public readonly Quaternion RootCameraRotation;

		// Token: 0x04002702 RID: 9986
		public readonly Quaternion ViewmodelCameraRotation;

		// Token: 0x04002703 RID: 9987
		public readonly Vector3 RootCameraPositionOffset;

		// Token: 0x04002704 RID: 9988
		public readonly float FovPercent;

		// Token: 0x04002705 RID: 9989
		public readonly float VerticalMouseLook;

		// Token: 0x04002706 RID: 9990
		public readonly float HorizontalMouseLook;
	}
}
