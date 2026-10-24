using System;

// Token: 0x02000179 RID: 377
[Flags]
public enum CentralAuthPreauthFlags : byte
{
	// Token: 0x040006B6 RID: 1718
	None = 0,
	// Token: 0x040006B7 RID: 1719
	ReservedSlot = 1,
	// Token: 0x040006B8 RID: 1720
	IgnoreBans = 2,
	// Token: 0x040006B9 RID: 1721
	IgnoreWhitelist = 4,
	// Token: 0x040006BA RID: 1722
	IgnoreGeoblock = 8,
	// Token: 0x040006BB RID: 1723
	GloballyBanned = 16,
	// Token: 0x040006BC RID: 1724
	NorthwoodStaff = 32,
	// Token: 0x040006BD RID: 1725
	AuthRejected = 64
}
