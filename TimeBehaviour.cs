using System;

// Token: 0x0200022A RID: 554
public static class TimeBehaviour
{
	// Token: 0x06000C4C RID: 3148 RVA: 0x00030440 File Offset: 0x0002E640
	public static long CurrentTimestamp()
	{
		return DateTime.UtcNow.Ticks;
	}

	// Token: 0x17000171 RID: 369
	// (get) Token: 0x06000C4D RID: 3149 RVA: 0x0003045C File Offset: 0x0002E65C
	public static long CurrentUnixTimestamp
	{
		get
		{
			return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		}
	}

	// Token: 0x06000C4E RID: 3150 RVA: 0x00030478 File Offset: 0x0002E678
	public static long GetBanExpirationTime(uint seconds)
	{
		return DateTime.UtcNow.AddSeconds(seconds).Ticks;
	}

	// Token: 0x06000C4F RID: 3151 RVA: 0x0003049D File Offset: 0x0002E69D
	public static bool ValidateTimestamp(long timestampentry, long timestampexit, long limit)
	{
		return timestampexit - timestampentry < limit;
	}

	// Token: 0x06000C50 RID: 3152 RVA: 0x000304A5 File Offset: 0x0002E6A5
	public static string Rfc3339Time()
	{
		return TimeBehaviour.Rfc3339Time(DateTimeOffset.Now);
	}

	// Token: 0x06000C51 RID: 3153 RVA: 0x000304B1 File Offset: 0x0002E6B1
	public static string Rfc3339Time(DateTimeOffset date)
	{
		return TimeBehaviour.FormatTime((date.Offset == TimeSpan.Zero) ? "yyyy-MM-dd HH:mm:ss.fffZ" : "yyyy-MM-dd HH:mm:ss.fff zzz", date);
	}

	// Token: 0x06000C52 RID: 3154 RVA: 0x000304D8 File Offset: 0x0002E6D8
	public static string FormatTime(string format)
	{
		return TimeBehaviour.FormatTime(format, DateTimeOffset.Now);
	}

	// Token: 0x06000C53 RID: 3155 RVA: 0x000304E8 File Offset: 0x0002E6E8
	public static string FormatTime(string format, DateTimeOffset date)
	{
		return format.Replace("yyyy", date.Year.ToString()).Replace("MM", Misc.LeadingZeroes(date.Month, 2U, false)).Replace("M", date.Month.ToString()).Replace("dd", Misc.LeadingZeroes(date.Day, 2U, false)).Replace("d", date.Day.ToString()).Replace("HH", Misc.LeadingZeroes(date.Hour, 2U, false)).Replace("H", date.Hour.ToString()).Replace("mm", Misc.LeadingZeroes(date.Minute, 2U, false)).Replace("m", date.Minute.ToString()).Replace("ss", Misc.LeadingZeroes(date.Second, 2U, false)).Replace("s", date.Second.ToString()).Replace("fff", Misc.LeadingZeroes(date.Millisecond, 3U, false)).Replace("ff", Misc.LeadingZeroes(date.Millisecond / 10, 2U, false)).Replace("f", (date.Millisecond / 100).ToString()).Replace("zzz", Misc.LeadingZeroes(date.Offset.Hours, 2U, true) + ":" + Misc.LeadingZeroes(date.Offset.Minutes, 2U, false)).Replace("zz", Misc.LeadingZeroes(date.Offset.Hours, 2U, true)).Replace("z", Misc.LeadingZeroes(date.Offset.Hours, 1U, true));
	}
}
