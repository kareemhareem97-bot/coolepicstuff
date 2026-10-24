using System;
using System.Diagnostics;
using CentralAuth;
using GameCore;
using InventorySystem;
using InventorySystem.Items;
using MapGeneration;
using PlayerRoles;
using Respawning.Waves;
using UnityEngine;

namespace Christmas.Scp2536.Gifts
{
	// Token: 0x020010F7 RID: 4343
	public class TapeGift : Scp2536GiftBase
	{
		// Token: 0x17001408 RID: 5128
		// (get) Token: 0x0600609B RID: 24731 RVA: 0x00004094 File Offset: 0x00002294
		public override UrgencyLevel Urgency
		{
			get
			{
				return UrgencyLevel.Zero;
			}
		}

		// Token: 0x0600609C RID: 24732 RVA: 0x001428B1 File Offset: 0x00140AB1
		public override void ServerGrant(ReferenceHub hub)
		{
			TapeGift.LastGiven.Restart();
			hub.inventory.ServerAddItem(ItemType.SCP1507Tape, ItemAddReason.Scp2536, 0, null);
			TapeGift._spawnChances = 30f;
		}

		// Token: 0x0600609D RID: 24733 RVA: 0x001428D8 File Offset: 0x00140AD8
		public override bool CanBeGranted(ReferenceHub hub)
		{
			if (!TapeGift._canSpawn)
			{
				return false;
			}
			if (RoundStart.RoundLength.TotalMinutes < 8.0 || TapeGift.LastGiven.Elapsed.TotalMinutes < 2.0)
			{
				return false;
			}
			float num = (float)WaveSpawner.GetAvailablePlayers(Team.Flamingos, null).Length;
			int playerCount = ReferenceHub.GetPlayerCount(ClientInstanceMode.ReadyClient, ClientInstanceMode.Dummy);
			return num >= (float)playerCount * 0.5f;
		}

		// Token: 0x0600609E RID: 24734 RVA: 0x0014294C File Offset: 0x00140B4C
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			SeedSynchronizer.OnGenerationFinished += delegate()
			{
				double num = new System.Random(SeedSynchronizer.Seed).NextDouble() * 100.0;
				TapeGift._canSpawn = ((double)TapeGift._spawnChances >= num);
				if (!TapeGift._canSpawn)
				{
					TapeGift._spawnChances += 20f;
				}
			};
		}

		// Token: 0x0400464C RID: 17996
		private const float BaseChances = 30f;

		// Token: 0x0400464D RID: 17997
		private const float ChanceIncrementPerFail = 20f;

		// Token: 0x0400464E RID: 17998
		private static float _spawnChances = 30f;

		// Token: 0x0400464F RID: 17999
		private static bool _canSpawn;

		// Token: 0x04004650 RID: 18000
		private static readonly Stopwatch LastGiven = Stopwatch.StartNew();
	}
}
