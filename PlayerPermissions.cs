using System;

// Token: 0x02000141 RID: 321
[Flags]
public enum PlayerPermissions : ulong
{
	// Token: 0x04000598 RID: 1432
	KickingAndShortTermBanning = 1UL,
	// Token: 0x04000599 RID: 1433
	BanningUpToDay = 2UL,
	// Token: 0x0400059A RID: 1434
	LongTermBanning = 4UL,
	// Token: 0x0400059B RID: 1435
	ForceclassSelf = 8UL,
	// Token: 0x0400059C RID: 1436
	ForceclassToSpectator = 16UL,
	// Token: 0x0400059D RID: 1437
	ForceclassWithoutRestrictions = 32UL,
	// Token: 0x0400059E RID: 1438
	GivingItems = 64UL,
	// Token: 0x0400059F RID: 1439
	WarheadEvents = 128UL,
	// Token: 0x040005A0 RID: 1440
	RespawnEvents = 256UL,
	// Token: 0x040005A1 RID: 1441
	RoundEvents = 512UL,
	// Token: 0x040005A2 RID: 1442
	SetGroup = 1024UL,
	// Token: 0x040005A3 RID: 1443
	GameplayData = 2048UL,
	// Token: 0x040005A4 RID: 1444
	Overwatch = 4096UL,
	// Token: 0x040005A5 RID: 1445
	FacilityManagement = 8192UL,
	// Token: 0x040005A6 RID: 1446
	PlayersManagement = 16384UL,
	// Token: 0x040005A7 RID: 1447
	PermissionsManagement = 32768UL,
	// Token: 0x040005A8 RID: 1448
	ServerConsoleCommands = 65536UL,
	// Token: 0x040005A9 RID: 1449
	ViewHiddenBadges = 131072UL,
	// Token: 0x040005AA RID: 1450
	ServerConfigs = 262144UL,
	// Token: 0x040005AB RID: 1451
	Broadcasting = 524288UL,
	// Token: 0x040005AC RID: 1452
	PlayerSensitiveDataAccess = 1048576UL,
	// Token: 0x040005AD RID: 1453
	Noclip = 2097152UL,
	// Token: 0x040005AE RID: 1454
	AFKImmunity = 4194304UL,
	// Token: 0x040005AF RID: 1455
	AdminChat = 8388608UL,
	// Token: 0x040005B0 RID: 1456
	ViewHiddenGlobalBadges = 16777216UL,
	// Token: 0x040005B1 RID: 1457
	Announcer = 33554432UL,
	// Token: 0x040005B2 RID: 1458
	Effects = 67108864UL,
	// Token: 0x040005B3 RID: 1459
	FriendlyFireDetectorImmunity = 134217728UL,
	// Token: 0x040005B4 RID: 1460
	FriendlyFireDetectorTempDisable = 268435456UL,
	// Token: 0x040005B5 RID: 1461
	ServerLogLiveFeed = 536870912UL,
	// Token: 0x040005B6 RID: 1462
	ExecuteAs = 1073741824UL,
	// Token: 0x040005B7 RID: 1463
	Vanish = 2147483648UL
}
