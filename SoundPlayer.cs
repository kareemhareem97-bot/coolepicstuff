using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000076 RID: 118
public class SoundPlayer : MonoBehaviour
{
	// Token: 0x06000290 RID: 656 RVA: 0x00009D5C File Offset: 0x00007F5C
	private void Start()
	{
		this._source = base.GetComponent<AudioSource>();
		this.LoadInspectorCurves();
	}

	// Token: 0x06000291 RID: 657 RVA: 0x00009D70 File Offset: 0x00007F70
	protected void LoadInspectorCurves()
	{
		foreach (CurvePreset curvePreset in this.Curves)
		{
			if (!this._curves.ContainsKey(curvePreset.Type))
			{
				this._curves.Add(curvePreset.Type, curvePreset);
			}
		}
	}

	// Token: 0x06000292 RID: 658 RVA: 0x00009DBC File Offset: 0x00007FBC
	public void Play(AudioClip clip, FalloffType falloff = FalloffType.Linear, float maxDistance = -1f)
	{
		CurvePreset curvePreset;
		if (!this._curves.TryGetValue(falloff, out curvePreset))
		{
			return;
		}
		this._source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curvePreset.FalloffCurve);
		if (maxDistance > 0f)
		{
			this._source.maxDistance = maxDistance;
		}
		this._source.clip = clip;
		this._source.Play();
	}

	// Token: 0x06000293 RID: 659 RVA: 0x00009E17 File Offset: 0x00008017
	public void Stop()
	{
		if (this._source.isPlaying)
		{
			this._source.Stop();
		}
		this._source.clip = null;
	}

	// Token: 0x04000218 RID: 536
	protected AudioSource _source;

	// Token: 0x04000219 RID: 537
	public CurvePreset[] Curves;

	// Token: 0x0400021A RID: 538
	private readonly Dictionary<FalloffType, CurvePreset> _curves = new Dictionary<FalloffType, CurvePreset>();
}
