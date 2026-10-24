using System;
using UnityEngine;

// Token: 0x020000D0 RID: 208
public class MaterialLanguageReplacer : MonoBehaviour
{
	// Token: 0x06000408 RID: 1032 RVA: 0x0000FD89 File Offset: 0x0000DF89
	private void Start()
	{
		base.GetComponent<Renderer>().material = this.englishVersion;
	}

	// Token: 0x06000409 RID: 1033 RVA: 0x0000FD9C File Offset: 0x0000DF9C
	private void OnDestroy()
	{
		UnityEngine.Object.Destroy(base.GetComponent<Renderer>().material);
	}

	// Token: 0x040003A3 RID: 931
	public Material englishVersion;
}
