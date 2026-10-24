using System;

namespace Achievements
{
	// Token: 0x02000AE6 RID: 2790
	public readonly struct Achievement
	{
		// Token: 0x06003F80 RID: 16256 RVA: 0x000D2941 File Offset: 0x000D0B41
		public Achievement(string steamName, long discordId, bool byServer = false)
		{
			this.SteamName = steamName;
			this._discordId = discordId;
			this._steamProgress = string.Empty;
			this._maxValue = 0;
			this.ActivatedByServer = byServer;
		}

		// Token: 0x06003F81 RID: 16257 RVA: 0x000D296A File Offset: 0x000D0B6A
		public Achievement(string steamName, string steamParameter, long discordId, int maxValue, bool byServer = false)
		{
			this.SteamName = steamName;
			this._discordId = discordId;
			this._steamProgress = steamParameter;
			this._maxValue = maxValue;
			this.ActivatedByServer = byServer;
		}

		// Token: 0x06003F82 RID: 16258 RVA: 0x00002E9A File Offset: 0x0000109A
		public void Achieve()
		{
		}

		// Token: 0x06003F83 RID: 16259 RVA: 0x00002E9A File Offset: 0x0000109A
		public void AddProgress(int amt = 1)
		{
		}

		// Token: 0x06003F84 RID: 16260 RVA: 0x00002E9A File Offset: 0x0000109A
		public void Reset()
		{
		}

		// Token: 0x04002E32 RID: 11826
		public readonly string SteamName;

		// Token: 0x04002E33 RID: 11827
		private readonly string _steamProgress;

		// Token: 0x04002E34 RID: 11828
		private readonly long _discordId;

		// Token: 0x04002E35 RID: 11829
		private readonly int _maxValue;

		// Token: 0x04002E36 RID: 11830
		public readonly bool ActivatedByServer;
	}
}
