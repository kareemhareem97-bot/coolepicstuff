using System;
using UnityEngine;

namespace FacilitySoundtrack
{
	// Token: 0x02000879 RID: 2169
	public class SoundtrackManager : MonoBehaviour
	{
		// Token: 0x0600317E RID: 12670 RVA: 0x000A661C File Offset: 0x000A481C
		private void Update()
		{
			int num = this._layers.Length - 1;
			float num2 = 0f;
			for (int i = num; i >= 0; i--)
			{
				float num3 = Mathf.Clamp01(this._layers[i].Weight);
				float volumeScale = Mathf.Max(1f - num2, 0f) * num3;
				if (!this._layers[i].Additive)
				{
					num2 += num3;
				}
				this._layers[i].UpdateVolume(volumeScale);
			}
		}

		// Token: 0x040026AE RID: 9902
		[SerializeField]
		private SoundtrackLayerBase[] _layers;
	}
}
