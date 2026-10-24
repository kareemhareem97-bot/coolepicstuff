using System;
using UnityEngine;

// Token: 0x020000F2 RID: 242
public class TextureMaterialLanguage : MonoBehaviour
{
	// Token: 0x06000493 RID: 1171 RVA: 0x00011735 File Offset: 0x0000F935
	private void Start()
	{
		this.mat.mainTexture = this.englishVersion;
	}

	// Token: 0x04000406 RID: 1030
	public Texture englishVersion;

	// Token: 0x04000407 RID: 1031
	public Material mat;
}
