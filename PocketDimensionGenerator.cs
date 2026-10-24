using System;
using System.Linq;
using System.Runtime.CompilerServices;
using GameCore;
using Mirror;
using UnityEngine;

// Token: 0x02000146 RID: 326
public static class PocketDimensionGenerator
{
	// Token: 0x06000639 RID: 1593 RVA: 0x000178E0 File Offset: 0x00015AE0
	public static void RandomizeTeleports()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		System.Random random = Misc.CreateRandom();
		PocketDimensionTeleport[] array = PocketDimensionGenerator.PrepTeleports();
		int @int = ConfigFile.ServerConfig.GetInt("pd_exit_count", 2);
		int num = 0;
		while (num < @int && PocketDimensionGenerator.ContainsKiller(array))
		{
			int num2 = -1;
			while ((num2 < 0 || array[num2].GetTeleportType() == PocketDimensionTeleport.PDTeleportType.Exit) && PocketDimensionGenerator.ContainsKiller(array))
			{
				num2 = random.Next(0, array.Length);
			}
			array[Mathf.Clamp(num2, 0, array.Length - 1)].SetType(PocketDimensionTeleport.PDTeleportType.Exit);
			num++;
		}
	}

	// Token: 0x0600063A RID: 1594 RVA: 0x00017968 File Offset: 0x00015B68
	private static PocketDimensionTeleport[] PrepTeleports()
	{
		PocketDimensionTeleport[] array = PocketDimensionTeleport.AllInstances.ToArray<PocketDimensionTeleport>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetType(PocketDimensionTeleport.PDTeleportType.Killer);
		}
		return array;
	}

	// Token: 0x0600063B RID: 1595 RVA: 0x00017998 File Offset: 0x00015B98
	private static bool ContainsKiller(PocketDimensionTeleport[] pdtps)
	{
		for (int i = 0; i < pdtps.Length; i++)
		{
			if (pdtps[i].GetTeleportType() == PocketDimensionTeleport.PDTeleportType.Killer)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600063C RID: 1596 RVA: 0x000179C0 File Offset: 0x00015BC0
	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		Action value;
		if ((value = PocketDimensionGenerator.<>O.<0>__RandomizeTeleports) == null)
		{
			value = (PocketDimensionGenerator.<>O.<0>__RandomizeTeleports = new Action(PocketDimensionGenerator.RandomizeTeleports));
		}
		PocketDimensionTeleport.OnInstancesUpdated += value;
	}

	// Token: 0x02000147 RID: 327
	[CompilerGenerated]
	private static class <>O
	{
		// Token: 0x040005C8 RID: 1480
		public static Action <0>__RandomizeTeleports;
	}
}
