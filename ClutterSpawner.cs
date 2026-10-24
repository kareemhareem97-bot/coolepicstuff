using System;
using System.Collections.Generic;
using GameCore;
using MapGeneration;
using MapGeneration.Holidays;
using UnityEngine;

// Token: 0x020001AC RID: 428
public class ClutterSpawner : MonoBehaviour
{
	// Token: 0x0600085A RID: 2138 RVA: 0x00021514 File Offset: 0x0001F714
	private void Awake()
	{
		SeedSynchronizer.OnGenerationStage += this.OnMapStage;
	}

	// Token: 0x0600085B RID: 2139 RVA: 0x00021527 File Offset: 0x0001F727
	private void OnDestroy()
	{
		SeedSynchronizer.OnGenerationStage -= this.OnMapStage;
	}

	// Token: 0x0600085C RID: 2140 RVA: 0x0002153A File Offset: 0x0001F73A
	private void OnMapStage(MapGenerationPhase stage)
	{
		if (stage != MapGenerationPhase.ComplexDecorationsAndClutter)
		{
			return;
		}
		this.GenerateClutter();
	}

	// Token: 0x0600085D RID: 2141 RVA: 0x00021548 File Offset: 0x0001F748
	public void GenerateClutter()
	{
		bool flag = false;
		for (int i = this.clutters.Count - 1; i >= 0; i--)
		{
			ClutterStruct clutterStruct = this.clutters[i];
			GameCore.Console.AddDebugLog("MGCLTR", string.Concat(new string[]
			{
				"Checking spawn conditions for clutter struct \"",
				clutterStruct.descriptor,
				"\" on object \"",
				base.gameObject.name,
				"\""
			}), MessageImportance.LeastImportant, true);
			if (clutterStruct.clutterComponent && !clutterStruct.clutterComponent.spawned)
			{
				bool flag2;
				if (clutterStruct.chanceToSpawn <= 0f)
				{
					flag2 = false;
				}
				else if ((float)UnityEngine.Random.Range(1, 101) > clutterStruct.chanceToSpawn)
				{
					flag2 = false;
				}
				else
				{
					bool invertTimespan = clutterStruct.invertTimespan;
					bool flag3 = clutterStruct.targetHolidays.IsAnyHolidayActive(false);
					flag2 = (invertTimespan ? (!flag3) : flag3);
				}
				if (flag2 && (!this.IsExclusive || !flag))
				{
					clutterStruct.clutterComponent.SpawnClutter();
					flag = true;
				}
				else
				{
					clutterStruct.clutterComponent.gameObject.SetActive(false);
					UnityEngine.Object.Destroy(clutterStruct.clutterComponent.holderObject);
				}
			}
		}
	}

	// Token: 0x040007A0 RID: 1952
	[SerializeField]
	private bool IsExclusive;

	// Token: 0x040007A1 RID: 1953
	[SerializeField]
	private List<ClutterStruct> clutters = new List<ClutterStruct>();
}
