using System;
using System.Collections.Generic;
using AudioPooling;

namespace FacilitySoundtrack.ChaseThemes
{
	// Token: 0x02000882 RID: 2178
	public interface IChaseThemeProvider
	{
		// Token: 0x1700093E RID: 2366
		// (get) Token: 0x060031BE RID: 12734
		List<DesiredChaseClip> ClipsToPlay { get; }

		// Token: 0x1700093F RID: 2367
		// (get) Token: 0x060031BF RID: 12735
		float DominanceScale { get; }

		// Token: 0x17000940 RID: 2368
		// (get) Token: 0x060031C0 RID: 12736
		int Priority { get; }

		// Token: 0x060031C1 RID: 12737
		bool TryStartChase();

		// Token: 0x060031C2 RID: 12738
		AudioPoolSession? OnChaseStarted(List<AudioPoolSession> alreadyPlayedStingers);

		// Token: 0x060031C3 RID: 12739
		void OnChaseStopped();

		// Token: 0x060031C4 RID: 12740
		bool UpdateChase();
	}
}
