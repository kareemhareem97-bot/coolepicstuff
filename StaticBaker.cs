using System;
using System.Runtime.CompilerServices;
using MapGeneration;
using MapGeneration.StaticHelpers;
using UnityEngine;
using UnityStandardAssets.Utility;

// Token: 0x02000100 RID: 256
public class StaticBaker : MonoBehaviour
{
	// Token: 0x060004D2 RID: 1234 RVA: 0x000120D1 File Offset: 0x000102D1
	private void Awake()
	{
		StaticBaker._currentRoot = base.gameObject;
	}

	// Token: 0x060004D3 RID: 1235 RVA: 0x000120DE File Offset: 0x000102DE
	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		Action<MapGenerationPhase> value;
		if ((value = StaticBaker.<>O.<0>__OnGenerationStage) == null)
		{
			value = (StaticBaker.<>O.<0>__OnGenerationStage = new Action<MapGenerationPhase>(StaticBaker.OnGenerationStage));
		}
		SeedSynchronizer.OnGenerationStage += value;
	}

	// Token: 0x060004D4 RID: 1236 RVA: 0x00012100 File Offset: 0x00010300
	private static void OnGenerationStage(MapGenerationPhase stage)
	{
		if (stage != MapGenerationPhase.StaticBatching)
		{
			return;
		}
		GameObject currentRoot = StaticBaker._currentRoot;
		foreach (Type type in StaticBaker.ExtractedTypes)
		{
			StaticBaker.ExtractType(currentRoot, type);
		}
		StaticBatchingUtility.Combine(currentRoot);
	}

	// Token: 0x060004D5 RID: 1237 RVA: 0x00012140 File Offset: 0x00010340
	private static void ExtractType(GameObject rootObject, Type type)
	{
		foreach (Component component in rootObject.GetComponentsInChildren(type, true))
		{
			if (StaticBaker.ShouldExtract(component))
			{
				component.transform.parent.gameObject.AddComponent<DynamicChildController>().SetChild(component.gameObject);
			}
		}
	}

	// Token: 0x060004D6 RID: 1238 RVA: 0x00012190 File Offset: 0x00010390
	private static bool ShouldExtract(Component component)
	{
		if (component.GetComponentInParent<Canvas>(true) != null)
		{
			return false;
		}
		foreach (Type t in StaticBaker.ExtractedTypes)
		{
			if (component.transform.parent.GetComponentInParent(t, true) != null)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0400042B RID: 1067
	private static readonly Type[] ExtractedTypes = new Type[]
	{
		typeof(Animator),
		typeof(AutoMoveAndRotate),
		typeof(IBlockStaticBatching)
	};

	// Token: 0x0400042C RID: 1068
	private static GameObject _currentRoot;

	// Token: 0x02000101 RID: 257
	[CompilerGenerated]
	private static class <>O
	{
		// Token: 0x0400042D RID: 1069
		public static Action<MapGenerationPhase> <0>__OnGenerationStage;
	}
}
