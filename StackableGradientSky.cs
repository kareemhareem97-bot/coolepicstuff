using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000077 RID: 119
public class StackableGradientSky : MonoBehaviour
{
	// Token: 0x17000087 RID: 135
	// (get) Token: 0x06000295 RID: 661 RVA: 0x00009E50 File Offset: 0x00008050
	private float Weight
	{
		get
		{
			if (!this._volume.enabled)
			{
				return 0f;
			}
			return this._volume.weight;
		}
	}

	// Token: 0x06000296 RID: 662 RVA: 0x00009E70 File Offset: 0x00008070
	public static void GetCombinedColor(out Color top, out Color mid, out Color bottom, out float diffusion)
	{
		top = Color.black;
		mid = Color.black;
		bottom = Color.black;
		diffusion = 1f;
		foreach (StackableGradientSky stackableGradientSky in StackableGradientSky.ActiveInstances)
		{
			float weight = stackableGradientSky.Weight;
			top = Color.Lerp(top, stackableGradientSky._top, weight);
			mid = Color.Lerp(mid, stackableGradientSky._middle, weight);
			bottom = Color.Lerp(bottom, stackableGradientSky._bottom, weight);
			diffusion = Mathf.Lerp(diffusion, stackableGradientSky._gradientDiffusion, weight);
		}
	}

	// Token: 0x06000297 RID: 663 RVA: 0x00009F44 File Offset: 0x00008144
	private void Awake()
	{
		this._volume = base.GetComponent<Volume>();
		for (int i = 0; i < StackableGradientSky.ActiveInstances.Count; i++)
		{
			if (this._volume.priority <= StackableGradientSky.ActiveInstances[i]._volume.priority)
			{
				StackableGradientSky.ActiveInstances.Insert(i, this);
				return;
			}
		}
		StackableGradientSky.ActiveInstances.Add(this);
	}

	// Token: 0x06000298 RID: 664 RVA: 0x00009FAC File Offset: 0x000081AC
	private void OnDestroy()
	{
		StackableGradientSky.ActiveInstances.Remove(this);
	}

	// Token: 0x0400021B RID: 539
	[ColorUsage(true, true)]
	[SerializeField]
	private Color _top;

	// Token: 0x0400021C RID: 540
	[ColorUsage(true, true)]
	[SerializeField]
	private Color _middle;

	// Token: 0x0400021D RID: 541
	[ColorUsage(true, true)]
	[SerializeField]
	private Color _bottom;

	// Token: 0x0400021E RID: 542
	[SerializeField]
	private float _gradientDiffusion;

	// Token: 0x0400021F RID: 543
	private Volume _volume;

	// Token: 0x04000220 RID: 544
	private static readonly List<StackableGradientSky> ActiveInstances = new List<StackableGradientSky>();
}
