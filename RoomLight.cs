using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ProgressiveCulling;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UserSettings;
using UserSettings.VideoSettings;

// Token: 0x020001BA RID: 442
public class RoomLight : CullableBehaviour, IBoundsCullable, ICullable
{
	// Token: 0x17000152 RID: 338
	// (get) Token: 0x06000891 RID: 2193 RVA: 0x0002216A File Offset: 0x0002036A
	// (set) Token: 0x06000892 RID: 2194 RVA: 0x00022172 File Offset: 0x00020372
	internal Light LightSource { get; private set; }

	// Token: 0x17000153 RID: 339
	// (get) Token: 0x06000893 RID: 2195 RVA: 0x0002217B File Offset: 0x0002037B
	public override bool ShouldBeVisible
	{
		get
		{
			return this._preventSubCulling || CullingCamera.CheckBoundsVisibility(this.WorldspaceBounds);
		}
	}

	// Token: 0x17000154 RID: 340
	// (get) Token: 0x06000894 RID: 2196 RVA: 0x00022192 File Offset: 0x00020392
	public Bounds WorldspaceBounds
	{
		get
		{
			this.UpdateCache();
			return new Bounds(this._centerTr.position, Vector3.one * this._lightSize);
		}
	}

	// Token: 0x06000895 RID: 2197 RVA: 0x000221BC File Offset: 0x000203BC
	private void UpdateCache()
	{
		if (this._cacheSet)
		{
			return;
		}
		this._cacheSet = true;
		this._rendererCount = this._renderers.Length;
		this.LightSource = base.GetComponentInChildren<Light>();
		this.HasLight = (this.LightSource != null);
		if (!this.HasLight)
		{
			this._centerTr = base.transform;
			return;
		}
		this._centerTr = this.LightSource.transform;
		this._initialLightColor = this.LightSource.color;
		this._lightSize = this.LightSource.range * 2f;
		HDAdditionalLightData hdadditionalLightData;
		if (!this.LightSource.TryGetComponent<HDAdditionalLightData>(out hdadditionalLightData))
		{
			return;
		}
		HDLightType type = hdadditionalLightData.type;
		if (type == HDLightType.Directional || type == HDLightType.Area)
		{
			this._preventSubCulling = true;
		}
	}

	// Token: 0x06000896 RID: 2198 RVA: 0x00022279 File Offset: 0x00020479
	private void OnValidate()
	{
		this._cacheSet = false;
	}

	// Token: 0x06000897 RID: 2199 RVA: 0x00022284 File Offset: 0x00020484
	private void Awake()
	{
		this.UpdateCache();
		CullableRoom cullableRoom;
		if (this._centerTr.TryGetComponentInParent(out cullableRoom))
		{
			cullableRoom.AddChildCullable(this);
		}
		if (this._rendererCount <= 0)
		{
			return;
		}
		this._copy = new Material(this._renderers[0].sharedMaterials[this._materialId]);
		for (int i = 0; i < this._rendererCount; i++)
		{
			Material[] sharedMaterials = this._renderers[i].sharedMaterials;
			sharedMaterials[this._materialId] = this._copy;
			this._renderers[i].materials = sharedMaterials;
		}
		if (this._copy.HasProperty(RoomLight.EmissionColor))
		{
			this._useEmissionMatProperty = true;
			this._initialMaterialColor = this._copy.GetColor(RoomLight.EmissionColor);
		}
		if (this._copy.HasProperty(RoomLight.BlackoutProperty))
		{
			this._useBlackoutMatProperty = true;
		}
		this.UpdateWarhead(AlphaWarheadController.InProgress);
	}

	// Token: 0x06000898 RID: 2200 RVA: 0x00022364 File Offset: 0x00020564
	protected override void UpdateVisible()
	{
		base.UpdateVisible();
		if (!this._isDirty)
		{
			return;
		}
		this.UpdateIntensity();
	}

	// Token: 0x06000899 RID: 2201 RVA: 0x0002237B File Offset: 0x0002057B
	private void OnDestroy()
	{
		RoomLight.RenderedInstances.Remove(this);
	}

	// Token: 0x0600089A RID: 2202 RVA: 0x00022389 File Offset: 0x00020589
	private void MarkAsDirty()
	{
		this._isDirty = true;
		this.UpdateIntensity();
	}

