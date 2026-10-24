using System;
using MapGeneration.Holidays;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000DC RID: 220
public class HolidayBackgroundReplacer : MonoBehaviour
{
	// Token: 0x06000439 RID: 1081 RVA: 0x0001052C File Offset: 0x0000E72C
	private void Awake()
	{
		foreach (HolidayBackgroundReplacer.BackgroundInfo info in this._backgrounds)
		{
			this.RefreshBackground(info);
		}
	}

	// Token: 0x0600043A RID: 1082 RVA: 0x00010560 File Offset: 0x0000E760
	private void RefreshBackground(HolidayBackgroundReplacer.BackgroundInfo info)
	{
		Sprite sprite;
		if (!info.Variants.TryGetResult(out sprite))
		{
			return;
		}
		info.Target.sprite = sprite;
	}

	// Token: 0x040003BE RID: 958
	[SerializeField]
	private HolidayBackgroundReplacer.BackgroundInfo[] _backgrounds;

	// Token: 0x020000DD RID: 221
	[Serializable]
	private struct BackgroundInfo
	{
		// Token: 0x040003BF RID: 959
		public Image Target;

		// Token: 0x040003C0 RID: 960
		public HolidayBackgroundReplacer.HolidayInfo[] Variants;
	}

	// Token: 0x020000DE RID: 222
	[Serializable]
	private struct HolidayInfo : IHolidayFetchableData<Sprite>
	{
		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x0600043C RID: 1084 RVA: 0x00010589 File Offset: 0x0000E789
		// (set) Token: 0x0600043D RID: 1085 RVA: 0x00010591 File Offset: 0x0000E791
		public HolidayType Holiday { readonly get; private set; }

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x0600043E RID: 1086 RVA: 0x0001059A File Offset: 0x0000E79A
		// (set) Token: 0x0600043F RID: 1087 RVA: 0x000105A2 File Offset: 0x0000E7A2
		public Sprite Result { readonly get; private set; }
	}
}
