using System;

// Token: 0x02000044 RID: 68
[Serializable]
public struct AlphaWarheadSyncInfo : IEquatable<AlphaWarheadSyncInfo>
{
	// Token: 0x1700006E RID: 110
	// (get) Token: 0x060001BF RID: 447 RVA: 0x00007B58 File Offset: 0x00005D58
	public bool InProgress
	{
		get
		{
			return this.StartTime != 0.0;
		}
	}

	// Token: 0x060001C0 RID: 448 RVA: 0x00007B6E File Offset: 0x00005D6E
	public override int GetHashCode()
	{
		return (int)((byte)((int)(this.StartTime * 10.0) % 32767 * 255 + (int)this.ScenarioId) + this.ScenarioType);
	}

	// Token: 0x060001C1 RID: 449 RVA: 0x00007B9B File Offset: 0x00005D9B
	public static bool operator ==(AlphaWarheadSyncInfo left, AlphaWarheadSyncInfo right)
	{
		return left.Equals(right);
	}

	// Token: 0x060001C2 RID: 450 RVA: 0x00007BA5 File Offset: 0x00005DA5
	public static bool operator !=(AlphaWarheadSyncInfo left, AlphaWarheadSyncInfo right)
	{
		return !left.Equals(right);
	}

	// Token: 0x060001C3 RID: 451 RVA: 0x00007BB2 File Offset: 0x00005DB2
	public bool Equals(AlphaWarheadSyncInfo other)
	{
		return other.ScenarioId == this.ScenarioId && other.StartTime == this.StartTime && other.ScenarioType == this.ScenarioType;
	}

	// Token: 0x060001C4 RID: 452 RVA: 0x00007BE0 File Offset: 0x00005DE0
	public override bool Equals(object obj)
	{
		if (obj is AlphaWarheadSyncInfo)
		{
			AlphaWarheadSyncInfo other = (AlphaWarheadSyncInfo)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x04000173 RID: 371
	public byte ScenarioId;

	// Token: 0x04000174 RID: 372
	public WarheadScenarioType ScenarioType;

	// Token: 0x04000175 RID: 373
	public double StartTime;
}
