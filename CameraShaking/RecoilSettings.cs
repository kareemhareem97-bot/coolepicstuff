using System;

namespace CameraShaking
{
	// Token: 0x02000890 RID: 2192
	[Serializable]
	public struct RecoilSettings
	{
		// Token: 0x060031F4 RID: 12788 RVA: 0x000A7EA5 File Offset: 0x000A60A5
		public RecoilSettings(float animationTime, float zAxis, float fovKick, float upKick, float sideKick)
		{
			this.AnimationTime = animationTime;
			this.ZAxis = zAxis;
			this.FovKick = fovKick;
			this.UpKick = upKick;
			this.SideKick = sideKick;
		}

		// Token: 0x0400270C RID: 9996
		public float AnimationTime;

		// Token: 0x0400270D RID: 9997
		public float ZAxis;

		// Token: 0x0400270E RID: 9998
		public float FovKick;

		// Token: 0x0400270F RID: 9999
		public float UpKick;

		// Token: 0x04002710 RID: 10000
		public float SideKick;
	}
}
