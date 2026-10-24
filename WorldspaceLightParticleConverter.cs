using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200012D RID: 301
public class WorldspaceLightParticleConverter : MonoBehaviour
{
	// Token: 0x170000E6 RID: 230
	// (get) Token: 0x060005B6 RID: 1462 RVA: 0x00015247 File Offset: 0x00013447
	private static Material InvisibleMaterial
	{
		get
		{
			if (!WorldspaceLightParticleConverter._matLoaded)
			{
				WorldspaceLightParticleConverter._invisibleMat = Resources.Load<Material>("StaticMaterials/Invisible");
				WorldspaceLightParticleConverter._matLoaded = true;
			}
			return WorldspaceLightParticleConverter._invisibleMat;
		}
	}

	// Token: 0x060005B7 RID: 1463 RVA: 0x0001526A File Offset: 0x0001346A
	public static void Convert(ParticleSystem system, bool includeSubsystems)
	{
		if (includeSubsystems)
		{
			GameObject gameObject = system.gameObject;
			Action<ParticleSystem> action;
			if ((action = WorldspaceLightParticleConverter.<>O.<0>__ConvertIndividual) == null)
			{
				action = (WorldspaceLightParticleConverter.<>O.<0>__ConvertIndividual = new Action<ParticleSystem>(WorldspaceLightParticleConverter.ConvertIndividual));
			}
			gameObject.ForEachComponentInChildren(action, true);
			return;
		}
		WorldspaceLightParticleConverter.ConvertIndividual(system);
	}

	// Token: 0x060005B8 RID: 1464 RVA: 0x0001529D File Offset: 0x0001349D
	public static void Convert(ParticleSystem system)
	{
		WorldspaceLightParticleConverter.Convert(system, true);
	}

	// Token: 0x060005B9 RID: 1465 RVA: 0x000152A8 File Offset: 0x000134A8
	private static void ConvertIndividual(ParticleSystem system)
	{
		if (!system.lights.enabled || system.lights.light == null)
		{
			return;
		}
		Transform transform = system.transform;
		ParticleSystem particleSystem = UnityEngine.Object.Instantiate<ParticleSystem>(system, transform);
		Transform transform2 = particleSystem.transform;
		ParticleSystem.MainModule main = particleSystem.main;
		transform2.localPosition = Vector3.zero;
		transform2.localRotation = Quaternion.identity;
		ParticleSystemScalingMode scalingMode = main.scalingMode;
		if (scalingMode != ParticleSystemScalingMode.Hierarchy)
		{
			if (scalingMode == ParticleSystemScalingMode.Local)
			{
				transform2.localScale = transform.localScale;
			}
		}
		else
		{
			transform2.localScale = Vector3.one;
		}
		particleSystem.gameObject.layer = 0;
		particleSystem.GetComponent<ParticleSystemRenderer>().sharedMaterial = WorldspaceLightParticleConverter.InvisibleMaterial;
		main.duration = Mathf.Max(main.duration, 0.025f);
		Transform transform3 = particleSystem.lights.light.transform;
		for (int i = 0; i < transform2.childCount; i++)
		{
			Transform child = transform2.GetChild(i);
			if (!(child == transform3))
			{
				UnityEngine.Object.Destroy(child.gameObject);
			}
		}
	}

	// Token: 0x040004F6 RID: 1270
	private const string MaterialPath = "StaticMaterials/Invisible";

	// Token: 0x040004F7 RID: 1271
	private const float MinFlashDur = 0.025f;

	// Token: 0x040004F8 RID: 1272
	private static Material _invisibleMat;

	// Token: 0x040004F9 RID: 1273
	private static bool _matLoaded;

	// Token: 0x0200012E RID: 302
	[CompilerGenerated]
	private static class <>O
	{
		// Token: 0x040004FA RID: 1274
		public static Action<ParticleSystem> <0>__ConvertIndividual;
	}
}
