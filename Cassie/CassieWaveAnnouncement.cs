using System;
using Respawning.Waves;

namespace Cassie
{
	// Token: 0x02000AC3 RID: 2755
	public class CassieWaveAnnouncement : CassieAnnouncement
	{
		// Token: 0x140000B4 RID: 180
		// (add) Token: 0x06003E23 RID: 15907 RVA: 0x000CD544 File Offset: 0x000CB744
		// (remove) Token: 0x06003E24 RID: 15908 RVA: 0x000CD578 File Offset: 0x000CB778
		public static event Action<CassieWaveAnnouncement> OnAnnouncementFinishedPlaying;

		// Token: 0x06003E25 RID: 15909 RVA: 0x000CD5AB File Offset: 0x000CB7AB
		public CassieWaveAnnouncement(IAnnouncedWave wave, CassieTtsPayload payload) : base(payload, 0f, 1f)
		{
			this.Wave = wave;
		}

		// Token: 0x06003E26 RID: 15910 RVA: 0x000CD5C5 File Offset: 0x000CB7C5
		public override void OnFinishedPlaying()
		{
			base.OnFinishedPlaying();
			Action<CassieWaveAnnouncement> onAnnouncementFinishedPlaying = CassieWaveAnnouncement.OnAnnouncementFinishedPlaying;
			if (onAnnouncementFinishedPlaying == null)
			{
				return;
			}
			onAnnouncementFinishedPlaying(this);
		}

		// Token: 0x04002D81 RID: 11649
		public readonly IAnnouncedWave Wave;
	}
}
