using System;

namespace Cassie
{
	// Token: 0x02000AB9 RID: 2745
	public struct CassiePlaybackModifiers
	{
		// Token: 0x06003DF6 RID: 15862 RVA: 0x000CC878 File Offset: 0x000CAA78
		public readonly double GetTimeUntilNextWord(CassieLine clip)
		{
			if (this.SpacingOverride != null)
			{
				return (double)this.SpacingOverride.Value;
			}
			double num = (double)clip.Duration;
			if (this.Pitch != null)
			{
				num /= (double)this.Pitch.Value;
			}
			if (this.StartTime != null)
			{
				num -= (double)this.StartTime.Value;
			}
			if (this.MaxDuration != null)
			{
				num = Math.Min(num, (double)this.MaxDuration.Value);
			}
			if (this.Stutter != null)
			{
				num += this.Stutter.Value.DurationCost;
			}
			if (this.Sleep != null)
			{
				num += (double)this.Sleep.Value;
			}
			return num;
		}

		// Token: 0x06003DF7 RID: 15863 RVA: 0x000CC93F File Offset: 0x000CAB3F
		public void ResetSingleUseModifiers()
		{
			this.StartTime = null;
			this.MaxDuration = null;
			this.SpacingOverride = null;
			this.Stutter = null;
		}

		// Token: 0x04002D50 RID: 11600
		public float? Pitch;

		// Token: 0x04002D51 RID: 11601
		public float? Volume;

		// Token: 0x04002D52 RID: 11602
		public float? StartTime;

		// Token: 0x04002D53 RID: 11603
		public float? MaxDuration;

		// Token: 0x04002D54 RID: 11604
		public float? Sleep;

		// Token: 0x04002D55 RID: 11605
		public float? SpacingOverride;

		// Token: 0x04002D56 RID: 11606
		public CassieStutterSettings? Stutter;
	}
}
