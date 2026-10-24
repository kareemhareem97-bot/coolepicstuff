using System;
using System.Collections.Generic;
using MEC;
using UnityEngine;

// Token: 0x0200007C RID: 124
public class TextureAnimator : MonoBehaviour
{
	// Token: 0x060002AB RID: 683 RVA: 0x0000A2DE File Offset: 0x000084DE
	private void Start()
	{
		Timing.RunCoroutine(this._Animate(), Segment.FixedUpdate);
	}

	// Token: 0x060002AC RID: 684 RVA: 0x0000A2ED File Offset: 0x000084ED
	private IEnumerator<float> _Animate()
	{
		while (this != null)
		{
			int num;
			for (int i = 0; i < this.textures.Length; i = num + 1)
			{
				this.optionalLight.enabled = (i < this.lightRange);
				this.targetRenderer.material = this.textures[i];
				int x = 0;
				while ((float)x < 50f * this.cooldown)
				{
					yield return 0f;
					num = x;
					x = num + 1;
				}
				num = i;
			}
		}
		yield break;
	}

	// Token: 0x04000235 RID: 565
	public Material[] textures;

	// Token: 0x04000236 RID: 566
	public Renderer targetRenderer;

	// Token: 0x04000237 RID: 567
	public float cooldown;

	// Token: 0x04000238 RID: 568
	public Light optionalLight;

	// Token: 0x04000239 RID: 569
	public int lightRange;
}
