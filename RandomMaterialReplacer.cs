using System;
using UnityEngine;

// Token: 0x020001AD RID: 429
public class RandomMaterialReplacer : MonoBehaviour
{
	// Token: 0x0600085F RID: 2143 RVA: 0x00021680 File Offset: 0x0001F880
	private void Start()
	{
		int num = UnityEngine.Random.Range(0, this.mats.Length);
		MeshRenderer[] componentsInChildren = base.GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].material = this.mats[num];
		}
	}

	// Token: 0x040007A2 RID: 1954
	public Material[] mats;
}