	// Token: 0x0600089B RID: 2203 RVA: 0x00022398 File Offset: 0x00020598
	private void UpdateWarhead(bool val)
	{
		this._warheadInProgress = val;
		this.MarkAsDirty();
	}

	// Token: 0x0600089C RID: 2204 RVA: 0x000223A8 File Offset: 0x000205A8
	private void UpdateIntensity()
	{
		Color matColor;
		Color lightColor;
		if (this._overrideColorSet)
		{
			matColor = this._overrideColor;
			lightColor = this._overrideColor;
		}
		else
		{
			matColor = (this._warheadInProgress ? RoomLight.DefaultWarheadColor : this._initialMaterialColor);
			lightColor = (this._warheadInProgress ? RoomLight.DefaultWarheadColor : this._initialLightColor);
		}
		if (!this._blackoutAnimProgress.IsRunning)
		{
			this._isDirty = false;
			this.SetColors(lightColor, matColor, (float)(this._targetBlackout ? 0 : 1));
			return;
		}
		float num = Mathf.Clamp01((float)this._blackoutAnimProgress.Elapsed.TotalSeconds);
		float intensity = this._targetBlackout ? this.FlickerBlackout(num) : this.FlickerReEnabling(num);
		this.SetColors(lightColor, matColor, intensity);
		if (num < 1f)
		{
			return;
		}
		this._blackoutAnimProgress.Stop();
	}

	// Token: 0x0600089D RID: 2205 RVA: 0x00022474 File Offset: 0x00020674
	private float FlickerBlackout(float f)
	{
		float num = f * f * f * f;
		return Mathf.Sin(3.1415927f * f * 20f) * (1f - num);
	}

	// Token: 0x0600089E RID: 2206 RVA: 0x000224A4 File Offset: 0x000206A4
	private float FlickerReEnabling(float f)
	{
		if (Mathf.Approximately(f, 0f))
		{
			return 0f;
		}
		float num = f * f * f;
		return f / (Mathf.Cos(3.1415927f / num) + 2f);
	}

	// Token: 0x0600089F RID: 2207 RVA: 0x000224E0 File Offset: 0x000206E0
	private void SetColors(Color lightColor, Color matColor, float intensity)
	{
		if (this._excludeFromRecoloring)
		{
			return;
		}
		if (this._rendererCount > 0)
		{
			if (this._useEmissionMatProperty)
			{
				this._copy.SetColor(RoomLight.EmissionColor, matColor * intensity);
			}
			if (this._useBlackoutMatProperty)
			{
				this._copy.SetFloat(RoomLight.BlackoutProperty, intensity);
			}
		}
		if (this.HasLight)
		{
			this.LightSource.color = lightColor * intensity;
		}
	}

	// Token: 0x060008A0 RID: 2208 RVA: 0x00022551 File Offset: 0x00020751
	internal void SetOverrideColor(Color overrideColor)
	{
		this._overrideColorSet = true;
		this._overrideColor = overrideColor;
		this.MarkAsDirty();
	}

	// Token: 0x060008A1 RID: 2209 RVA: 0x00022567 File Offset: 0x00020767
	internal void ResetOverrideColor()
	{
		this._overrideColorSet = false;
		this.MarkAsDirty();
	}

	// Token: 0x060008A2 RID: 2210 RVA: 0x00022576 File Offset: 0x00020776
	internal void SetBlackout(bool state)
	{
		this._targetBlackout = state;
		this._blackoutAnimProgress.Restart();
		this.MarkAsDirty();
	}

	// Token: 0x060008A3 RID: 2211 RVA: 0x00022590 File Offset: 0x00020790
	protected override void OnVisibilityChanged(bool isVisible)
	{
		base.gameObject.SetActive(isVisible && RoomLight.FacilityLightsSetting.Value);
		if (isVisible)
		{
			this.UpdateWarhead(AlphaWarheadController.InProgress);
			RoomLight.RenderedInstances.Add(this);
			return;
		}
		RoomLight.RenderedInstances.Remove(this);
	}

