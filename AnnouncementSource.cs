using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MapGeneration;
using PlayerRoles.PlayableScps.Scp079;
using UnityEngine;
using UserSettings;
using UserSettings.AudioSettings;

// Token: 0x02000055 RID: 85
public class AnnouncementSource : MonoBehaviour
{
	// Token: 0x1700007A RID: 122
	// (get) Token: 0x060001FF RID: 511 RVA: 0x0000857D File Offset: 0x0000677D
	private static Vector3 LastCamPos
	{
		get
		{
			return MainCameraController.LastPosition;
		}
	}

	// Token: 0x06000200 RID: 512 RVA: 0x00008584 File Offset: 0x00006784
	private void Awake()
	{
		this._tr = base.transform;
		AnnouncementSource._singleton = this;
		MainCameraController.OnUpdated += this.OnCamPosUpdated;
		AnnouncementSource.SourceSettingsPair[] sources = AnnouncementSource._singleton._sources;
		for (int i = 0; i < sources.Length; i++)
		{
			sources[i].Init();
		}
	}

	// Token: 0x06000201 RID: 513 RVA: 0x000085D5 File Offset: 0x000067D5
	private void OnDestroy()
	{
		if (AnnouncementSource._singleton == this)
		{
			AnnouncementSource._singleton = null;
		}
		MainCameraController.OnUpdated -= this.OnCamPosUpdated;
	}

	// Token: 0x06000202 RID: 514 RVA: 0x000085F8 File Offset: 0x000067F8
	private void OnCamPosUpdated()
	{
		if (!AnnouncementSource.SpatializationSetting.Value)
		{
			this._sources.ForEach(delegate(AnnouncementSource.SourceSettingsPair x)
			{
				x.SetDistance(1f);
			});
			return;
		}
		this.UpdateNearby();
		int count = AnnouncementSource.NearbyPositions.Count;
		Vector3 vector;
		float normalizedDistance;
		if (count != 0)
		{
			if (count != 1)
			{
				this.ResolveMultipleNearbySources(out vector, out normalizedDistance);
			}
			else
			{
				vector = AnnouncementSource.NearbyPositions[0];
				normalizedDistance = this.GetNormalizedDistanceToCam(vector);
			}
		}
		else
		{
			vector = default(Vector3);
			normalizedDistance = 1f;
		}
		this.SetSources(vector, normalizedDistance);
	}

	// Token: 0x06000203 RID: 515 RVA: 0x00008690 File Offset: 0x00006890
	private void UpdateNearby()
	{
		AnnouncementSource.NearbyPositions.Clear();
		float num = this._2dDistance * this._2dDistance;
		Vector3 lastCamPos = AnnouncementSource.LastCamPos;
		foreach (Vector3 vector in AnnouncementSource.AllSpeakerPositions)
		{
			if ((vector - lastCamPos).sqrMagnitude <= num)
			{
				AnnouncementSource.NearbyPositions.Add(vector);
			}
		}
	}

	// Token: 0x06000204 RID: 516 RVA: 0x00008718 File Offset: 0x00006918
	private void ResolveMultipleNearbySources(out Vector3 sourcePos, out float normalizedDistance)
	{
		Vector3 lastCamPos = AnnouncementSource.LastCamPos;
		int count = AnnouncementSource.NearbyPositions.Count;
		Vector3 a = Vector3.zero;
		float num = float.MaxValue;
		float num2 = 0f;
		for (int i = 0; i < count; i++)
		{
			Vector3 vector = AnnouncementSource.NearbyPositions[i];
			float normalizedDistanceToCam = this.GetNormalizedDistanceToCam(vector);
			num = Mathf.Min(num, normalizedDistanceToCam);
			float num3 = 1f - normalizedDistanceToCam * normalizedDistanceToCam;
			num2 += num3;
			a += (vector - lastCamPos) * num3;
		}
		if (num2 < 0.01f)
		{
			sourcePos = default(Vector3);
			normalizedDistance = 1f;
			return;
		}
		Vector3 a2 = a / num2;
		sourcePos = a2 + lastCamPos;
		normalizedDistance = num;
	}

	// Token: 0x06000205 RID: 517 RVA: 0x000087D8 File Offset: 0x000069D8
	private void SetSources(Vector3 sourcePos, float normalizedDistance)
	{
		this._tr.position = sourcePos;
		AnnouncementSource.SourceSettingsPair[] sources = this._sources;
		for (int i = 0; i < sources.Length; i++)
		{
			sources[i].SetDistance(normalizedDistance);
		}
	}

