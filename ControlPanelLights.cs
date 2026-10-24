using System;
using System.Collections.Generic;
using MEC;
using UnityEngine;

// Token: 0x0200005D RID: 93
public class ControlPanelLights : MonoBehaviour
{
	// Token: 0x0600022C RID: 556 RVA: 0x00008DB5 File Offset: 0x00006FB5
	private void Start()
	{
		Timing.RunCoroutine(this._Animate(), Segment.FixedUpdate);
	}

	// Token: 0x0600022D RID: 557 RVA: 0x00008DC4 File Offset: 0x00006FC4
	private IEnumerator<float> _Animate()
	{
		int i = this.emissions.Length;
		while (this != null)
		{
			if (this.targetMat != null)
			{
				this.targetMat.SetTexture(ControlPanelLights._emissionMap, this.emissions[UnityEngine.Random.Range(0, i)]);
			}
			yield return Timing.WaitForSeconds(UnityEngine.Random.Range(0.2f, 0.8f));
		}
		yield break;
	}

	// Token: 0x040001C6 RID: 454
	public Texture[] emissions;

	// Token: 0x040001C7 RID: 455
	public Material targetMat;

	// Token: 0x040001C8 RID: 456
	private static readonly int _emissionMap = Shader.PropertyToID("_EmissionMap");
}