	// Token: 0x060008A4 RID: 2212 RVA: 0x000225E0 File Offset: 0x000207E0
	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		Action<bool> value;
		if ((value = RoomLight.<>O.<0>__OnWarheadChanged) == null)
		{
			value = (RoomLight.<>O.<0>__OnWarheadChanged = new Action<bool>(RoomLight.OnWarheadChanged));
		}
		AlphaWarheadController.OnProgressChanged += value;
		LightingVideoSetting key = LightingVideoSetting.RenderLights;
		Action<bool> listenerEvent;
		if ((listenerEvent = RoomLight.<>O.<1>__OnRenderLightsSettingChanged) == null)
		{
			listenerEvent = (RoomLight.<>O.<1>__OnRenderLightsSettingChanged = new Action<bool>(RoomLight.OnRenderLightsSettingChanged));
		}
		UserSetting<bool>.AddListener<LightingVideoSetting>(key, listenerEvent);
	}

	// Token: 0x060008A5 RID: 2213 RVA: 0x00022630 File Offset: 0x00020830
	private static void OnRenderLightsSettingChanged(bool newSetting)
	{
		foreach (RoomLight roomLight in RoomLight.RenderedInstances)
		{
			if (!roomLight.IsCulled)
			{
				roomLight.gameObject.SetActive(newSetting);
			}
		}
	}

	// Token: 0x060008A6 RID: 2214 RVA: 0x00022690 File Offset: 0x00020890
	private static void OnWarheadChanged(bool newProgress)
	{
		foreach (RoomLight roomLight in RoomLight.RenderedInstances)
		{
			roomLight.UpdateWarhead(newProgress);
		}
	}

	// Token: 0x040007D9 RID: 2009
	public static readonly Color DefaultWarheadColor = new Color(1f, 0.2f, 0.2f);

	// Token: 0x040007DA RID: 2010
	private static readonly int EmissionColor = Shader.PropertyToID("_EmissiveColor");

	// Token: 0x040007DB RID: 2011
	private static readonly int BlackoutProperty = Shader.PropertyToID("_BlackoutEmissionMultiplier");

	// Token: 0x040007DC RID: 2012
	private static readonly CachedUserSetting<bool> FacilityLightsSetting = new CachedUserSetting<bool>(LightingVideoSetting.RenderLights);

	// Token: 0x040007DD RID: 2013
	private static readonly HashSet<RoomLight> RenderedInstances = new HashSet<RoomLight>();

	// Token: 0x040007DE RID: 2014
	[SerializeField]
	private int _materialId;

	// Token: 0x040007DF RID: 2015
	[SerializeField]
	private Renderer[] _renderers;

	// Token: 0x040007E0 RID: 2016
	[SerializeField]
	private bool _excludeFromRecoloring;

	// Token: 0x040007E1 RID: 2017
	[Tooltip("Only used if no light source is detected. Does not need to be accurate, default should be sufficent unless you see objects culling while visible.")]
	[SerializeField]
	private float _lightSize = 3f;

	// Token: 0x040007E2 RID: 2018
	private Material _copy;

	// Token: 0x040007E3 RID: 2019
	private Transform _centerTr;

	// Token: 0x040007E4 RID: 2020
	private int _rendererCount;

	// Token: 0x040007E5 RID: 2021
	private bool _useEmissionMatProperty;

	// Token: 0x040007E6 RID: 2022
	private bool _useBlackoutMatProperty;

	// Token: 0x040007E7 RID: 2023
	private bool _isDirty;

	// Token: 0x040007E8 RID: 2024
	private bool _cacheSet;

	// Token: 0x040007E9 RID: 2025
	private bool _warheadInProgress;

	// Token: 0x040007EA RID: 2026
	private bool _overrideColorSet;

	// Token: 0x040007EB RID: 2027
	private bool _preventSubCulling;

	// Token: 0x040007EC RID: 2028
	private bool _targetBlackout;

	// Token: 0x040007ED RID: 2029
	private readonly Stopwatch _blackoutAnimProgress = new Stopwatch();

	// Token: 0x040007EE RID: 2030
	private Color _overrideColor;

	// Token: 0x040007EF RID: 2031
	private Color _initialLightColor;

	// Token: 0x040007F0 RID: 2032
	private Color _initialMaterialColor;

	// Token: 0x040007F1 RID: 2033
	internal bool HasLight;

	// Token: 0x020001BB RID: 443
	[CompilerGenerated]
	private static class <>O
	{
		// Token: 0x040007F3 RID: 2035
		public static Action<bool> <0>__OnWarheadChanged;

		// Token: 0x040007F4 RID: 2036
		public static Action<bool> <1>__OnRenderLightsSettingChanged;
	}
}