	// Token: 0x06000206 RID: 518 RVA: 0x0000880F File Offset: 0x00006A0F
	private float GetNormalizedDistanceToCam(Vector3 pos)
	{
		return Mathf.Clamp01(Vector3.Distance(AnnouncementSource.LastCamPos, pos) / this._2dDistance);
	}

	// Token: 0x06000207 RID: 519 RVA: 0x00008828 File Offset: 0x00006A28
	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		Action value;
		if ((value = AnnouncementSource.<>O.<0>__RefreshPositions) == null)
		{
			value = (AnnouncementSource.<>O.<0>__RefreshPositions = new Action(AnnouncementSource.RefreshPositions));
		}
		SeedSynchronizer.OnGenerationFinished += value;
	}

	// Token: 0x06000208 RID: 520 RVA: 0x0000884C File Offset: 0x00006A4C
	private static void RefreshPositions()
	{
		AnnouncementSource.AllSpeakerPositions.Clear();
		foreach (Scp079InteractableBase scp079InteractableBase in Scp079InteractableBase.AllInstances)
		{
			if (scp079InteractableBase is Scp079Speaker)
			{
				AnnouncementSource.AllSpeakerPositions.Add(scp079InteractableBase.Position);
			}
		}
	}

	// Token: 0x06000209 RID: 521 RVA: 0x000088BC File Offset: 0x00006ABC
	public static void SetVolumeScale(AudioSource target, float scale)
	{
		if (AnnouncementSource._singleton == null)
		{
			return;
		}
		AnnouncementSource.SourceSettingsPair[] sources = AnnouncementSource._singleton._sources;
		for (int i = 0; i < sources.Length; i++)
		{
			sources[i].SetVolumeScale(target, scale);
		}
	}

	// Token: 0x040001AD RID: 429
	private static readonly List<Vector3> AllSpeakerPositions = new List<Vector3>();

	// Token: 0x040001AE RID: 430
	private static readonly List<Vector3> NearbyPositions = new List<Vector3>();

	// Token: 0x040001AF RID: 431
	private static readonly CachedUserSetting<bool> SpatializationSetting = new CachedUserSetting<bool>(OtherAudioSetting.SpatialAnnouncements);

	// Token: 0x040001B0 RID: 432
	private static AnnouncementSource _singleton;

	// Token: 0x040001B1 RID: 433
	private Transform _tr;

	// Token: 0x040001B2 RID: 434
	[SerializeField]
	private float _2dDistance;

	// Token: 0x040001B3 RID: 435
	[SerializeField]
	private AnnouncementSource.SourceSettingsPair[] _sources;

	// Token: 0x02000056 RID: 86
	[Serializable]
	private class SourceSettingsPair
	{
		// Token: 0x0600020C RID: 524 RVA: 0x0000891C File Offset: 0x00006B1C
		public void Init()
		{
			this._volumeMultipliers = new float[this.Sources.Length];
			for (int i = 0; i < this.Sources.Length; i++)
			{
				this._volumeMultipliers[i] = 1f;
			}
		}

		// Token: 0x0600020D RID: 525 RVA: 0x0000895C File Offset: 0x00006B5C
		public void SetVolumeScale(AudioSource target, float scale)
		{
			for (int i = 0; i < this.Sources.Length; i++)
			{
				AudioSource audioSource = this.Sources[i];
				if (target == audioSource)
				{
					this._volumeMultipliers[i] = scale;
					return;
				}
			}
		}

		// Token: 0x0600020E RID: 526 RVA: 0x00008994 File Offset: 0x00006B94
		public void SetDistance(float normalizedDistance)
		{
			float num = this.VolumeOverNormalizedDistance.Evaluate(normalizedDistance);
			float num2 = this.SpatialBlendOverNormalizedDistance.Evaluate(normalizedDistance);
			float spread = 360f * (1f - num2);
			for (int i = 0; i < this.Sources.Length; i++)
			{
				AudioSource audioSource = this.Sources[i];
				float num3 = this._volumeMultipliers[i];
				audioSource.spatialBlend = num2;
				audioSource.spread = spread;
				audioSource.volume = num * num3;
			}
		}

		// Token: 0x040001B4 RID: 436
		public AudioSource[] Sources;

		// Token: 0x040001B5 RID: 437
		public AnimationCurve VolumeOverNormalizedDistance;

		// Token: 0x040001B6 RID: 438
		public AnimationCurve SpatialBlendOverNormalizedDistance;

		// Token: 0x040001B7 RID: 439
		private float[] _volumeMultipliers;
	}

	// Token: 0x02000057 RID: 87
	[CompilerGenerated]
	private static class <>O
	{
		// Token: 0x040001B8 RID: 440
		public static Action <0>__RefreshPositions;
	}
}
