using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000AC RID: 172
public class ClassPresetChooser : MonoBehaviour
{
	// Token: 0x0600038B RID: 907 RVA: 0x00002E9A File Offset: 0x0000109A
	private void Update()
	{
	}

	// Token: 0x04000324 RID: 804
	public GameObject bottomMenuItem;

	// Token: 0x04000325 RID: 805
	public Transform bottomMenuHolder;

	// Token: 0x04000326 RID: 806
	public ClassPresetChooser.PickerPreset[] presets;

	// Token: 0x04000327 RID: 807
	private string curKey;

	// Token: 0x04000328 RID: 808
	private List<ClassPresetChooser.PickerPreset> curPresets = new List<ClassPresetChooser.PickerPreset>();

	// Token: 0x020000AD RID: 173
	[Serializable]
	public class PickerPreset
	{
		// Token: 0x04000329 RID: 809
		public string classID;

		// Token: 0x0400032A RID: 810
		public Texture icon;

		// Token: 0x0400032B RID: 811
		public int health;

		// Token: 0x0400032C RID: 812
		public float wSpeed;

		// Token: 0x0400032D RID: 813
		public float rSpeed;

		// Token: 0x0400032E RID: 814
		public float stamina;

		// Token: 0x0400032F RID: 815
		public Texture[] startingItems;

		// Token: 0x04000330 RID: 816
		public string en_additionalInfo;

		// Token: 0x04000331 RID: 817
		public string pl_additionalInfo;
	}
}
